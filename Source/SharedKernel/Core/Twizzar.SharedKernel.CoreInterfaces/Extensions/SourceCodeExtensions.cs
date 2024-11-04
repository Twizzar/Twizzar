using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// Extension methods for the source code generation.
    /// </summary>
    public static class SourceCodeExtensions
    {
        /// <summary>
        /// Convert common invalid characters like [] to valid characters for declaring code variables.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>The string with the invalid character replaced or removed.</returns>
        public static string ToSourceVariableCodeFriendly(this string self) =>
            self.Replace("[]", "Index")
                .Replace("[", "OBr")
                .Replace("]", "CBr")
                .Replace('.', '_')
                .Replace("<", string.Empty)
                .Replace(">", string.Empty)
                .Replace("`", string.Empty)
                .Replace(" ", string.Empty)
                .Replace(",", string.Empty);

        /// <summary>
        /// Make a string unique given on a set of reserved strings.
        /// </summary>
        /// <param name="self">The string itself.</param>
        /// <param name="reservedSet">All reserved strings.</param>
        /// <returns>The same string or alter with one or more underline postfix when not unique.</returns>
        public static string MakeUnique(this string self, IImmutableSet<string> reservedSet) =>
            reservedSet.Contains(self)
                ? MakeUnique($"{self}_", reservedSet)
                : self;

        /// <summary>
        /// Make a string unique given on a set of reserved strings.
        /// </summary>
        /// <param name="self">The string itself.</param>
        /// <param name="reservedSet">All reserved strings.</param>
        /// <returns>The same string or alter with one or more underline postfix when not unique.</returns>
        public static string MakeUnique(this string self, ISet<string> reservedSet) =>
            reservedSet.Contains(self)
                ? MakeUnique($"{self}_", reservedSet)
                : self;

        /// <summary>
        /// Aggregate the sequence of strings and separated the items with a new line.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>A string with all items separated by <see cref="Environment.NewLine"/>.</returns>
        public static string AggregateWithNewLine(this IEnumerable<string> self) =>
            string.Join(Environment.NewLine, self);
    }
}