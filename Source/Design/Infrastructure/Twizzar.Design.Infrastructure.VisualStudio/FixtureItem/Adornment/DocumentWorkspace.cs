using System.Collections.Generic;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment
{
    /// <inheritdoc cref="IDocumentWorkspace" />
    public class DocumentWorkspace : ValueObject, IDocumentWorkspace
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentWorkspace"/> class.
        /// </summary>
        /// <param name="documentReader">The document reader.</param>
        /// <param name="documentWriter">The document writer.</param>
        /// <param name="viAdornmentCache">The adornment cache.</param>
        /// <param name="snapshotHistory"></param>
        /// <param name="documentAdornmentController"></param>
        public DocumentWorkspace(
            IDocumentReader documentReader,
            IDocumentWriter documentWriter,
            IViAdornmentCache viAdornmentCache,
            ISnapshotHistory snapshotHistory,
            IDocumentAdornmentController documentAdornmentController)
        {
            this.EnsureMany()
                .Parameter(documentReader, nameof(documentReader))
                .Parameter(documentWriter, nameof(documentWriter))
                .Parameter(viAdornmentCache, nameof(viAdornmentCache))
                .Parameter(snapshotHistory, nameof(snapshotHistory))
                .Parameter(documentAdornmentController, nameof(documentAdornmentController))
                .ThrowWhenNull();

            this.DocumentReader = documentReader;
            this.DocumentWriter = documentWriter;
            this.ViAdornmentCache = viAdornmentCache;
            this.SnapshotHistory = snapshotHistory;
            this.DocumentAdornmentController = documentAdornmentController;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public IDocumentAdornmentController DocumentAdornmentController { get; }

        /// <inheritdoc />
        public IDocumentReader DocumentReader { get; }

        /// <inheritdoc />
        public IDocumentWriter DocumentWriter { get; }

        /// <inheritdoc />
        public IViAdornmentCache ViAdornmentCache { get; }

        /// <inheritdoc />
        public ISnapshotHistory SnapshotHistory { get; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Overrides of ValueObject

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.DocumentAdornmentController;
            yield return this.DocumentReader;
            yield return this.DocumentWriter;
            yield return this.SnapshotHistory;
            yield return this.ViAdornmentCache;
        }

        #endregion
    }
}