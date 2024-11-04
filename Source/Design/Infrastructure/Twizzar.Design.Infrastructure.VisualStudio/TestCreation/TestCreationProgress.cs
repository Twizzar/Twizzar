using System.Diagnostics.CodeAnalysis;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Twizzar.Design.CoreInterfaces.TestCreation;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation;

/// <inheritdoc />
[ExcludeFromCodeCoverage]
public sealed class TestCreationProgress : ITestCreationProgress
{
    #region fields

    private readonly int _numberOfSteps;
    private int _step = 1;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="TestCreationProgress"/> class.
    /// </summary>
    /// <param name="numberOfSteps"></param>
    public TestCreationProgress(int numberOfSteps)
    {
        this._numberOfSteps = numberOfSteps;
    }

    #endregion

    #region members

    /// <inheritdoc />
    public void Report(TestCreationProgressData value)
    {
        VS.StatusBar.ShowProgressAsync(value.Message, this._step, this._numberOfSteps)
            .FireAndForget(false);

        this._step++;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        VS.StatusBar.ShowProgressAsync("Finished", this._numberOfSteps, this._numberOfSteps);
        VS.StatusBar.ClearAsync();
    }

    #endregion
}