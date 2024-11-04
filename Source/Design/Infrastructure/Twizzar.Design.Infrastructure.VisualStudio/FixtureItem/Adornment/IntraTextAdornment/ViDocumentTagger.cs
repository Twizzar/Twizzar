using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Threading;

using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Infrastructure.VisualStudio.Extensions;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.Util;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

using Task = System.Threading.Tasks.Task;

#pragma warning disable SA1312 // Variable names should begin with lower-case letter

namespace Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.IntraTextAdornment
{
    /// <inheritdoc cref="IViDocumentTagger" />
    public sealed class ViDocumentTagger :
        IHasEnsureHelper,
        IHasLogger,
        IViDocumentTagger
    {
        #region fields

        private readonly IDocumentWorkspace _documentWorkspace;
        private readonly Workspace _workspace;
        private readonly IUiEventHub _eventHub;

        /// <summary>
        /// SortedDictionary uses more memory than SortedList but has a complexity of O(log n) for insertion and removal instead of O(n) for SortedList.
        /// A search cost us a complexity of O(log n). But we don't need to search.
        /// A IntervalTree would be the best data structure to use. But our list will never be really big therefore this is sufficient.
        /// </summary>
        private readonly SortedDictionary<SnapshotSpan, IViAdornment> _cache =
            new(Comparer<SnapshotSpan>.Create((a, b) => a.Start - b.Start)); // sort ASC

        private readonly string _documentFilePath;

        private Maybe<DocumentId> _documentId;

        private IWpfTextView _view;
        private int _adornmentInformationHash = -1;
        private CancellationTokenSource _cancellationTokenRoslynSource = new();
        private CancellationTokenSource _cancellationTokenGetTagsSource = new();
        private ITextVersion _cacheVersion;
        private bool _hasTwizzarAnalyzer = false;
        private int _objectCreatedHash = -1;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViDocumentTagger" /> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="documentFilePath">The file path to the document.</param>
        /// <param name="projectName"></param>
        /// <param name="documentWorkspaceFactory">The factory for creating the <see cref="IDocumentWorkspace"/>.</param>
        /// <param name="peekBroker"></param>
        /// <param name="workspace"></param>
        /// <param name="eventHub"></param>
        /// <exception cref="ArgumentNullException">view.</exception>
        /// <exception cref="ArgumentNullException">getInstanceTagAggregatorHelper.</exception>
        /// <exception cref="ArgumentNullException">definitionUiExpanderCache.</exception>
        /// <exception cref="ArgumentNullException">definitionUiExpanderCachePositionUpdater.</exception>
        public ViDocumentTagger(
            IWpfTextView view,
            string documentFilePath,
            string projectName,
            IDocumentWorkspaceFactory documentWorkspaceFactory,
            IPeekBroker peekBroker,
            Workspace workspace,
            IUiEventHub eventHub)
        {
            this.EnsureMany()
                .Parameter(view, nameof(view))
                .Parameter(documentWorkspaceFactory, nameof(documentWorkspaceFactory))
                .Parameter(peekBroker, nameof(peekBroker))
                .Parameter(workspace, nameof(workspace))
                .Parameter(eventHub, nameof(eventHub))
                .ThrowWhenNull();

            this.EnsureMany<string>()
                .Parameter(documentFilePath, nameof(documentFilePath))
                .Parameter(projectName, nameof(projectName))
                .IsNotNullAndNotEmpty()
                .ThrowOnFailure();

            this._workspace = workspace;
            this._view = view;
            this._documentFilePath = documentFilePath;
            this._eventHub = eventHub;

            this._documentWorkspace = documentWorkspaceFactory.Create(
                projectName,
                documentFilePath,
                peekBroker,
                view);

            workspace.WorkspaceChanged += this.OnWorkspaceChanged;

            Task.Run(this.InitializeAsync).FireAndForget();
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public bool IsDisposes { get; private set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public void Dispose()
        {
            this.IsDisposes = true;

            if (this._workspace != null)
            {
                this._workspace.WorkspaceChanged -= this.OnWorkspaceChanged;
            }

            this._documentWorkspace?.DocumentAdornmentController?.Dispose();
            this._cache?.Clear();
            this._cancellationTokenRoslynSource?.Dispose();
            this._cancellationTokenGetTagsSource?.Dispose();
            this._view = null;
        }

        /// <summary>
        /// Provides the tags for the specified spans. This will be called by VS when something has changed in the text view (text, scrolling, etc).
        /// </summary>
        /// <param name="spans">The spans.</param>
        /// <returns>List of tags which matches the spans.</returns>
        public IEnumerable<ITagSpan<IntraTextAdornmentTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            try
            {
                return this.GetTagsUnsafe(spans);
            }
            catch (OperationCanceledException)
            {
                return Enumerable.Empty<ITagSpan<IntraTextAdornmentTag>>();
            }
        }

        /// <inheritdoc />
        public Maybe<SnapshotSpan> GetAffectedSpan(AdornmentSizeChangedEvent e) =>
            this._cache.Any(pair => pair.Value.Id == e.AdornmentId)
                ? e.AdornmentInformation.ObjectCreationSpan.ToSnapshotSpan(this._documentWorkspace.SnapshotHistory)
                : Maybe.None<SnapshotSpan>();

        /// <inheritdoc />
        public SnapshotSpan GetDocumentSpan() =>
            new(this._view.TextSnapshot, new Span(0, this._view.TextSnapshot.Length));

        private async Task InitializeAsync()
        {
            var documentId = Maybe.None<DocumentId>();
            var maxIterations = (2 * 60) / 2; // 2 min
            var i = 0;

            while (documentId.IsNone && !this.IsDisposes)
            {
                documentId = this._workspace.CurrentSolution.GetDocumentIdsWithFilePath(this._documentFilePath)
                    .FirstOrNone();

                await Task.Delay(TimeSpan.FromSeconds(2));
                i++;

                if (i >= maxIterations)
                {
                    this.Dispose();
                    return;
                }
            }

            if (this.IsDisposes)
            {
                return;
            }

            var hasTwizzarAnalyzer = false;

            i = 0;

            while (!hasTwizzarAnalyzer)
            {
                var document = this._workspace.CurrentSolution.GetDocument(documentId.GetValueUnsafe());
                hasTwizzarAnalyzer = document?.Project.HasTwizzarAnalyzer() ?? false;
                this._documentId = documentId;
                await Task.Delay(TimeSpan.FromSeconds(2));
                i++;

                if (i >= maxIterations)
                {
                    this.Dispose();
                    return;
                }
            }

            this._hasTwizzarAnalyzer = hasTwizzarAnalyzer;
            await this.HandleDocumentChangedAsync();
        }

        private IEnumerable<ITagSpan<IntraTextAdornmentTag>> GetTagsUnsafe(NormalizedSnapshotSpanCollection spans)
        {
            if (this._documentWorkspace == null || this.IsDisposes)
            {
                yield break;
            }

            using var methodOperation = ViMonitor.StartOperation(nameof(this.GetTagsUnsafe));
            methodOperation.Telemetry.Properties["spans"] = spans.ToString();

            this._cancellationTokenGetTagsSource?.Cancel();
            this._cancellationTokenGetTagsSource = new CancellationTokenSource();

            var cancellationToken =
                this._cancellationTokenGetTagsSource.Token.CombineWith(
                        this._cancellationTokenRoslynSource.Token)
                    .Token;

            // the tags are ordered therefore take all takes which are in the spans interval
            foreach (var (snapshotSpan, adornment) in this._cache
                         .Select(pair =>
                             new KeyValuePair<SnapshotSpan, IViAdornment>(
                                 pair.Key.TranslateTo(this._view.TextSnapshot, SpanTrackingMode.EdgeExclusive),
                                 pair.Value))
                         .SkipWhile(pair => spans.First().Start.Position >= pair.Key.End.Position)
                         .TakeWhile(pair => spans.Last().End.Position >= pair.Key.Start.Position))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var newObjectCreationSpan =
                    this._documentWorkspace.SnapshotHistory.UpdateSpan(
                        adornment.AdornmentInformation.ObjectCreationSpan);

                var newObjectCreationTypeSpan = this._documentWorkspace.SnapshotHistory.UpdateSpan(
                    adornment.AdornmentInformation.ObjectCreationTypeSpan);

                adornment.Update(
                    adornment.AdornmentInformation.UpdateSpans(newObjectCreationSpan, newObjectCreationTypeSpan));

                // the IntraTextAdornmentTag will provide space in the text view for displaying a ui element.
                var tag = new IntraTextAdornmentTag(adornment.AdornmentExpander.UiElement, null);

                yield return new TagSpan<IntraTextAdornmentTag>(snapshotSpan, tag);
            }
        }

        private void OnWorkspaceChanged(object sender, WorkspaceChangeEventArgs e) =>
            Task.Run(() => this.OnWorkspaceChangedAsync(e));

        private async Task OnWorkspaceChangedAsync(WorkspaceChangeEventArgs e)
        {
            try
            {
                if (!this.IsDisposes && this._documentId.AsMaybeValue() is SomeValue<DocumentId> someDocumentId)
                {
                    if (e.ProjectId == someDocumentId.Value.ProjectId && e.Kind == WorkspaceChangeKind.ProjectChanged)
                    {
                        await this.HandleProjectChangedAsync(e);
                    }

                    if (e.DocumentId == someDocumentId.Value && e.Kind == WorkspaceChangeKind.DocumentChanged)
                    {
                        await this.HandleDocumentChangedAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Log(ex);
            }
        }

        private async Task HandleDocumentChangedAsync()
        {
            try
            {
                if (!this._hasTwizzarAnalyzer)
                {
                    return;
                }

                this._cancellationTokenRoslynSource?.Cancel();
                this._cancellationTokenRoslynSource = new CancellationTokenSource();
                var token = this._cancellationTokenRoslynSource.Token;

                try
                {
                    if (await this.UpdateCacheAsync(this._view.TextSnapshot, token) && !token.IsCancellationRequested)
                    {
                        this._eventHub.Publish(new DocumentTagsUpdatedEvent(this._documentFilePath));
                    }
                }
                catch (OperationCanceledException)
                {
                    // ignored
                }
                catch (Exception exp)
                {
                    this.Log(exp);
                }
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
        }

        private async Task HandleProjectChangedAsync(WorkspaceChangeEventArgs e)
        {
            try
            {
                var project = e.NewSolution.GetProject(e.ProjectId);

                var hasTwizzarAnalyzer = project.HasTwizzarAnalyzer();

                var hasChanged = this._hasTwizzarAnalyzer != hasTwizzarAnalyzer;
                this._hasTwizzarAnalyzer = hasTwizzarAnalyzer;

                if (hasChanged)
                {
                    this._cache.Clear();
                    this._objectCreatedHash = -1;

                    if (hasTwizzarAnalyzer)
                    {
                        await this.UpdateCacheAsync(this._view.TextSnapshot, this._cancellationTokenRoslynSource.Token);
                    }

                    this._eventHub.Publish(new DocumentTagsUpdatedEvent(this._documentFilePath));
                }
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
            catch (OperationCanceledException)
            {
                // ignored
            }
        }

        /// <summary>
        /// Update the cache.
        /// </summary>
        private async Task<bool> UpdateCacheAsync(ITextSnapshot snapshot, CancellationToken cancellationToken)
        {
            if (this.IsDisposes)
            {
                return false;
            }

            using var _ = ViMonitor.StartOperation($"{nameof(ViDocumentTagger)}.{nameof(this.UpdateCacheAsync)}");

            var document = snapshot.GetOpenDocumentInCurrentContextWithChanges();

            if (document is null)
            {
                this.Log($"Cannot get document form snapshot in file: {this._documentFilePath}");
                return false;
            }

            var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);

            if (syntaxTree is null)
            {
                this.Log($"Cannot get the syntax tree form the document: {this._documentFilePath}");
                return false;
            }

            var root = await syntaxTree.GetRootAsync(cancellationToken);

            var objectCreatedHash = root.DescendantNodes()
                .OfType<ObjectCreationExpressionSyntax>()
                .Where(syntax => syntax.Type is NameSyntax)
                .Select(syntax => syntax.ToString())
                .GetHashCodeOfElements();

            if (objectCreatedHash == this._objectCreatedHash)
            {
                return false;
            }

            // check if the spans are on a never version
            if (snapshot.Version != this._cacheVersion)
            {
                this._documentWorkspace.SnapshotHistory.Add(snapshot);
                this._cacheVersion = snapshot.Version;
            }

            var compilation = await document.Project.GetCompilationAsync(cancellationToken);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var context = new RoslynContext(semanticModel, document, root, compilation);

            // get all the adornment information for the document.
            // and add the current version to it
            var adornmentInformation = this._documentWorkspace.DocumentReader
                .GetAdornmentInformation(context, cancellationToken)
                .Select(
                    information => information.UpdateVersion(new ViSpanVersion(snapshot.Version.VersionNumber)))
                .ToArray();

            var newHash = adornmentInformation
                .OrderBy(information => information.ObjectCreationSpan.Start)
                .GetHashCodeOfElements();

            // check if the information has changed
            if (newHash == this._adornmentInformationHash)
            {
                return false;
            }

            this._cache.Clear();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            foreach (var viAdornment in this._documentWorkspace.ViAdornmentCache.GetOrCreate(
                         adornmentInformation,
                         this._view,
                         this._documentWorkspace.DocumentAdornmentController))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var invocationSpan = viAdornment.AdornmentInformation.ObjectCreationTypeSpan;

                // grab the space behind the invocation for placing the adornment there.
                var snapshotSpan = new SnapshotSpan(
                    snapshot,
                    new Span(invocationSpan.Start + invocationSpan.Length, 0));

                this._cache.AddOrUpdate(snapshotSpan, viAdornment);
            }

            // update the hashes at the end so the hashes only get updated when the operation was not canceled.
            this._adornmentInformationHash = newHash;
            this._objectCreatedHash = objectCreatedHash;
            return true;
        }

        #endregion
    }
}