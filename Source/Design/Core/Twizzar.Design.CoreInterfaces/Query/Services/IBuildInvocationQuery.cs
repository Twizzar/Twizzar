using System.Threading;
using System.Threading.Tasks;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.CoreInterfaces.Query.Services;

/// <summary>
/// Query for reading the build invocation information.
/// </summary>
public interface IBuildInvocationQuery
{
    #region members

    /// <summary>
    /// Get the number of build invocations in a certain project.
    /// </summary>
    /// <param name="projectName"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IResult<int, Failure>> GetBuildInvocationCountAsync(string projectName, CancellationToken token);

    #endregion
}