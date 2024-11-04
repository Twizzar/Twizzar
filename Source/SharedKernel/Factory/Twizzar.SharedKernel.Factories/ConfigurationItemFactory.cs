using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Autofac;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.SharedKernel.Factories
{
    /// <inheritdoc cref="IConfigurationItemFactory" />
    [ExcludeFromCodeCoverage]
    public class ConfigurationItemFactory : FactoryBase, IConfigurationItemFactory
    {
        private readonly FactoryDelegate _factoryDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationItemFactory"/> class.
        /// </summary>
        /// <param name="componentContext">The autofac component context.</param>
        /// <param name="factoryDelegate">Factory for autofac.</param>
        public ConfigurationItemFactory(IComponentContext componentContext, FactoryDelegate factoryDelegate)
            : base(componentContext)
        {
            this._factoryDelegate = factoryDelegate;
        }

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public delegate IConfigurationItem FactoryDelegate(
            FixtureItemId id,
            IImmutableDictionary<string, IFixtureConfiguration> fixtureConfigurations,
            IImmutableDictionary<string, IMemberConfiguration> memberConfigurations,
            IImmutableDictionary<string, IImmutableList<object>> callbacks);
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <inheritdoc />
        public IConfigurationItem CreateConfigurationItem(
            FixtureItemId id,
            IImmutableDictionary<string, IFixtureConfiguration> fixtureConfigurations,
            IImmutableDictionary<string, IMemberConfiguration> memberConfigurations,
            IImmutableDictionary<string, IImmutableList<object>> callbacks) =>
                this._factoryDelegate(id, fixtureConfigurations, memberConfigurations, callbacks);

        /// <inheritdoc />
        public IConfigurationItem CreateConfigurationItem(
            FixtureItemId id,
            IMemberConfiguration memberConfiguration)
        {
            this.EnsureParameter(memberConfiguration, nameof(memberConfiguration)).ThrowWhenNull();

            return this._factoryDelegate(
                id,
                ImmutableDictionary<string, IFixtureConfiguration>.Empty,
                ImmutableDictionary<string, IMemberConfiguration>.Empty.Add(
                    memberConfiguration.Name, memberConfiguration),
                ImmutableDictionary.Create<string, IImmutableList<object>>());
        }

        /// <inheritdoc />
        public IConfigurationItem CreateConfigurationItem(FixtureItemId id) =>
            this._factoryDelegate(
                id,
                ImmutableDictionary<string, IFixtureConfiguration>.Empty,
                ImmutableDictionary<string, IMemberConfiguration>.Empty,
                ImmutableDictionary<string, IImmutableList<object>>.Empty);
    }
}
