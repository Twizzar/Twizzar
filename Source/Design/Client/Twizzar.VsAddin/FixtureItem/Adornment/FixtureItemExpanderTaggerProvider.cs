using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.IntraTextAdornment;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces.Util;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.VsAddin.Interfaces.CompositionRoot;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.VsAddin.FixtureItem.Adornment
{
    /// <summary>
    /// Create a <see cref="VsTextViewTagger"/> for tagging the text view with our FixtureItem Adornment.
    /// </summary>
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("csharp")]
    [TagType(typeof(IntraTextAdornmentTag))]
    [TextViewRole(PredefinedTextViewRoles.Analyzable)]
    [ExcludeFromCodeCoverage]
    public sealed class FixtureItemExpanderTaggerProvider : IViewTaggerProvider, IHasEnsureHelper, IHasLogger
    {
        #region fields

        // ReSharper disable UnassignedField.Local
#pragma warning disable CS0169, CS0649, IDE0044

        [Import]
        private IIocOrchestrator _iocOrchestrator;

        /// <summary>
        /// Defines the adornment layer for the adornment. This layer is ordered
        /// after the selection layer in the Z-order.
        /// </summary>
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("DefinitionUIAdornment")]
        [Order(After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text)]
        private AdornmentLayerDefinition _editorAdornmentLayer;

        [Import]
        private IPeekBroker _peekBroker = null;

#pragma warning restore

        // ReSharper restore UnassignedField.Local

        #endregion

        #region properties

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region members

        /// <summary>
        /// Creates the tagger.
        /// </summary>
        /// <typeparam name="T">Type of the tag.</typeparam>
        /// <param name="textView">The text view.</param>
        /// <param name="buffer">The buffer.</param>
        /// <returns>Microsoft.VisualStudio.Value.Tagging.ITagger&lt;T&gt;.</returns>
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer)
            where T : ITag
        {
            this.EnsureMany()
                .Parameter(textView, nameof(textView))
                .Parameter(buffer, nameof(buffer))
                .ThrowWhenNull();

            if (buffer != textView.TextBuffer)
            {
                return null;
            }

            var document = GetDocument(textView);

            return document.Match(
                textDocument => this.CreateNewTagger<T>(textView, textDocument),
                failure =>
                {
                    this.Logger?.Log(LogLevel.Error, failure.Message);
                    return null;
                });
        }

        private ITagger<T> CreateNewTagger<T>(ITextView textView, ITextDocument textDocument)
            where T : ITag
        {
            // Create new tagger
            var fixtureItemExpanderTagger =
                this.CreateTagger((IWpfTextView)textView, textDocument.FilePath);

            var renameFileEvent = CreateRenameFileEvent(fixtureItemExpanderTagger);
            var closedEvent = CreateDocumentClosedEvent(textDocument, fixtureItemExpanderTagger, renameFileEvent);

            textDocument.FileActionOccurred += renameFileEvent.Invoke;
            textView.Closed += closedEvent.Invoke;

            return (ITagger<T>)(ITagger<IntraTextAdornmentTag>)fixtureItemExpanderTagger;
        }

        private static LambdaEvent<EventArgs, ITextView> CreateDocumentClosedEvent(
            ITextDocument textDocument,
            VsTextViewTagger fixtureItemExpanderTagger,
            LambdaEvent<TextDocumentFileActionEventArgs, ITextDocument> renameFileEvent) =>
                new((sender, args, oneTimeEvent) =>
                    {
                        fixtureItemExpanderTagger?.Dispose();
                        sender.Closed -= oneTimeEvent.Invoke;
                        textDocument.FileActionOccurred -= renameFileEvent.Invoke;
                    });

        private static LambdaEvent<TextDocumentFileActionEventArgs, ITextDocument> CreateRenameFileEvent(
            VsTextViewTagger fixtureItemExpanderTagger) =>
                new((sender, args, oneTimeEvent) =>
                    {
                        if (args.FileActionType == FileActionTypes.DocumentRenamed)
                        {
                            fixtureItemExpanderTagger.Update(args.FilePath);
                        }
                    });

        private VsTextViewTagger CreateTagger(IWpfTextView textView, string documentFilePath) =>
            new(
                documentFilePath,
                this._iocOrchestrator.Resolve<IUiEventHub>(),
                this._iocOrchestrator.Resolve<IVsProjectManager>(),
                this._iocOrchestrator.Resolve<IVsEventCache>(),
                this._iocOrchestrator.Resolve<IViDocumentTaggerFactory>(),
                textView,
                this._peekBroker);

        private static Result<ITextDocument, Failure> GetDocument(ITextView wpfTextView)
        {
            var properties = wpfTextView?.TextDataModel?.DocumentBuffer?.Properties;

            if (properties == null)
            {
                return new Failure($"Document properties is null");
            }

            if (properties.TryGetProperty(
                typeof(ITextDocument),
                out ITextDocument document))
            {
                if (document.TextBuffer != null)
                {
                    return Result.Success(document);
                }
                else
                {
                    return new Failure("TextBuffer is null");
                }
            }
            else
            {
                return new Failure($"Document has no Property {nameof(ITextDocument)}");
            }
        }

        #endregion
    }
}