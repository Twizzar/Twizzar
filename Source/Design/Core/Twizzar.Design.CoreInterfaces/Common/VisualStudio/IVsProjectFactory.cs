using System.Threading.Tasks;

namespace Twizzar.Design.CoreInterfaces.Common.VisualStudio;

/// <summary>
/// Factory for creating a new project in Visual Studio.
/// </summary>
public interface IVsProjectFactory
{
    /// <summary>
    /// Create a new project.
    /// </summary>
    /// <param name="projectName">The name of the project.</param>
    /// <param name="projectDirectory">The absolute path to the project directory.</param>
    /// <param name="netVersion">The Target framework moniker (TFM) see https://learn.microsoft.com/en-us/dotnet/standard/frameworks.</param>
    /// <returns></returns>
    Task<IIdeProject> CreateProjectAsync(string projectName, string projectDirectory, string netVersion);
}