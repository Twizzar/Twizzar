using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment
{
    /// <inheritdoc />
    public class ViAdornmentCacheCreator : IViAdornmentCache
    {
        #region fields

        private readonly IViAdornmentCreator _adornmentCreator;
        private readonly ISnapshotHistory _snapshotHistory;

        private readonly SortedDictionary<SnapshotSpan, IViAdornment> _cache =
            new(Comparer<SnapshotSpan>.Create((a, b) => a.Start - b.Start));

        private ITextSnapshot _textSnapshot;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViAdornmentCacheCreator"/> class.
        /// </summary>
        /// <param name="adornmentCreator">The adornment creator.</param>
        /// <param name="snapshotHistory">The snapshot history.</param>
        public ViAdornmentCacheCreator(IViAdornmentCreator adornmentCreator, ISnapshotHistory snapshotHistory)
        {
            this.EnsureMany()
                .Parameter(snapshotHistory, nameof(snapshotHistory))
                .Parameter(adornmentCreator, nameof(adornmentCreator))
                .ThrowWhenNull();

            this._adornmentCreator = adornmentCreator;
            this._snapshotHistory = snapshotHistory;
            this._textSnapshot = null;
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
        public IEnumerable<IViAdornment> GetOrCreate(
            IAdornmentInformation[] adornmentInformation,
            IWpfTextView textView,
            IDocumentAdornmentController documentAdornmentController)
        {
            if (adornmentInformation.Length == 0)
            {
                yield break;
            }

            var currentSnapshot = this._snapshotHistory.Get(adornmentInformation.First().ObjectCreationSpan.Version)
                .SomeOrProvided(() => throw new InternalException("Version is not found in the snapshot history."));

            // Upgrade cache to the newest version
            if (currentSnapshot != this._textSnapshot)
            {
                var newCache = UpgradeCache(currentSnapshot, this._cache).ToList();
                this._cache.Clear();
                this._cache.AddRange(newCache);
                this._textSnapshot = currentSnapshot;
            }

            // all cached elements
            var cachedQueue = new Queue<KeyValuePair<SnapshotSpan, IViAdornment>>(this._cache);

            // incoming elements
            var incomingQueue = new Queue<IAdornmentInformation>(adornmentInformation);

            var newAdded = new List<IAdornmentInformation>();

            // elements not found in cache will be added to this list
            var removeFormCache = new List<KeyValuePair<SnapshotSpan, IViAdornment>>();

            while (cachedQueue.Count > 0 && incomingQueue.Count > 0)
            {
                var pair = cachedQueue.Peek();
                var information = incomingQueue.Peek();

                var span = information.ObjectCreationSpan.ToSpan();
                var snapshotSpan = new SnapshotSpan(currentSnapshot, span);

                if (pair.Key.OverlapsWith(snapshotSpan))
                {
                    cachedQueue.Dequeue();
                    incomingQueue.Dequeue();
                    var (intersectionSnapshotSpan, adornment) = pair;
                    this._cache.Remove(intersectionSnapshotSpan);
                    this._cache.Add(snapshotSpan, adornment);
                    adornment.Update(information);
                    yield return adornment;
                }
                else
                {
                    // When the cached end span is smaller than the current checked span there is no intersection possible after this point.
                    if (pair.Key.End <= snapshotSpan.Start)
                    {
                        removeFormCache.Add(cachedQueue.Dequeue());
                    }
                    else
                    {
                        newAdded.Add(incomingQueue.Dequeue());
                    }
                }
            }

            // Remove all left in the queue and in the remove list
            foreach (var (snapshotSpan, adornment) in cachedQueue.Concat(removeFormCache))
            {
                adornment.Dispose();
                this._cache.Remove(snapshotSpan);
            }

            // Create all not intersecting items
            foreach (var information in incomingQueue.Concat(newAdded))
            {
                var snapshotSpan = new SnapshotSpan(currentSnapshot, information.ObjectCreationSpan.ToSpan());
                yield return this.CreateNew(snapshotSpan, information, textView, documentAdornmentController);
            }
        }

        private IViAdornment CreateNew(
            SnapshotSpan snapshotSpan,
            IAdornmentInformation information,
            IWpfTextView textView,
            IDocumentAdornmentController documentAdornmentController)
        {
            var adornment = this._adornmentCreator.Create(information, textView, this._snapshotHistory, documentAdornmentController);
            this._cache.Add(snapshotSpan, adornment);
            return adornment;
        }

        private static IEnumerable<KeyValuePair<SnapshotSpan, IViAdornment>> UpgradeCache(
            ITextSnapshot currentSnapshot,
            IEnumerable<KeyValuePair<SnapshotSpan, IViAdornment>> oldCache)
        {
            foreach (var (span, adornment) in oldCache
                .Select(
                    pair => new KeyValuePair<SnapshotSpan, IViAdornment>(
                        pair.Key.TranslateTo(currentSnapshot, SpanTrackingMode.EdgeInclusive), pair.Value)))
            {
                if (span.IsEmpty)
                {
                    adornment.Dispose();
                }
                else
                {
                    yield return new KeyValuePair<SnapshotSpan, IViAdornment>(span, adornment);
                }
            }
        }

        #endregion
    }
}