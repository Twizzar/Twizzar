using Microsoft.CodeAnalysis.Text;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Common.FixtureItem.Adornment;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <summary>
    /// Extension Methods for <see cref="TextSpan"/>.
    /// </summary>
    public static class SpanExtensions
    {
        /// <summary>
        /// Converts to viSpan.
        /// </summary>
        /// <param name="textSpan">The text span.</param>
        /// <returns><see cref="IViSpan"/>.</returns>
        public static IViSpan ConvertToViSpan(this TextSpan textSpan)
        {
            var viSpan = new ViSpan(textSpan.Start, textSpan.Length);
            return viSpan;
        }

        /// <summary>
        /// Converts to viSpan.
        /// </summary>
        /// <param name="viSpan"><see cref="IViSpan"/>.</param>
        /// <returns>The text span.</returns>
        public static TextSpan ToTextSpan(this IViSpan viSpan)
        {
            var textSpan = new TextSpan(viSpan.Start, viSpan.Length);
            return textSpan;
        }
    }
}