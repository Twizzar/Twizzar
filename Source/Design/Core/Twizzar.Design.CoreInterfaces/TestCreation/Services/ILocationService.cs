using System.Threading.Tasks;

namespace Twizzar.Design.CoreInterfaces.TestCreation;

/// <summary>
/// Interface for collecting the <see cref="CreationContext"/> of the current caret position..
/// </summary>
public interface ILocationService
{
    /// <summary>
    /// Gets the <see cref="CreationContext"/> of the current caret position.
    /// </summary>
    /// <param name="filePath">The file path of the document.</param>
    /// <param name="cursorIndex">Zero based cursor index.</param>
    /// <returns></returns>
    Task<CreationContext> GetCurrentLocation(string filePath, int cursorIndex);

    /// <summary>
    /// Check of the current location is a valid location.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="cursorIndex"></param>
    /// <returns></returns>
    Task<bool> CheckIfValidLocationAsync(string filePath, int cursorIndex);
}