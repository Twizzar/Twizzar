using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.TestCreation;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation;

/// <inheritdoc cref="ITestCreationProgressFactory" />
[ExcludeFromCodeCoverage]
public class TestCreationProgressFactory : ITestCreationProgressFactory
{
    #region members

    /// <inheritdoc />
    public ITestCreationProgress Create(int numberOfSteps) =>
        new TestCreationProgress(numberOfSteps);

    #endregion
}