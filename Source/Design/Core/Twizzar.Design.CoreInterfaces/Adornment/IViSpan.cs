namespace Twizzar.Design.CoreInterfaces.Adornment
{
    /// <summary>
    /// Interface to abstract a text span.
    /// </summary>
    public interface IViSpan
    {
        /// <summary>
        /// Gets the start of the span.
        /// </summary>
        int Start { get; }

        /// <summary>
        /// Gets the length of the span.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets the version of the span.
        /// </summary>
        IViSpanVersion Version { get; }

        /// <summary>
        /// Create a new span with another version.
        /// </summary>
        /// <param name="version">The new version.</param>
        /// <returns>A new vi span.</returns>
        IViSpan WithVersion(IViSpanVersion version);
    }
}