using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Infrastructure.VisualStudio.Extensions;
using Twizzar.Design.Infrastructure.VisualStudio.Models;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

using VSLangProj;

namespace Twizzar.Design.Infrastructure.VisualStudio.Services.VsEvents
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage] // dependent on VsSDK
    public sealed class VsProjectEventsPublisher : IVsProjectEventsPublisher
    {
        #region fields

        private readonly VSProject _project;
        private readonly IUiEventHub _eventHub;
        private readonly IVsEventCacheRegistrant _eventCacheRegistrant;
        private int _referenceCount;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="VsProjectEventsPublisher"/> class.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="eventCacheRegistrant">The event cache registrant.</param>
        public VsProjectEventsPublisher(VSProject project, IUiEventHub eventHub, IVsEventCacheRegistrant eventCacheRegistrant)
        {
            this.EnsureMany()
                .Parameter(project, nameof(project))
                .Parameter(eventHub, nameof(eventHub))
                .ThrowWhenNull();

            this._project = project;
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

        /// <summary>
        /// Registers the listeners.
        /// </summary>
        public void RegisterListeners()
        {
            this.RegisterProject();

            // When already all references are loaded.
            if (this._project.References.SafeCast<Reference>().Any(reference => reference.Path != null))
            {
                this.OnAllReferencesLoaded();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.UnRegisterProject();
        }

        private void RegisterProject()
        {
            this._project.Events.ReferencesEvents.ReferenceAdded += this.OnReferenceAdded;
        }

        private void UnRegisterProject()
        {
            try
            {
                if (this._project?.Events?.ReferencesEvents != null)
                {
                    this._project.Events.ReferencesEvents.ReferenceAdded -= this.OnReferenceAdded;
                }
            }
            catch (Exception)
            {
                // do nothing this happens on projects with the old project structure.
            }
        }

        private void OnReferenceAdded(Reference reference)
        {
            if (this._referenceCount <= this._project.References.Count)
            {
                this._referenceCount++;

                if (this._referenceCount == this._project.References.Count)
                {
                    this.OnAllReferencesLoaded();
                }
            }
            else
            {
                this._eventHub.Publish(
                    new ReferenceAddedEvent(new ViReference(reference), new VsProject(this._project.Project)));
            }
        }

        private void OnAllReferencesLoaded()
        {
            var viProject = new VsProject(this._project.Project);
            this._eventCacheRegistrant.RegisterAllReferencesLoaded(viProject);
            this._eventHub.Publish(new ProjectReferencesLoadedEvent(viProject));
        }

        #endregion
    }
}