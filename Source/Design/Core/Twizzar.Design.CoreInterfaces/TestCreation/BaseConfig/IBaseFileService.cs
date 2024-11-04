using System.IO;
using System.Threading.Tasks;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.TestCreation.BaseConfig;

/// <summary>
/// Base service for reading and creating config files.
/// </summary>
public interface IBaseFileService
{
    /// <summary>
    /// Get the file from filePath, if the file doesn't exist a new
    /// file with the default content is created.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    Task<Maybe<TextReader>> GetFileReaderAsync(string filePath);

    /// <summary>
    /// Gets the default file content.
    /// </summary>
    /// <returns></returns>
    Task<TextReader> GetDefaultFileReaderAsync();

    /// <summary>
    /// Create a file at the file path with the default content.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    Task CreateDefaultFile(string filePath);
}