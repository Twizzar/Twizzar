using System.Collections.Immutable;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services
{
    /// <summary>
    /// Factory methods for creating a <see cref="IConfigurationItem"/>.
    /// </summary>
    public interface IConfigurationItemFactory : IFactory
    {
        /// <summary>
        /// Create a new <see cref="IConfigurationItem"/>.
        /// </summary>
        /// <param name="id">The configuration id.</param>
        /// <param name="fixtureConfigurations">The fixture configurations.</param>
        /// <param name="memberConfigurations">The member configurations.</param>
        /// <param name="callbacks"></param>
        /// <returns>A new <see cref="IConfigurationItem"/>.</returns>
        public IConfigurationItem CreateConfigurationItem(
            FixtureItemId id,
            IImmutableDictionary<string, IFixtureConfiguration> fixtureConfigurations,
            IImmutableDictionary<string, IMemberConfiguration> memberConfigurations,
            IImmutableDictionary<string, IImmutableList<object>> callbacks);

        /// <summary>
        /// Create a new <see cref="IConfigurationItem"/>.
        /// </summary>
        /// <param name="id">The configuration id.</param>
        /// <param name="memberConfiguration">A member configuration.</param>
        /// <returns>A new <see cref="IConfigurationItem"/>.</returns>
        public IConfigurationItem CreateConfigurationItem(
            FixtureItemId id,
            IMemberConfiguration memberConfiguration);

        /// <summary>
        /// Create a new <see cref="IConfigurationItem"/>.
        /// </summary>
        /// <param name="id">The configuration id.</param>
        /// <returns>A new <see cref="IConfigurationItem"/>.</returns>
        public IConfigurationItem CreateConfigurationItem(
            FixtureItemId id);
    }
}
