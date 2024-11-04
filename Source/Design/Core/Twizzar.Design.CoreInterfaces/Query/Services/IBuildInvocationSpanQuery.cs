using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Adornment;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.CoreInterfaces.Query.Services
{
    /// <summary>
    /// Query for getting the invocation span form Build.New(..)
    /// </summary>
    public interface IBuildInvocationSpanQuery
    {
        /// <summary>
        /// Get the project name form the root item path.
        /// </summary>
        /// <param name="rootItemPath"></param>
        /// <returns>The project name if successful else a failure.</returns>
        Task<IResult<IViSpan, Failure>> GetSpanAsync(Maybe<string> rootItemPath);
    }
}