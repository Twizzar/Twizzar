using System;
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.TestCreation;
using ViCommon.Functional.Monads.MaybeMonad;

namespace TestCreation.Services;

/// <inheritdoc cref="IProgressUpdater" />
[ExcludeFromCodeCoverage]
public abstract class ProgressUpdater : IProgressUpdater
{
    private Maybe<IProgress<TestCreationProgressData>> _progress;

    #region properties

    /// <inheritdoc />
    public abstract int NumberOfProgressSteps { get; }

    #endregion

    #region members

    /// <inheritdoc />
    public void AddProgress(IProgress<TestCreationProgressData> progress)
    {
        this._progress = Maybe.Some(progress);
    }

    /// <summary>
    /// Report a message.
    /// </summary>
    /// <param name="message"></param>
    protected void Report(string message)
    {
        this._progress
            .IfSome(progress => progress.Report(new TestCreationProgressData(message)));
    }

    #endregion
}