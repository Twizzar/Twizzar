using System.Diagnostics.CodeAnalysis;

using Autofac;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Common.FixtureItem.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.SharedKernel.Factories;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.VsAddin.Factory
{
    /// <inheritdoc cref="IDocumentWorkspaceFactory" />
    [ExcludeFromCodeCoverage]
    public class DocumentWorkspaceFactory : FactoryBase, IDocumentWorkspaceFactory
    {
        #region fields

        private readonly IRoslynDocumentReaderFactory _roslynDocumentReaderFactory;
        private readonly SnapshotHistoryFactory _snapshotHistoryFactory;
        private readonly AdornmentCreatorFactory _adornmentCreatorFactory;
        private readonly AdornmentCacheFactory _adornmentCacheFactory;
        private readonly DocumentFactory _documentFactory;
        private readonly DocumentWriterFactory _documentWriterFactory;
        private readonly DocumentAdornmentControllerFactory _documentAdornmentController;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentWorkspaceFactory"/> class.
        /// </summary>
        /// <param name="componentContext"></param>
        /// <param name="roslynDocumentReaderFactory"></param>
        /// <param name="snapshotHistoryFactory"></param>
        /// <param name="adornmentCreatorFactory"></param>
        /// <param name="adornmentCacheFactory"></param>
        /// <param name="documentFactory"></param>
        /// <param name="documentWriterFactory"></param>
        /// <param name="documentAdornmentController"></param>
        public DocumentWorkspaceFactory(
            IComponentContext componentContext,
            IRoslynDocumentReaderFactory roslynDocumentReaderFactory,
            SnapshotHistoryFactory snapshotHistoryFactory,
            AdornmentCreatorFactory adornmentCreatorFactory,
            AdornmentCacheFactory adornmentCacheFactory,
            DocumentFactory documentFactory,
            DocumentWriterFactory documentWriterFactory,
            DocumentAdornmentControllerFactory documentAdornmentController)
            : base(componentContext)
        {
            this.EnsureMany()
                .Parameter(roslynDocumentReaderFactory, nameof(roslynDocumentReaderFactory))
                .Parameter(snapshotHistoryFactory, nameof(snapshotHistoryFactory))
                .Parameter(adornmentCreatorFactory, nameof(adornmentCreatorFactory))
                .Parameter(adornmentCacheFactory, nameof(adornmentCacheFactory))
                .Parameter(documentFactory, nameof(documentFactory))
                .Parameter(documentAdornmentController, nameof(documentAdornmentController))
                .ThrowWhenNull();

            this._roslynDocumentReaderFactory = roslynDocumentReaderFactory;
            this._snapshotHistoryFactory = snapshotHistoryFactory;
            this._adornmentCreatorFactory = adornmentCreatorFactory;
            this._adornmentCacheFactory = adornmentCacheFactory;
            this._documentFactory = documentFactory;
            this._documentWriterFactory = documentWriterFactory;
            this._documentAdornmentController = documentAdornmentController;
        }

        #endregion

        #region delegates

        /// <summary>
        /// Factory injected by Autofac.
        /// </summary>
        /// <param name="adornmentCreator"></param>
        /// <param name="snapshotHistory"></param>
        /// <returns>A new instance of <see cref="IViAdornmentCache"/>.</returns>
        public delegate IViAdornmentCache AdornmentCacheFactory(
            IViAdornmentCreator adornmentCreator,
            ISnapshotHistory snapshotHistory);

        /// <summary>
        /// Factory injected by Autofac.
        /// </summary>
        /// <param name="snapshotHistory"></param>
        /// <param name="documentWriter"></param>
        /// <returns>A new instance of <see cref="IViAdornmentCreator"/>.</returns>
        public delegate IViAdornmentCreator AdornmentCreatorFactory(
            ISnapshotHistory snapshotHistory,
            IDocumentWriter documentWriter);

        /// <summary>
        /// Factory injected by Autofac.
        /// </summary>
        /// <param name="documentReader"></param>
        /// <param name="documentWriter"></param>
        /// <param name="viAdornmentCache"></param>
        /// <param name="snapshotHistory"></param>
        /// <param name="documentAdornmentController"></param>
        /// <returns>A new instance of <see cref="IDocumentWorkspace"/>.</returns>
        public delegate IDocumentWorkspace DocumentFactory(
            IDocumentReader documentReader,
            IDocumentWriter documentWriter,
            IViAdornmentCache viAdornmentCache,
            ISnapshotHistory snapshotHistory,
            IDocumentAdornmentController documentAdornmentController);

        /// <summary>
        /// Factory injected by Autofac.
        /// </summary>
        /// <returns>A new instance of <see cref="ISnapshotHistory"/>.</returns>
        public delegate ISnapshotHistory SnapshotHistoryFactory();

        /// <summary>
        /// Factory injected by Autofac.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>A new instance of <see cref="IDocumentWriter"/>.</returns>
        public delegate IDocumentWriter DocumentWriterFactory(string filePath);

        /// <summary>
        /// Factory injected by Autofac.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="view"></param>
        /// <param name="peekBroker"></param>
        /// <param name="documentWriter"></param>
        /// <returns>A new instance of <see cref="IDocumentAdornmentController"/>.</returns>
        public delegate IDocumentAdornmentController DocumentAdornmentControllerFactory(
            string projectName,
            IWpfTextView view,
            IPeekBroker peekBroker,
            IDocumentWriter documentWriter);

        #endregion

        #region members

        /// <inheritdoc />
        public IDocumentWorkspace Create(string projectName, string documentFilePath, IPeekBroker peekBroker, IWpfTextView wpfTextView)
        {
            var snapshotHistory = this._snapshotHistoryFactory();
            var documentWriter = this._documentWriterFactory(documentFilePath);

            var adornmentCreator = this._adornmentCreatorFactory(snapshotHistory, documentWriter);
            var documentAdornmentController = this._documentAdornmentController(projectName, wpfTextView, peekBroker, documentWriter);

            return this._documentFactory(
                this._roslynDocumentReaderFactory.CreateViContainerReader(documentFilePath, projectName),
                documentWriter,
                this._adornmentCacheFactory(adornmentCreator, snapshotHistory),
                snapshotHistory,
                documentAdornmentController);
        }

        #endregion
    }
}