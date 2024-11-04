using System.Threading;
using System.Threading.Tasks;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.CoreInterfaces.TestCreation.Config;

/// <summary>
/// Query for getting the mapping config.
/// </summary>
public interface IConfigQuery
{
    /// <summary>
    /// Gets the mapping config.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IResult<TestCreationConfig, Failure>> GetConfigAsync(string filePath, CancellationToken cancellationToken);
}