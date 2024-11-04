using System;

namespace Twizzar.Design.CoreInterfaces.TestCreation;

/// <summary>
/// Service which updates the <see cref="IProgress{T}"/>.
/// </summary>
public interface IProgressUpdater
{
    #region properties

    /// <summary>
    /// Gets the number of progress reports this service will make.
    /// </summary>
    int NumberOfProgressSteps { get; }

    #endregion

    #region members

    /// <summary>
    /// Add or overwrite the progress.
    /// </summary>
    /// <param name="progress"></param>
    void AddProgress(IProgress<TestCreationProgressData> progress);

    #endregion
}