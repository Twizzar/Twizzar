using System.Threading.Tasks;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.CoreInterfaces.Query.Services
{
    /// <summary>
    /// Query for getting the project name.
    /// </summary>
    public interface IProjectNameQuery
    {
        /// <summary>
        /// Get the project name form the root item path.
        /// </summary>
        /// <param name="rootItemPath"></param>
        /// <returns>The project name if successful else a failure.</returns>
        Task<IResult<string, Failure>> GetProjectNameAsync(Maybe<string> rootItemPath);
    }
}