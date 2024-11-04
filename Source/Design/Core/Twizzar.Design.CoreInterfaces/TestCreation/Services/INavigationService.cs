using System.Threading;
using System.Threading.Tasks;
using ViCommon.Functional;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.CoreInterfaces.TestCreation;

/// <summary>
/// Service for the navigation between the code to test and the unit test code.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigate to the target.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IResult<Unit, Failure>> NavigateAsync(CreationInfo target, CancellationToken cancellationToken);

    /// <summary>
    /// Navigate form the unit test back.
    /// </summary>
    /// <param name="sourceContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IResult<Unit, Failure>> NavigateBackAsync(CreationContext sourceContext, CancellationToken cancellationToken);
}