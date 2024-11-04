using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Twizzar.Design.Infrastructure.Services;

/// <inheritdoc cref="IFileService" />
[ExcludeFromCodeCoverage]
public class FileService : IFileService
{
    #region members

    /// <inheritdoc />
    public string[] ReadAllLines(string path) => File.ReadAllLines(path);

    /// <inheritdoc />
    public bool Exists(string path) => File.Exists(path);

    /// <inheritdoc />
    public void Create(string path) => File.Create(path);

    /// <inheritdoc />
    public void WriteAllLines(string path, string[] contents) => File.WriteAllLines(path, contents);

    #endregion
}