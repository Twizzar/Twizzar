using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

using static ViCommon.Functional.Monads.ResultMonad.Result;

namespace Twizzar.SharedKernel.Core.FixtureItem.Configuration
{
    /// <summary>
    /// A configuration item which holds the information about the configuration of a fixture item.
    /// </summary>
    public class ConfigurationItem : Entity<ConfigurationItem, FixtureItemId>, IConfigurationItem
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationItem"/> class.
        /// </summary>
        /// <param name="id">The configuration id.</param>
        /// <param name="fixtureConfigurations">The fixture configuration.</param>
        /// <param name="memberConfigurations">.</param>
        /// <param name="callbacks"></param>
        public ConfigurationItem(
            FixtureItemId id,
            IImmutableDictionary<string, IFixtureConfiguration> fixtureConfigurations,
            IImmutableDictionary<string, IMemberConfiguration> memberConfigurations,
            IImmutableDictionary<string, IImmutableList<object>> callbacks)
            : base(id)
        {
            this.EnsureMany()
                .Parameter(id, nameof(id))
                .Parameter(fixtureConfigurations, nameof(fixtureConfigurations))
                .Parameter(memberConfigurations, nameof(memberConfigurations))
                .ThrowWhenNull();

            this.Id = id;
            this.FixtureConfigurations = fixtureConfigurations;
            this.MemberConfigurations = memberConfigurations;
            this.Callbacks = callbacks;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the id of the configuration item (nullable name, and type).
        /// </summary>
        public FixtureItemId Id { get; }

        /// <inheritdoc />
        public IImmutableDictionary<string, IFixtureConfiguration> FixtureConfigurations { get; }

        /// <inheritdoc />
        public IImmutableDictionary<string, IMemberConfiguration> MemberConfigurations { get; }

        /// <inheritdoc />
        public IImmutableDictionary<string, IImmutableList<object>> Callbacks { get; }

        /// <inheritdoc />
        public IImmutableDictionary<string, IMemberConfiguration> OnlyVariableMemberConfiguration =>
            this.MemberConfigurations.Remove(".ctor");

        /// <inheritdoc />
        public IImmutableDictionary<string, IMemberConfiguration> OnlyCtorParameterMemberConfigurations =>
            this.MemberConfigurations.GetMaybe(ConfigurationConstants.CtorMemberName)
                .Map(configuration => (CtorMemberConfiguration)configuration)
                .Map(configuration => configuration.ConstructorParameters)
                .SomeOrProvided(ImmutableDictionary<string, IMemberConfiguration>.Empty);

        /// <inheritdoc />
        public Maybe<IMemberConfiguration> CtorConfiguration =>
            this.MemberConfigurations.GetMaybe(ConfigurationConstants.CtorMemberName);

        #endregion

        #region members

        /// <summary>
        /// Merge a new partial config into this config.
        /// This Config should be equal or more detailed than the otherItem.
        /// The other item is the more important config item and will override the config values of this.
        /// </summary>
        /// <param name="otherItem">The other item to merge into.</param>
        /// <returns>A new <see cref="ConfigurationItem"/> on success else a <see cref="InvalidConfigurationFailure"/>.</returns>
        public Result<IConfigurationItem, InvalidConfigurationFailure> Merge(IConfigurationItem otherItem)
        {
            this.EnsureParameter(otherItem, nameof(otherItem)).ThrowWhenNull();
            return this.MergeInternal(otherItem);
        }

        /// <summary>
        /// Merge a new partial config into this config.
        /// This Config should be equal or more detailed than the otherItem.
        /// The other item is the more important config item and will override the config values of this.
        /// </summary>
        /// <param name="otherItem">The other item to merge into or none. When none returns this.</param>
        /// <returns>A new <see cref="IConfigurationItem"/> on success else a <see cref="InvalidConfigurationFailure"/>.</returns>
        public Result<IConfigurationItem, InvalidConfigurationFailure> Merge(Maybe<IConfigurationItem> otherItem) =>
            otherItem.Match(
                some: this.Merge,
                none: Success<IConfigurationItem>(this));

        /// <summary>
        /// Merge this configuration item with another.
        /// </summary>
        /// <param name="b"></param>
        /// <returns>The merged configuration item.</returns>
        public object Merge(object b)
        {
            if (b is IConfigurationItem bConfigItem)
            {
                return this.Merge(bConfigItem)
                    .Match(
                        item => item,
                        failure =>
                        {
                            this.Log(failure.Message, LogLevel.Error);
                            return this;
                        });
            }

            return this;
        }

        /// <inheritdoc />
        public IConfigurationItem WithMemberConfigurations(
            IImmutableDictionary<string, IMemberConfiguration> memberConfigurations) =>
            new ConfigurationItem(this.Id, this.FixtureConfigurations, memberConfigurations, this.Callbacks);

        /// <inheritdoc />
        public IConfigurationItem AddOrUpdateMemberConfiguration(IMemberConfiguration memberConfiguration) =>
            new ConfigurationItem(
                this.Id,
                this.FixtureConfigurations,
                this.MemberConfigurations.AddOrUpdate(
                    memberConfiguration.Name,
                    memberConfiguration),
                this.Callbacks);

        /// <inheritdoc />
        public IConfigurationItem WithId(FixtureItemId id) =>
            new ConfigurationItem(
                id,
                this.FixtureConfigurations,
                this.MemberConfigurations,
                this.Callbacks);

        /// <summary>
        /// Add callbacks to the configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callbacks"></param>
        /// <returns></returns>
        public IConfigurationItem AddCallbacks(string key, IEnumerable<object> callbacks)
        {
            var newCallbacks = this.Callbacks.ContainsKey(key)
                ? this.Callbacks.SetItem(key, this.Callbacks[key].AddRange(callbacks))
                : this.Callbacks.Add(key, callbacks.ToImmutableList());

            return new ConfigurationItem(
                this.Id,
                this.FixtureConfigurations,
                this.MemberConfigurations,
                newCallbacks);
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"ConfigurationItem({this.Id.Name.Map(s => s.Replace(this.Id.RootItemPath.SomeOrProvided(string.Empty), string.Empty))})\n" +
            $"{this.MemberConfigurations.Select(pair => $"{pair.Key}: {pair.Value}").ToDisplayString("\n")} \n" +
            $"{this.Callbacks.Select(pair => $"{pair.Key}: {pair.Value.ToCommaSeparated()}").ToCommaSeparated()}";

        /// <inheritdoc />
        protected override bool Equals(FixtureItemId a, FixtureItemId b)
        {
            this.EnsureMany<FixtureItemId>()
                .Parameter(a, nameof(a))
                .Parameter(b, nameof(b))
                .ThrowWhenNull();

            return a.Equals(b);
        }

        /// <summary>
        /// Execute the <see cref="Merge(IConfigurationItem)"/> without the parameter validations.
        /// </summary>
        /// <param name="otherItem">The item to merge into.</param>
        /// <returns>A new <see cref="ConfigurationItem"/>.</returns>
        protected Result<IConfigurationItem, InvalidConfigurationFailure> MergeInternal(IConfigurationItem otherItem)
        {
            this.EnsureParameter(otherItem, nameof(otherItem)).ThrowWhenNull();

            var memberConfigurations =
                this.MemberConfigurations.Merge(otherItem.MemberConfigurations);

            var callbacks = this.Callbacks.AddRange(otherItem.Callbacks);

            return new ConfigurationItem(
                otherItem.Id,
                this.FixtureConfigurations.Merge(otherItem.FixtureConfigurations),
                memberConfigurations,
                callbacks);
        }

        #endregion
    }
}