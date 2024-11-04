namespace Twizzar.Design.CoreInterfaces.TestCreation;

/// <summary>
/// Factory for creating a <see cref="ITestCreationProgress"/>.
/// </summary>
public interface ITestCreationProgressFactory
{
    /// <summary>
    /// Create a new instance of <see cref="ITestCreationProgress"/>.
    /// </summary>
    /// <param name="numberOfSteps"></param>
    /// <returns></returns>
    ITestCreationProgress Create(int numberOfSteps);
}