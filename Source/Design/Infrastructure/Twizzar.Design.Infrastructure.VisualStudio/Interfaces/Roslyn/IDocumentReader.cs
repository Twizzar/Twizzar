using System.Collections.Generic;
using System.Threading;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;

namespace Twizzar.Design.CoreInterfaces.Adornment
{
    /// <summary>
    /// Interface to collect the <see cref="IAdornmentInformation"/> from a source file.
    /// </summary>
    public interface IDocumentReader
    {
        /// <summary>
        /// Get the adornment information for a source file.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>collected <see cref="IAdornmentInformation"/>.</returns>
        IEnumerable<IAdornmentInformation> GetAdornmentInformation(IRoslynContext context, CancellationToken cancellationToken = default);
    }
}