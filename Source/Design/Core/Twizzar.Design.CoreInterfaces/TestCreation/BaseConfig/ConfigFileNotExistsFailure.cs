using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.CoreInterfaces.TestCreation.BaseConfig;

/// <summary>
/// Failure when a config file does not exists.
/// </summary>
public class ConfigFileNotExistsFailure : Failure
{
    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigFileNotExistsFailure"/> class.
    /// </summary>
    /// <param name="filePath"></param>
    public ConfigFileNotExistsFailure(string filePath)
        : base($"{filePath} does not exists.")
    {
        this.FilePath = filePath;
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets the file path.
    /// </summary>
    public string FilePath { get; }

    #endregion
}