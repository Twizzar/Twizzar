using System.Threading.Tasks;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.TestCreation;

/// <summary>
/// Interface of a service for creating or retrieving a document.
/// </summary>
public interface IDocumentQuery : IProgressUpdater
{
    /// <summary>
    /// Gets the described file if it exists or tries to creates a new document.
    /// Throws an exception when the retry count is exceeded.
    /// </summary>
    /// <param name="destination">Info defining the destination file.</param>
    /// <param name="sourceContext">Context describing the source.</param>
    /// <returns>Result task containing the context of the destination file.</returns>
    Task<Maybe<CreationContext>> GetOrCreateDocumentAsync(CreationInfo destination, CreationContext sourceContext);
}