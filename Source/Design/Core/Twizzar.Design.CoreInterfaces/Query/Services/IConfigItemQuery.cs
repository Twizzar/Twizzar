using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.CoreInterfaces.Query.Services
{
    /// <summary>
    /// Query for requesting a complete <see cref="IConfigurationItem"/>, with system default and user configuration.
    /// </summary>
    public interface IConfigItemQuery
    {
        /// <summary>
        /// Get a <see cref="IConfigurationItem"/> for a specific fixture item id.
        /// </summary>
        /// <param name="id">The fixture item id.</param>
        /// <param name="typeDescription">The type description.</param>
        /// <returns>A complete <see cref="IConfigurationItem"/>.</returns>
        IConfigurationItem GetConfigurationItem(FixtureItemId id, ITypeDescription typeDescription);
    }
}
