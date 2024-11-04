using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Query.Services.ReadModel;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.CoreInterfaces.Query.Services
{
    /// <summary>
    /// Query for requesting a <see cref="IFixtureItemModel"/>.
    /// </summary>
    public interface IFixtureItemReadModelQuery : IQuery
    {
        /// <summary>
        /// Get a <see cref="IFixtureItemModel"/> with the specific id.
        /// </summary>
        /// <param name="id">The fixture item id.</param>
        /// <returns>A new view model.</returns>
        public Task<IResult<IFixtureItemModel, Failure>> GetFixtureItem(FixtureItemId id);
    }
}
