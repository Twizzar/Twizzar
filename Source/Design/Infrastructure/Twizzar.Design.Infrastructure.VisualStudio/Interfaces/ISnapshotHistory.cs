using Microsoft.VisualStudio.Text;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.SharedKernel.CoreInterfaces;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces
{
    /// <summary>
    /// A history of the <see cref="ITextSnapshot"/>.
    /// </summary>
    public interface ISnapshotHistory : IService
    {
        #region properties

        /// <summary>
        /// Gets the current snapshot of the newest version.
        /// </summary>
        public ITextSnapshot CurrentSnapshot { get; }

        /// <summary>
        /// Gets the amount of snapshot versions stored.
        /// </summary>
        public int Count { get; }

        #endregion

        #region members

        /// <summary>
        /// Add a new entry to the history.
        /// </summary>
        /// <param name="snapshot">The text snapshot.</param>
        void Add(ITextSnapshot snapshot);

        /// <summary>
        /// Get the snapshot of a specific version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>The snapshot at the given version.</returns>
        Maybe<ITextSnapshot> Get(IViSpanVersion version);

        /// <summary>
        /// Update the span.
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        IViSpan UpdateSpan(IViSpan span);

        #endregion
    }
}