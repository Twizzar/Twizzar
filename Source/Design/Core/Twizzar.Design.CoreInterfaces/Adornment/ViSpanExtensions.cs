namespace Twizzar.Design.CoreInterfaces.Adornment
{
    /// <summary>
    /// Extension methods for <see cref="IViSpan"/>.
    /// </summary>
    public static class ViSpanExtensions
    {
        /// <summary>
        /// Gets the end of the span. (start + length).
        /// </summary>
        /// <param name="span">>The span.</param>
        /// <returns>The position at the end of the span.</returns>
        public static int End(this IViSpan span) =>
            span.Start + span.Length;
    }
}