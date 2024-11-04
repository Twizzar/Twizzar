using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualStudio.Text.Editor;

using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment
{
    /// <inheritdoc />
    public class ViAdornmentCreator : IViAdornmentCreator
    {
        #region fields

        private readonly ViAdornmentFactory _viAdornmentViAdornmentFactory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViAdornmentCreator"/> class.
        /// </summary>
        /// <param name="viAdornmentViAdornmentFactory">The vi adornment factory.</param>
        public ViAdornmentCreator(ViAdornmentFactory viAdornmentViAdornmentFactory)
        {
            this.EnsureMany()
                .Parameter(viAdornmentViAdornmentFactory, nameof(viAdornmentViAdornmentFactory))
                .ThrowWhenNull();

            this._viAdornmentViAdornmentFactory = viAdornmentViAdornmentFactory;
        }

        #endregion

        #region delegates

        /// <summary>
        /// Factory for autofac.
        /// </summary>
        /// <param name="adornmentInformation"></param>
        /// <param name="snapshotHistory"></param>
        /// <param name="textView"></param>
        /// <param name="documentAdornmentController"></param>
        /// <returns>A new instance of <see cref="IViAdornment"/>.</returns>
        public delegate IViAdornment ViAdornmentFactory(
            IAdornmentInformation adornmentInformation,
            ISnapshotHistory snapshotHistory,
            IWpfTextView textView,
            IDocumentAdornmentController documentAdornmentController);

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public IViAdornment Create(
            IAdornmentInformation adornmentInformation,
            IWpfTextView textView,
            ISnapshotHistory snapshotHistory,
            IDocumentAdornmentController documentAdornmentController) =>
            this._viAdornmentViAdornmentFactory(
                adornmentInformation,
                snapshotHistory,
                textView,
                documentAdornmentController);

        #endregion
    }
}