using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Twizzar.Design.CoreInterfaces.Common.Messaging.Events;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.IntraTextAdornment
{
    /// <summary>
    /// Class DefinitionUIExpanderTagger. This class cannot be inherited.
    /// </summary>
    public sealed class VsTextViewTagger :
        ITagger<IntraTextAdornmentTag>,
        IHasEnsureHelper,
        IHasLogger,
        IDisposable
    {
        #region fields

        private readonly IUiEventHub _eventHub;
        private readonly IViDocumentTaggerFactory _documentTaggerFactory;
        private readonly IWpfTextView _textView;
        private readonly IPeekBroker _peekBroker;
        private string _projectName;
        private string _documentFilePath;

        private bool _taggerIsReady;

        private IViDocumentTagger _viDocumentTagger;
        private bool _isDisposed;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="VsTextViewTagger" /> class.
        /// </summary>
        /// <param name="documentFilePath">The file path to the document.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="vsProjectManager">The project manager.</param>
        /// <param name="eventCache">The event cache.</param>
        /// <param name="documentTaggerFactory"></param>
        /// <param name="textView"></param>
        /// <param name="peekBroker"></param>
        /// <exception cref="ArgumentNullException">view.</exception>
        /// <exception cref="ArgumentNullException">getInstanceTagAggregatorHelper.</exception>
        /// <exception cref="ArgumentNullException">definitionUiExpanderCache.</exception>
        /// <exception cref="ArgumentNullException">definitionUiExpanderCachePositionUpdater.</exception>
        public VsTextViewTagger(
            string documentFilePath,
            IUiEventHub eventHub,
            IVsProjectManager vsProjectManager,
            IVsEventCache eventCache,
            IViDocumentTaggerFactory documentTaggerFactory,
            IWpfTextView textView,
            IPeekBroker peekBroker)
        {
            this.EnsureParameter(documentFilePath, nameof(documentFilePath))
                .IsNotNullAndNotEmpty()
                .ThrowOnFailure();

            this.EnsureMany()
                .Parameter(eventHub, nameof(eventHub))
                .Parameter(vsProjectManager, nameof(vsProjectManager))
                .Parameter(eventCache, nameof(eventCache))
                .Parameter(documentTaggerFactory, nameof(documentTaggerFactory))
                .Parameter(textView, nameof(textView))
                .Parameter(peekBroker, nameof(peekBroker))
                .ThrowWhenNull();

            this._eventHub = eventHub;
            this._documentTaggerFactory = documentTaggerFactory;
            this._textView = textView;
            this._peekBroker = peekBroker;

            if (vsProjectManager.FindProjectName(documentFilePath).AsMaybeValue() is SomeValue<string> projectName)
            {
                this._projectName = projectName;
                this._taggerIsReady = eventCache.AllReferencesAreLoaded(projectName);
                this._documentFilePath = documentFilePath;

                this._viDocumentTagger = this._documentTaggerFactory.Create(textView, peekBroker, documentFilePath, this._projectName);
                eventHub.Subscribe<AdornmentSizeChangedEvent>(this, this.AdornmentSizeChanged);
                eventHub.Subscribe<ProjectReferencesLoadedEvent>(this, this.AllProjectReferenceAreLoaded);
                eventHub.Subscribe<ProjectRenamedEvent>(this, this.ProjectRenamed);
                eventHub.Subscribe<DocumentTagsUpdatedEvent>(this, this.UpdateAllTags);
            }
            else
            {
                this.Dispose();
            }
        }

        #endregion

        #region events

        /// <summary>
        /// Occurs when tags changed. This event will be send to VisualStudio and VisualStudio will trigger <see cref="GetTags"/> again.
        /// </summary>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        #endregion

        #region properties

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public void Dispose()
        {
            if (this._isDisposed)
            {
                return;
            }

            this._isDisposed = true;
            this._viDocumentTagger?.Dispose();
            this._viDocumentTagger = null;
            this._eventHub.Unsubscribe<AdornmentSizeChangedEvent>(this, this.AdornmentSizeChanged);
            this._eventHub.Unsubscribe<ProjectReferencesLoadedEvent>(this, this.AllProjectReferenceAreLoaded);
            this._eventHub.Unsubscribe<ProjectRenamedEvent>(this, this.ProjectRenamed);
            this._eventHub.Unsubscribe<DocumentTagsUpdatedEvent>(this, this.UpdateAllTags);
        }

        /// <summary>
        /// Updates the tagger, when source file is changed.
        /// </summary>
        /// <param name="documentFilePath"></param>
        public void Update(string documentFilePath)
        {
            this.Update(documentFilePath, this._projectName);
        }

        /// <summary>
        /// Provides the tags for the specified spans. This will be called by VS when something has changed in the text view (text, scrolling, etc).
        /// </summary>
        /// <param name="spans">The spans.</param>
        /// <returns>List of tags which matches the spans.</returns>
        public IEnumerable<ITagSpan<IntraTextAdornmentTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (this._isDisposed || spans == null || spans.Count == 0 || this._viDocumentTagger is { IsDisposes: true } || !this._taggerIsReady)
            {
                return Enumerable.Empty<ITagSpan<IntraTextAdornmentTag>>();
            }

            return this._viDocumentTagger.GetTags(spans);
        }

        private void ProjectRenamed(ProjectRenamedEvent e)
        {
            this.Update(this._documentFilePath, e.NewProject.Name);
        }

        private void Update(string documentFilePath, string projectName)
        {
            this.EnsureMany<string>()
                .Parameter(documentFilePath, nameof(documentFilePath))
                .Parameter(projectName, nameof(projectName))
                .IsNotNullAndNotEmpty()
                .ThrowOnFailure();

            if (this._isDisposed)
            {
                return;
            }

            this._documentFilePath = documentFilePath;
            this._projectName = projectName;
            this._viDocumentTagger?.Dispose();
            this._viDocumentTagger = this._documentTaggerFactory.Create(this._textView, this._peekBroker, documentFilePath, projectName);
            this.UpdateAllTags();
        }

        /// <summary>
        /// When all projects are loaded invoke <see cref="TagsChanged"/> for the hole document, so we can create the adornments.
        /// Before all references are loaded its not possible for uss to build a semantic model with roslyn.
        /// </summary>
        /// <param name="obj"></param>
        private void AllProjectReferenceAreLoaded(ProjectReferencesLoadedEvent obj)
        {
            if (this._isDisposed)
            {
                return;
            }

            if (obj.Project.Name.Trim() == this._projectName.Trim())
            {
                this._taggerIsReady = true;
                this.UpdateAllTags();
            }
        }

        private void UpdateAllTags(DocumentTagsUpdatedEvent e)
        {
            if (this._isDisposed)
            {
                return;
            }

            if (e.DocumentFilePath.Trim() == this._documentFilePath)
            {
                this._taggerIsReady = true;
                this.UpdateAllTags();
            }
        }

        private void UpdateAllTags()
        {
            if (this._viDocumentTagger is { IsDisposes: false })
            {
                this.OnTagsChanged(this._viDocumentTagger.GetDocumentSpan());
            }
        }

        /// <summary>
        /// When the size of a adornment is changed we need to change the reserved space of the IntraTextAdornmentTag.
        /// </summary>
        /// <param name="e"></param>
        private void AdornmentSizeChanged(AdornmentSizeChangedEvent e)
        {
            if (this._isDisposed)
            {
                return;
            }

            this._viDocumentTagger?.GetAffectedSpan(e)
                .IfSome(this.OnTagsChanged);
        }

        private void OnTagsChanged(SnapshotSpan span)
        {
            if (this._isDisposed)
            {
                return;
            }

            this.TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
        }

        #endregion
    }
}