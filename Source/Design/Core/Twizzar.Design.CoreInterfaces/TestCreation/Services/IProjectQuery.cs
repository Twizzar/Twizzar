using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.CoreInterfaces.TestCreation.Config;

namespace Twizzar.Design.CoreInterfaces.TestCreation;

/// <summary>
/// Query for getting a projects.
/// </summary>
public interface IProjectQuery : IProgressUpdater
{
    /// <summary>
    /// Get a project, if it not exist create it.
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="sourceContext"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    Task<IIdeProject> GetOrCreateProject(CreationInfo destination, CreationContext sourceContext, TestCreationConfig config);
}