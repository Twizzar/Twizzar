using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Common.FixtureItem.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;

namespace Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment
{
    /// <summary>
    /// Extension methods for the <see cref="IViSpan"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ViSpanExtensions
    {
        /// <summary>
        /// Convert a <see cref="IViSpan"/> to an <see cref="Span"/>.
        /// </summary>
        /// <param name="viSpan">The viSpan to convert.</param>
        /// <returns>A new instance of <see cref="Span"/>.</returns>
        public static Span ToSpan(this IViSpan viSpan) =>
            new(viSpan.Start, viSpan.Length);

        /// <summary>
        /// Convert <see cref="IViSpan"/> to an <see cref="SnapshotSpan"/>.
        /// </summary>
        /// <param name="viSpan">The viSpan to convert.</param>
        /// <param name="snapshotHistory">The snapshot history.</param>
        /// <returns>A new snapshot span.</returns>
        public static SnapshotSpan ToSnapshotSpan(this IViSpan viSpan, ISnapshotHistory snapshotHistory) =>
            new(
                snapshotHistory.Get(viSpan.Version).SomeOrProvided(() => throw new InternalException("Snapshot version not found")),
                viSpan.ToSpan());

        /// <summary>
        /// Convert <see cref="SnapshotSpan"/> to <see cref="IViSpan"/>.
        /// </summary>
        /// <param name="snapshotSpan"></param>
        /// <returns></returns>
        public static IViSpan ToViSpan(this SnapshotSpan snapshotSpan) =>
            new ViSpan(
                snapshotSpan.Start,
                snapshotSpan.Length,
                new ViSpanVersion(snapshotSpan.Snapshot.Version.VersionNumber));
    }
}