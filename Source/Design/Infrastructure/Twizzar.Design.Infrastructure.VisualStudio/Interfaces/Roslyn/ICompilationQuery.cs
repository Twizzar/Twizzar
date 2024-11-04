using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio2019.Interfaces.Roslyn;

/// <summary>
/// Query for getting a <see cref="Compilation"/>.
/// </summary>
public interface ICompilationQuery
{
    /// <summary>
    /// Get a <see cref="Compilation"/> form a <see cref="ITextBuffer"/>.
    /// </summary>
    /// <param name="textBuffer"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    Task<Maybe<Compilation>> GetFromBufferAsync(ITextBuffer textBuffer, CancellationToken cancellation = default);
}