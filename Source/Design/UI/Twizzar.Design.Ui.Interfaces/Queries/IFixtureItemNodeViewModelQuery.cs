using System.Collections.Generic;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Ui.Interfaces.Queries
{
    /// <summary>
    /// Queries the ReadModel and creates corresponding <see cref="IFixtureItemNodeViewModel"/> out of the result.
    /// </summary>
    /// <seealso cref="IQuery" />
    public interface IFixtureItemNodeViewModelQuery : IQuery
    {
        /// <summary>
        /// Creates a List of the <see cref="IFixtureItemNodeViewModel"/> according of a fixture definition id.
        /// </summary>
        /// <param name="id">The fixture item id.</param>
        /// <param name="memberName">The member name.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="compilationTypeQuery"></param>
        /// <returns>The created List of the <see cref="IFixtureItemNodeViewModel"/>.</returns>
        Task<IResult<IEnumerable<IFixtureItemNodeViewModel>, Failure>> GetFixtureItemNodeViewModels(
            FixtureItemId id,
            Maybe<string> memberName,
            Maybe<IFixtureItemNode> parent,
            ICompilationTypeQuery compilationTypeQuery);
    }
}