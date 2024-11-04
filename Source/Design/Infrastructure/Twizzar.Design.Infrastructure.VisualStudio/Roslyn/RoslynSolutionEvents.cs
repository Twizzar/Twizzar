using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using Twizzar.Design.CoreInterfaces.Common.Messaging.Events;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn.Models;
using Twizzar.Design.Shared.Infrastructure;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <summary>
    /// Event publisher implemented with roslyn.
    /// </summary>
    public sealed class RoslynSolutionEvents : ISolutionEventsPublisher
    {
        #region fields

        private readonly Workspace _workspace;
        private readonly ISet<string> _currentProjectNames = new HashSet<string>();
        private readonly IUiEventHub _eventHub;
        private readonly IVsEventCacheRegistrant _eventCacheRegistrant;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynSolutionEvents"/> class.
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="eventHub"></param>
        /// <param name="eventCacheRegistrant"></param>
        public RoslynSolutionEvents(
            Workspace workspace,
            IUiEventHub eventHub,
            IVsEventCacheRegistrant eventCacheRegistrant)
        {
            this.EnsureMany()
                .Parameter(workspace, nameof(workspace))
                .Parameter(eventHub, nameof(eventHub))
                .Parameter(eventCacheRegistrant, nameof(eventCacheRegistrant))
                .ThrowWhenNull();

            this._workspace = workspace;
            this._eventHub = eventHub;
            this._eventCacheRegistrant = eventCacheRegistrant;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public void Dispose()
        {
            this._workspace.WorkspaceChanged -= this.WorkspaceOnWorkspaceChanged;
        }

        /// <inheritdoc />
        public void Initialize()
        {
            this._workspace.WorkspaceChanged += this.WorkspaceOnWorkspaceChanged;

            this._currentProjectNames.AddRange(
                this._workspace.CurrentSolution.Projects
                    .Select(project => project.GetFileNameWithoutExtension()));

            this._workspace.CurrentSolution.Projects
                .Select(GetTwizzarAnalyzer)
                .Select(GetAssemblyIdentity)
                .Somes()
                .ForEach(identity => this._eventHub.Publish(new TwizzarAnalyzerAddedEvent(identity.Version)));
        }

        private void WorkspaceOnWorkspaceChanged(object sender, WorkspaceChangeEventArgs e)
        {
            switch (e.Kind)
            {
                case WorkspaceChangeKind.ProjectChanged when e.ProjectId != null:
                    var (maybeOldProject, maybeNewProject) = ExtractProjectEventArguments(e);

                    var tuple =
                        from oldProject in maybeOldProject
                        from newProject in maybeNewProject
                        select (oldProject, newProject);

                    tuple.IfSome(x => this.ProjectChanged(x.oldProject, x.newProject));
                    break;
                case WorkspaceChangeKind.ProjectAdded when e.ProjectId != null:
                    ExtractProjectEventArguments(e).NewProject.IfSome(this.ProjectAdded);
                    break;
                case WorkspaceChangeKind.ProjectRemoved when e.ProjectId != null:
                    ExtractProjectEventArguments(e).OldProject.IfSome(this.ProjectRemoved);
                    break;
            }
        }

        private void ProjectChanged(Project oldProject, Project newProject)
        {
            var oldProjectName = oldProject.GetFileNameWithoutExtension();
            var newProjectName = newProject.GetFileNameWithoutExtension();

            if (!this._currentProjectNames.Contains(newProjectName) && oldProjectName != newProjectName)
            {
                this._currentProjectNames.Add(newProjectName);
                this._currentProjectNames.Remove(oldProjectName);

                var oldViProject = new RoslynProject(oldProject);
                var newViProject = new RoslynProject(newProject);

                this._eventCacheRegistrant.RegisterProjectUnloaded(oldViProject);
                this._eventCacheRegistrant.RegisterAllReferencesLoaded(newViProject);

                this._eventHub.Publish(
                    new ProjectRenamedEvent(oldViProject, newViProject));
            }

            var analyzer = GetTwizzarAnalyzer(newProject);

            // check if the twizzar analyzer has changed.
            if (analyzer.IsSome && GetTwizzarAnalyzer(oldProject)
                    .Map(reference => reference.Id.Equals(analyzer.GetValueUnsafe().Id))
                    .Match(b => !b, true))
            {
                GetAssemblyIdentity(analyzer)
                    .IfSome(identity => this._eventHub.Publish(new TwizzarAnalyzerAddedEvent(identity.Version)));
            }
        }

        private static (Maybe<Project> OldProject, Maybe<Project> NewProject) ExtractProjectEventArguments(
            WorkspaceChangeEventArgs e)
        {
            var oldProject = e.OldSolution.GetProject(e.ProjectId);
            var newProject = e.NewSolution.GetProject(e.ProjectId);

            return (Maybe.ToMaybe(oldProject), Maybe.ToMaybe(newProject));
        }

        private void ProjectRemoved(Project project)
        {
            this._currentProjectNames.Remove(project.GetFileNameWithoutExtension());
        }

        private void ProjectAdded(Project project)
        {
            GetAssemblyIdentity(GetTwizzarAnalyzer(project))
                .IfSome(identity => this._eventHub.Publish(new TwizzarAnalyzerAddedEvent(identity.Version)));

            this._currentProjectNames.Add(project.GetFileNameWithoutExtension());
        }

        private static Maybe<AnalyzerReference> GetTwizzarAnalyzer(Project project) =>
            project.AnalyzerReferences
                .FirstOrNone(
                    reference =>
                        reference.FullPath?.Contains(ApiNames.AnalyzerProjectNamePrefix) ?? false);

        private static Maybe<AssemblyIdentity> GetAssemblyIdentity(Maybe<AnalyzerReference> analyzer) =>
            analyzer
                .Bind(reference => Maybe.ToMaybe(reference.Id as AssemblyIdentity));

        #endregion
    }
}