using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Infrastructure.VisualStudio.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Services;

/// <inheritdoc cref="IVsProjectQuery"/>
[ExcludeFromCodeCoverage]
public class VsProjectQuery : IVsProjectQuery
{
    #region fields

    private readonly DTE _dte;

    /// <summary>
    /// Initializes a new instance of the <see cref="VsProjectQuery"/> class.
    /// </summary>
    /// <param name="dte"></param>
    public VsProjectQuery(DTE dte)
    {
        this._dte = dte;
    }

    #endregion

    #region members

    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD010:Invoke single-threaded types on Main thread", Justification = "We are on the main thread the lambda does not change the thread.")]
    public async Task<Maybe<IIdeProject>> FindProjectAsync(string name)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        return await this._dte.Solution.GetAllProjects()
            .FirstOrNone(p => p.Name == name)
            .MapAsync(async project => (IIdeProject)new IdeProject(
                project,
                await VS.Solutions.FindProjectsAsync(name)));
    }

    #endregion
}