namespace Twizzar.Design.CoreInterfaces.TestCreation.ProjectCreation;

/// <summary>
/// Query for getting the Target framework moniker.
/// </summary>
public interface ITargetFrameworkMonikerQuery
{
    /// <summary>
    /// Get the Target framework moniker.
    /// </summary>
    /// <param name="projectPath">The path to the project.</param>
    /// <returns></returns>
    string GetTargetFrameworkMoniker(string projectPath);
}