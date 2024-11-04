using Twizzar.SharedKernel.Core.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.Factories;

namespace Twizzar.TestCommon.Configuration.Builders
{
    /// <summary>
    /// Builder for configuration item factory.
    /// </summary>
    public class ConfigurationItemFactoryBuilder
    {
        /// <summary>
        /// Builds up the factory.
        /// </summary>
        /// <returns>The <see cref="IConfigurationItemFactory"/> instance.</returns>
        public IConfigurationItemFactory Build()
        {
            return new ConfigurationItemFactory(
                null,
                (id, configurations, memberConfigurations, callbacks) =>
                    new ConfigurationItem(id, configurations, memberConfigurations, callbacks));
        }
    }
}
