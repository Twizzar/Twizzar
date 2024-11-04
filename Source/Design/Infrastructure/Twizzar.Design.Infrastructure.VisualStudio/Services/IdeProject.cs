using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Community.VisualStudio.Toolkit;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.ServiceBroker;

using NuGet.VisualStudio;
using NuGet.VisualStudio.Contracts;

using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Infrastructure.VisualStudio.Models;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.Functional;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

using Project = EnvDTE.Project;
using TProject = Community.VisualStudio.Toolkit.Project;

namespace Twizzar.Design.Infrastructure.VisualStudio.Services;

/// <inheritdoc cref="IIdeProject" />
[ExcludeFromCodeCoverage]
public class IdeProject : VsProject, IIdeProject
{
    #region fields

    private readonly Project _dteProject;
    private readonly TProject _toolkitProject;
    private InstalledPackagesResult _cached;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeProject"/> class.
    /// </summary>
    /// <param name="dteProject"></param>
    /// <param name="toolkitProject"></param>
    public IdeProject(Project dteProject, TProject toolkitProject)
        : base(dteProject)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        this._dteProject = dteProject;
        this._toolkitProject = toolkitProject;
    }

    #endregion

    #region properties

    /// <inheritdoc />
    public bool IsLoaded => this._toolkitProject.IsLoaded;

    #endregion

    #region members

    /// <inheritdoc />
    public async Task AddNugetPackageAsync(string packageId, string version = null)
    {
        try
        {
            await this.AddNugetPackageInternalAsync(packageId, version, 20, new List<Exception>());
        }
        catch (Exception e)
        {
            this.Log(e);
        }
    }

    /// <inheritdoc />
    public async Task<IResult<Unit, Failure>> AddProjectReferenceAsync(IViProject project)
    {
        try
        {
            var otherProject = await VS.Solutions.FindProjectsAsync(project.Name);

            if (otherProject is null)
            {
                return new Failure($"Cannot find the project {project.Name}").ToResult<Unit, Failure>();
            }

            await this._toolkitProject.References.AddAsync(otherProject);
            return Unit.New.ToSuccess<Unit, Failure>();
        }
        catch (Exception e)
        {
            return new Failure(e.Message).ToResult<Unit, Failure>();
        }
    }

    private async Task<Maybe<InstalledPackagesResult>> GetInstalledPackagesAsync()
    {
        // this cache only works because the IdeProject gets constructed new every time needed and not cached.
        if (this._cached is not null)
        {
            return this._cached;
        }

        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var brokeredServiceContainer =
            await VS.GetServiceAsync<SVsBrokeredServiceContainer, IBrokeredServiceContainer>();

        if (brokeredServiceContainer is null)
        {
            Maybe.None();
        }

        var serviceBroker = brokeredServiceContainer.GetFullAccessServiceBroker();

        var nugetProjectService =
            await serviceBroker.GetProxyAsync<INuGetProjectService>(NuGetServices.NuGetProjectServiceV1);

        using (nugetProjectService as IDisposable)
        {
            if (nugetProjectService is null)
            {
                return Maybe.None();
            }

            var vsSolution = await VS.Services.GetSolutionAsync();

            vsSolution.GetProjectOfUniqueName(this._dteProject.UniqueName, out var hierarchy);
            vsSolution.GetGuidOfProject(hierarchy, out var guid);
            var packages = await nugetProjectService.GetInstalledPackagesAsync(guid, CancellationToken.None);

            if (packages.Packages is null)
            {
                return Maybe.None();
            }

            this._cached = packages;
            return this._cached;
        }
    }

    private async Task AddNugetPackageInternalAsync(
        string packageId,
        string version,
        int remainingRetries,
        ICollection<Exception> exceptions)
    {
        if (remainingRetries <= 0)
        {
            throw new InternalException($"Cannot install the nuget package {packageId} \n {exceptions.Skip(exceptions.Count - 3).ToDisplayString("\n")}");
        }

        var maybeInstalledPackages = await this.GetInstalledPackagesAsync();

        if (maybeInstalledPackages.AsMaybeValue() is SomeValue<InstalledPackagesResult> installedPackages &&
            installedPackages.Value.Packages.Any(package => package.Id == packageId))
        {
            return;
        }

        try
        {
            var installer = await VS.GetMefServiceAsync<IVsPackageInstaller2>();

            if (version is null)
            {
                installer.InstallLatestPackage(null, this._dteProject, packageId, false, false);
            }
            else
            {
                installer.InstallPackage(null, this._dteProject, packageId, version, false);
            }
        }

        // NuGet.PackageManagement.VisualStudio.Exceptions.ProjectNotNominatedException
        catch (Exception e) when (e.Source == "NuGet.PackageManagement.VisualStudio")
        {
            await Task.Delay(500);
            exceptions.Add(e);
            await this.AddNugetPackageInternalAsync(packageId, version, remainingRetries - 1, exceptions);
        }
    }

    #endregion
}