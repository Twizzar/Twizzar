using System.Collections.Generic;
using System.Collections.Immutable;
using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.Util;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration
{
    /// <summary>
    /// A configuration item which holds the information about the configuration of a fixture item.
    /// </summary>
    public interface IConfigurationItem : IEntity, IMergeable
    {
        /// <summary>
        /// Gets the id of the configuration item (nullable name, and type).
        /// </summary>
        FixtureItemId Id { get; }

        /// <summary>
        /// Gets the fixture configurations.
        /// </summary>
        IImmutableDictionary<string, IFixtureConfiguration> FixtureConfigurations { get; }

        /// <summary>
        /// Gets the member configurations.
        /// </summary>
        IImmutableDictionary<string, IMemberConfiguration> MemberConfigurations { get; }

        /// <summary>
        /// Gets the method callbacks.
        /// </summary>
        IImmutableDictionary<string, IImmutableList<object>> Callbacks { get; }

        /// <summary>
        /// Gets the filter out the Variable Members (Fields, properties and methods) form the <see cref="MemberConfigurations"/>.
        /// </summary>
        IImmutableDictionary<string, IMemberConfiguration> OnlyVariableMemberConfiguration { get; }

        /// <summary>
        /// Gets the filter out the Constructor Parameters from the <see cref="MemberConfigurations"/>.
        /// </summary>
        IImmutableDictionary<string, IMemberConfiguration> OnlyCtorParameterMemberConfigurations { get; }

        /// <summary>
        /// Gets the ctor configuration, which can be None.
        /// </summary>
        Maybe<IMemberConfiguration> CtorConfiguration { get; }

        /// <summary>
        /// Merge a new partial config into this config.
        /// This Config should be equal or more detailed than the otherItem.
        /// The other item is the more important config item and will override the config values of this.
        /// </summary>
        /// <param name="otherItem">The other item to merge into.</param>
        /// <returns>A new <see cref="IConfigurationItem"/> on success else a <see cref="InvalidConfigurationFailure"/>.</returns>
        Result<IConfigurationItem, InvalidConfigurationFailure> Merge(
            IConfigurationItem otherItem);

        /// <summary>
        /// Merge a new partial config into this config.
        /// This Config should be equal or more detailed than the otherItem.
        /// The other item is the more important config item and will override the config values of this.
        /// </summary>
        /// <param name="otherItem">The other item to merge into or none. When none returns this.</param>
        /// <returns>A new <see cref="IConfigurationItem"/> on success else a <see cref="InvalidConfigurationFailure"/>.</returns>
        Result<IConfigurationItem, InvalidConfigurationFailure> Merge(
            Maybe<IConfigurationItem> otherItem);

        /// <summary>
        /// Create a copy of the <see cref="IConfigurationItem"/> with a new memberConfigurations.
        /// </summary>
        /// <param name="memberConfigurations">The new member configurations.</param>
        /// <returns>A new configuration item.</returns>
        IConfigurationItem WithMemberConfigurations(
            IImmutableDictionary<string, IMemberConfiguration> memberConfigurations);

        /// <summary>
        /// Create a copy of the <see cref="IConfigurationItem"/> and add the memberConfigurations.
        /// </summary>
        /// <param name="memberConfiguration">The new member configuration.</param>
        /// <returns>A new configuration item.</returns>
        IConfigurationItem AddOrUpdateMemberConfiguration(
            IMemberConfiguration memberConfiguration);

        /// <summary>
        /// Creates a copy of the config item with a new id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IConfigurationItem WithId(FixtureItemId id);

        /// <summary>
        /// Creates a copy of the config item and add new callbacks to it.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callbacks"></param>
        /// <returns></returns>
        IConfigurationItem AddCallbacks(string key, IEnumerable<object> callbacks);
    }
}
