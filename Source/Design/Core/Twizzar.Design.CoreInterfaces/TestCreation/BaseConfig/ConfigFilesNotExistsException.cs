using System;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;

namespace Twizzar.Design.CoreInterfaces.TestCreation.BaseConfig;

/// <summary>
/// Exception thrown when one or more config files not exits.
/// </summary>
public class ConfigFilesNotExistsException : Exception
{
    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigFilesNotExistsException"/> class.
    /// </summary>
    /// <param name="files"></param>
    public ConfigFilesNotExistsException(params string[] files)
        : base(GetMessage(files))
    {
        this.Files = files;
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets the full path to the not existing files.
    /// </summary>
    public string[] Files { get; }

    #endregion

    #region members

    /// <inheritdoc />
    public override string ToString() => GetMessage(this.Files);

    private static string GetMessage(string[] files) =>
        files switch
        {
            { Length: 1 } => $"Config file {files[0]} was not found.",
            { Length: 2 } => $"Config files {files[0]} and {files[1]} were not found",
            { Length: > 2 } => $"Config files: {files.ToCommaSeparated()} were not found",
            _ => $"Config files where not found",
        };

    #endregion
}