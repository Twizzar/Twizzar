using System.Threading.Tasks;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services
{
    /// <summary>
    /// Query for retrieving a configuration item.
    /// </summary>
    public interface IConfigurationItemQuery : IQuery
    {
        /// <summary>
        /// Gets a fully defined configuration item for a fixture item.
        /// </summary>
        /// <param name="id">The fixture item id.</param>
        /// <param name="typeDescription">Type description of the fixture item.</param>
        /// <returns>A fully defined configuration item.</returns>
        Task<IConfigurationItem> GetConfigurationItem(
            FixtureItemId id,
            ITypeDescription typeDescription);
    }
}
