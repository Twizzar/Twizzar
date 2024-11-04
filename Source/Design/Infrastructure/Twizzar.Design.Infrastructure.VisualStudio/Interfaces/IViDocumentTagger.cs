using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.IntraTextAdornment;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces
{
    /// <summary>
    /// This Tagger creates tags for a specific document. This class lives as long the <see cref="VsTextViewTagger"/> lives.
    /// And as long the document name does not change.
    /// </summary>
    public interface IViDocumentTagger : IDisposable
    {
        #region properties

        /// <summary>
        /// Gets a value indicating whether the tagger was disposed.
        /// </summary>
        public bool IsDisposes { get; }

        #endregion

        #region members

        /// <summary>
        /// Get spans affected by the <see cref="AdornmentSizeChangedEvent"/>.
        /// </summary>
        /// <param name="e">The event.</param>
        /// <returns>A <see cref="SnapshotSpan"/> which includes the affected area.</returns>
        Maybe<SnapshotSpan> GetAffectedSpan(AdornmentSizeChangedEvent e);

        /// <summary>
        /// Gets a span for the whole document.
        /// </summary>
        /// <returns>A <see cref="SnapshotSpan"/>.</returns>
        SnapshotSpan GetDocumentSpan();

        /// <summary>
        /// Gets the tags for a specific region.
        /// </summary>
        /// <param name="spans">The span collection is used to determine the region.</param>
        /// <returns>A sequence of many <see cref="ITagSpan{T}"/>.</returns>
        IEnumerable<ITagSpan<IntraTextAdornmentTag>> GetTags(NormalizedSnapshotSpanCollection spans);

        #endregion
    }
}