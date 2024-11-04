using System.Threading;
using System.Threading.Tasks;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.SharedKernel.CoreInterfaces;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn
{
    /// <summary>
    /// Query for returning a <see cref="IRoslynContext"/>.
    /// </summary>
    public interface IRoslynContextQuery : IQuery
    {
        /// <summary>
        /// Get the <see cref="IRoslynContext"/> for the file path.
        /// </summary>
        /// <param name="filePath">The absolute file path to the document.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Success if the document was found else a failure.</returns>
        Task<IResult<IRoslynContext, Failure>> GetContextAsync(string filePath, CancellationToken cancellationToken = default);
    }
}