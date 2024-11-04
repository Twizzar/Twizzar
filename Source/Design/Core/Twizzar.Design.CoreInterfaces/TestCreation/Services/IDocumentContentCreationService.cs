using System.Threading.Tasks;

namespace Twizzar.Design.CoreInterfaces.TestCreation;

/// <summary>
/// Service for creating the content of a unit test document.
/// </summary>
public interface IDocumentContentCreationService : IProgressUpdater
{
    /// <summary>
    /// Create the content of a document or update it.
    /// </summary>
    /// <param name="destination">Context about the unit test destination.</param>
    /// <returns></returns>
    Task<bool> CreateContentAsync(CreationContext destination);
}