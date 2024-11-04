using System.Collections.Generic;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// Extension methods for converting <see cref="IEnumerable{T}"/> to string.
    /// </summary>
    public static class IEnumerableToStringExtension
    {
        /// <summary>
        /// Convert to comma separated string.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="self"></param>
        /// <returns>A comma separated string.</returns>
        public static string ToCommaSeparated<T>(this IEnumerable<T> self) => self.ToDisplayString(", ");

        /// <summary>
        /// Covert the sequence to a display string. Separated by the defined separated and surrounded by prefix and postfix.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="self"></param>
        /// <param name="separator"></param>
        /// <param name="prefix"></param>
        /// <param name="postfix"></param>
        /// <returns>A string.</returns>
        public static string ToDisplayString<T>(
            this IEnumerable<T> self,
            string separator,
            string prefix = "",
            string postfix = "") =>
                prefix +
                string.Join(separator, self) +
                postfix;
    }
}