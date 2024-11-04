using System.Collections.Generic;
using System.Threading;
using Twizzar.Analyzer;
using Twizzar.Design.Shared.Infrastructure.Discovery;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Analyzer2022.App.Interfaces
{
    /// <summary>
    /// Service for building the class which contains all the extension methods, for the ItemBuilders.
    /// </summary>
    public interface IBuilderExtensionMethodSourceTextGenerator
    {
        /// <summary>
        /// Generate the class.
        /// </summary>
        /// <param name="creationInformation"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Success with the code, else Failure where Failure can be a <see cref="OperationCanceledFailure"/>.</returns>
        IResult<string, Failure> GenerateClass(IEnumerable<ItemBuilderCreationInformation> creationInformation, CancellationToken cancellationToken);
    }
}