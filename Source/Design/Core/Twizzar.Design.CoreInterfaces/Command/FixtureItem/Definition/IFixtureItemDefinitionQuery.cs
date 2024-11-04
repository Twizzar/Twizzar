using System.Threading.Tasks;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.CoreInterfaces.Command.FixtureItem.Definition
{
    /// <summary>
    /// Query for a <see cref="IFixtureItemDefinitionNode"/>.
    /// </summary>
    public interface IFixtureItemDefinitionQuery : IQuery
    {
        /// <summary>
        /// Get the definition node for the corresponding <see cref="FixtureItemId"/>.
        /// </summary>
        /// <param name="id">The fixture item id.</param>
        /// <returns>A new instance of <see cref="IFixtureItemDefinitionNode"/>.</returns>
        Task<IResult<IFixtureItemDefinitionNode, Failure>> GetDefinitionNode(FixtureItemId id);
    }
}
