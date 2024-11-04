using System.Threading.Tasks;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.Services
{
    /// <summary>
    /// Query for a <see cref="IFixtureItemDefinitionNode"/>.
    /// </summary>
    public interface IFixtureItemDefinitionQuery : IQuery
    {
        /// <summary>
        /// Get the definition node for the corresponding <see cref="FixtureItemId"/>.
        /// </summary>
        /// <param name="fixtureItemId">The fixture item id.</param>
        /// <returns>A new instance of <see cref="IFixtureItemDefinitionNode"/>.</returns>
        Task<IFixtureItemDefinitionNode> GetDefinitionNode(FixtureItemId fixtureItemId);
    }
}
