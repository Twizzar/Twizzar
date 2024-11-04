using System.Threading.Tasks;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.CoreInterfaces.Command.FixtureItem.Definition
{
    /// <summary>
    /// Factory for getting or creating <see cref="IFixtureItemDefinitionNode"/>.
    /// </summary>
    public interface IFixtureItemDefinitionRepository : IService
    {
        /// <summary>
        /// Creates a fixture item.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The created fixture item definition node if the node does not exists, else the existed node.</returns>
        Task<IResult<IFixtureItemDefinitionNode, Failure>> CreateFixtureItem(
            FixtureItemId id);

        /// <summary>
        /// Restores the definition node.
        /// </summary>
        /// <param name="id">The fixture item id.</param>
        /// <returns>The fixture item which is up to date with the relevant events form the event store.</returns>
        Task<IResult<IFixtureItemDefinitionNode, Failure>> RestoreDefinitionNode(FixtureItemId id);

        /// <summary>
        /// Check if the fixture item exists in the event store.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>True if it exist else false.</returns>
        Task<bool> FixtureItemExitsInEventStore(FixtureItemId id);
    }
}
