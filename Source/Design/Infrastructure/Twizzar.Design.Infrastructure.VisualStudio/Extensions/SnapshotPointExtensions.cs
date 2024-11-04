using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Text;

namespace Twizzar.Design.Infrastructure.VisualStudio.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="SnapshotPoint"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class SnapshotPointExtensions
    {
        /// <summary>
        /// Get the line number.
        /// </summary>
        /// <param name="snapshotPoint">The snapshot point.</param>
        /// <returns>The line number.</returns>
        public static int GetLineNumber(this SnapshotPoint snapshotPoint) =>
            snapshotPoint.Snapshot.GetLineNumberFromPosition(snapshotPoint);
    }
}