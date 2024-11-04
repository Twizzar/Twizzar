using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Infrastructure.VisualStudio.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio.Services;

/// <inheritdoc cref="IVsProjectFactory" />
[ExcludeFromCodeCoverage]
public class VsProjectFactory : IVsProjectFactory
{
    #region fields

    private readonly DTE2 _dte2;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="VsProjectFactory"/> class.
    /// </summary>
    /// <param name="dte2"></param>
    public VsProjectFactory(DTE2 dte2)
    {
        this._dte2 = dte2;
    }

    #endregion

    #region members

    /// <inheritdoc />
    [SuppressMessage(
        "Usage",
        "VSTHRD010:Invoke single-threaded types on Main thread",
        Justification = "We are on the main thread the lambda does not change the thread.")]
    public async Task<IIdeProject> CreateProjectAsync(string projectName, string projectDirectory, string netVersion)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var solution = this._dte2.Solution;
        var solution2 = (Solution2)solution;

        var projectTemplate = solution2.GetProjectTemplate("TwizzarProject.zip", "CSharp");
        solution2.AddFromTemplate($"{projectTemplate}|netVersion={netVersion}", projectDirectory, projectName);

        var project = solution.GetAllProjects().Single(p => p.Name == projectName);
        var tProject = await VS.Solutions.FindProjectsAsync(project.Name);
        return new IdeProject(project, tProject);
    }

    #endregion
}