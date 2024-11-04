using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualStudio.Text;

using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment
{
    /// <inheritdoc />
    public class SnapshotHistory : ISnapshotHistory
    {
        #region fields

        private readonly Dictionary<IViSpanVersion, ITextSnapshot> _historyAccess =
            new();

        private readonly Queue<IViSpanVersion> _historyQueue = new();

        #endregion

        #region properties

        /// <inheritdoc />
        public ITextSnapshot CurrentSnapshot { get; private set; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public int Count => this._historyAccess.Count;

        #endregion

        #region members

        /// <inheritdoc />
        public void Add(ITextSnapshot snapshot)
        {
            this.EnsureParameter(snapshot, nameof(snapshot)).ThrowWhenNull();

            var version = new ViSpanVersion(snapshot.Version.VersionNumber);
            this._historyAccess.AddOrUpdate(new ViSpanVersion(snapshot.Version.VersionNumber), snapshot);
            this._historyQueue.Enqueue(version);
            this.CurrentSnapshot = snapshot;

            // keep the history small
            if (this._historyQueue.Count > 100)
            {
                this._historyAccess.Remove(this._historyQueue.Dequeue());
            }
        }

        /// <inheritdoc />
        public Maybe<ITextSnapshot> Get(IViSpanVersion version) =>
            this._historyAccess.GetMaybe(version);

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public IViSpan UpdateSpan(IViSpan span)
        {
            var snapShotSpan = span.ToSnapshotSpan(this);
            return snapShotSpan.TranslateTo(this.CurrentSnapshot, SpanTrackingMode.EdgeExclusive)
                .ToViSpan();
        }

        #endregion
    }
}