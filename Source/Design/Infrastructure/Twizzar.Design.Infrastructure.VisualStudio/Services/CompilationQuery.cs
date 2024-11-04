using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Twizzar.Design.Infrastructure.VisualStudio2019.Interfaces.Roslyn;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Services;

/// <inheritdoc cref="ICompilationQuery"/>
public class CompilationQuery : ICompilationQuery
{
    #region members

    /// <inheritdoc />
    public Task<Maybe<Compilation>>
        GetFromBufferAsync(ITextBuffer textBuffer, CancellationToken cancellation = default) =>
        textBuffer.GetRelatedDocuments()
            .FirstOrNone()
            .MapAsync(document => document.Project.GetCompilationAsync(cancellation));

    #endregion
}