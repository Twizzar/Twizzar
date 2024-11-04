using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using EnvDTE;

using EnvDTE80;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Infrastructure.VisualStudio.Extensions;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Infrastructure.VisualStudio.Models;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.Util;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

using VSLangProj;

using static Microsoft.VisualStudio.VSConstants;

namespace Twizzar.Design.Infrastructure.VisualStudio.Services.VsEvents
{
    /// <inheritdoc cref="ISolutionEventsPublisher" />
    [ExcludeFromCodeCoverage] // dependent on VsSDK
    public sealed class VsSolutionEventsPublisher : IVsSolutionEvents, ISolutionEventsPublisher
    {
        #region fields

        private readonly IVsSolution _vsSolution;
        private readonly IUiEventHub _eventHub;
        private readonly IVsProjectEventPublisherFactory _projectEventPublisherFactory;
        private readonly IVsEventCacheRegistrant _eventCacheRegistrant;

        private readonly Dictionary<string, IVsProjectEventsPublisher> _projectEventPublishers =
            new();

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="VsSolutionEventsPublisher"/> class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="projectEventPublisherFactory">The project event publisher factory.</param>
        /// <param name="eventCacheRegistrant">The event cache registrant.</param>
        /// <exception cref="InternalException">The {nameof(IVsSolution)} services should be available.</exception>
        /// <exception cref="InternalException">The {nameof(DTE2)} services should be available.</exception>
        /// <exception cref="ArgumentNullException">eventHub is null.</exception>
        /// <exception cref="ArgumentNullException">projectEventPublisherFactory is null.</exception>
        /// <exception cref="COMException">Must be called on the ui thread.</exception>
        public VsSolutionEventsPublisher(
            IUiEventHub eventHub,
            IVsProjectEventPublisherFactory projectEventPublisherFactory,
            IVsEventCacheRegistrant eventCacheRegistrant)
        {
            this.EnsureMany()
                .Parameter(eventHub, nameof(eventHub))
                .Parameter(projectEventPublisherFactory, nameof(projectEventPublisherFactory))
                .Parameter(eventCacheRegistrant, nameof(eventCacheRegistrant))
                .ThrowWhenNull();

            this._eventHub = eventHub;
            this._projectEventPublisherFactory = projectEventPublisherFactory;
            this._eventCacheRegistrant = eventCacheRegistrant;

            ThreadHelper.ThrowIfNotOnUIThread();
            this._vsSolution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;

            if (this._vsSolution == null)
            {
                throw this.LogAndReturn(
                    new InternalException($"The {nameof(IVsSolution)} services should be available."));
            }
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
        public void Initialize()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (ServiceProvider.GlobalProvider.GetService(typeof(DTE)) is not DTE2 dte2)
            {
                throw this.LogAndReturn(new InternalException($"The {nameof(DTE2)} services should be available."));
            }

            foreach (var project in dte2.Solution.GetAllProjects())
            {
                this.InitializeEventPublisher(project);
            }

            this._vsSolution.AdviseSolutionEvents(this, out _);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // left empty intentionally
        }

        /// <inheritdoc />
        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var project = pHierarchy.ToDteProject();

            if (project.AsMaybeValue() is SomeValue<Project> someProject && someProject.Value.Object is VSProject)
            {
                this.InitializeEventPublisher(someProject.Value);
                this._eventHub.Publish(new AfterOpenProjectEvent(new VsProject(someProject.Value)));
            }

            return S_OK;
        }

        /// <inheritdoc />
        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel) =>
            S_OK;

        /// <inheritdoc />
        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var project = pHierarchy.ToDteProject();

            if (project.IsSome &&
                project.GetValueUnsafe().Object is VSProject &&
                this._projectEventPublishers.ContainsKey(project.GetValueUnsafe().Name))
            {
                var projectName = project.GetValueUnsafe().Name;
                this._projectEventPublishers[projectName].Dispose();
                this._projectEventPublishers.Remove(projectName);
                this._eventHub.Publish(new BeforeCloseProjectEvent(new VsProject(project.GetValueUnsafe())));
                this._eventCacheRegistrant.RegisterProjectUnloaded(new VsProject(project.GetValueUnsafe()));
            }

            return S_OK;
        }

        /// <inheritdoc />
        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy) =>
            S_OK;

        /// <inheritdoc />
        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel) =>
            S_OK;

        /// <inheritdoc />
        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy) =>
            S_OK;

        /// <inheritdoc />
        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution) =>
            S_OK;

        /// <inheritdoc />
        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel) =>
            S_OK;

        /// <inheritdoc />
        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            ViMonitor.Flush();
            this.Dispose();
            return S_OK;
        }

        /// <inheritdoc />
        public int OnAfterCloseSolution(object pUnkReserved) =>
            S_OK;

        private void InitializeEventPublisher(Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (project.Object is not VSProject vsProject)
            {
                return;
            }

            var projectEventPublisher = this._projectEventPublisherFactory.Create(vsProject);
            this._projectEventPublishers.Add(project.Name, projectEventPublisher);
            projectEventPublisher.RegisterListeners();
        }

        #endregion
    }
}