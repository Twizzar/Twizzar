using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.CoreInterfaces.TestCreation.Config;
using Twizzar.Design.CoreInterfaces.TestCreation.ProjectCreation;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace TestCreation.Services;

/// <inheritdoc cref="IProjectQuery"/>
public class ProjectQuery : ProgressUpdater, IProjectQuery
{
    #region fields

    private readonly IVsProjectQuery _vsProjectQuery;
    private readonly IVsProjectFactory _vsProjectFactory;
    private readonly IUserFeedbackService _feedbackService;
    private readonly ITargetFrameworkMonikerQuery _frameworkMonikerQuery;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectQuery"/> class.
    /// </summary>
    /// <param name="vsProjectQuery"></param>
    /// <param name="vsProjectFactory"></param>
    /// <param name="frameworkMonikerQuery"></param>
    /// <param name="feedbackService"></param>
    public ProjectQuery(
        IVsProjectQuery vsProjectQuery,
        IVsProjectFactory vsProjectFactory,
        ITargetFrameworkMonikerQuery frameworkMonikerQuery,
        IUserFeedbackService feedbackService)
    {
        EnsureHelper.GetDefault.Many()
            .Parameter(vsProjectQuery, nameof(vsProjectQuery))
            .Parameter(vsProjectFactory, nameof(vsProjectFactory))
            .Parameter(frameworkMonikerQuery, nameof(frameworkMonikerQuery))
            .Parameter(feedbackService, nameof(feedbackService))
            .ThrowWhenNull();

        this._vsProjectQuery = vsProjectQuery;
        this._vsProjectFactory = vsProjectFactory;
        this._frameworkMonikerQuery = frameworkMonikerQuery;
        this._feedbackService = feedbackService;
    }

    #endregion

    #region properties

    /// <inheritdoc />
    public override int NumberOfProgressSteps => 3;

    #endregion

    #region members

    /// <inheritdoc />
    public async Task<IIdeProject> GetOrCreateProject(
        CreationInfo destination,
        CreationContext sourceContext,
        TestCreationConfig config)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        var project = await this.GetOrCreateProjectAsync(destination, sourceContext);

        if (!SpinWait.SpinUntil(() => project.IsLoaded, TimeSpan.FromSeconds(2)))
        {
            throw new InternalException("Project took to long to load.");
        }

        this.Report($"Check for missing references");

        foreach (var package in config.AdditionalNugetPackages)
        {
            var version = package.Version.IsSome
                ? package.Version.GetValueUnsafe()
                : null;
            await project.AddNugetPackageAsync(package.PackageId, version);
        }

        var sourceProjectName = sourceContext.Info.Project.SplitPath().FileName;

        var sourceProject = (await this._vsProjectQuery.FindProjectAsync(sourceProjectName))
            .SomeOrProvided(() =>
                throw new InternalException($"Cannot find the project {sourceProjectName}"));

        await project.AddProjectReferenceAsync(sourceProject);

        return project;
    }

    private async Task<IIdeProject> GetOrCreateProjectAsync(CreationInfo destination, CreationContext sourceContext)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var (prefix, fileName, _) = destination.Project.SplitPath();
        this.Report($"Search for project {fileName}");
        var project = await this._vsProjectQuery.FindProjectAsync(fileName);

        if (project.AsMaybeValue() is SomeValue<IIdeProject> someProject)
        {
            this.Report($"Project {fileName} found");
            return someProject.Value;
        }

        if (!await this._feedbackService.ShowYesNoBoxAsync(
                $"Project {fileName} was not found, do you want to create the project?"))
        {
            throw new UserCanceledException("Test creation was cancelled by the user.");
        }

        var targetFramework =
            this._frameworkMonikerQuery.GetTargetFrameworkMoniker(sourceContext.Info.Project);

        this.Report($"Project not found, {fileName} will be created.");
        return await this._vsProjectFactory.CreateProjectAsync(fileName, prefix, targetFramework);
    }

    #endregion
}