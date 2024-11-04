using System;
using System.Collections.Generic;
using System.Linq;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}"/> which uses a <see cref="Maybe"/> monad.
    /// </summary>
    public static class IEnumerableMaybeExtensions
    {
        /// <summary>Returns the first element of the sequence that satisfies a condition or <c>None</c> if no such element is found.</summary>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
        /// <returns>
        /// <c>None</c> if <paramref name="source" /> is empty or if no element passes the test specified by <paramref name="predicate" />; otherwise, the first element in <paramref name="source" /> that passes the test specified by <paramref name="predicate" />.
        /// </returns>
        public static Maybe<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(source, nameof(source))
                .Parameter(predicate, nameof(predicate))
                .ThrowWhenNull();

            using var enumerator = source.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (predicate(enumerator.Current))
                {
                    return enumerator.Current;
                }
            }

            return Maybe.None();
        }

        /// <summary>Returns the first element of the sequence that satisfies a condition or <c>None</c> if no such element is found.</summary>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return an element from.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source" /> is <see langword="null" />.</exception>
        /// <returns>
        /// <c>None</c> if <paramref name="source" /> is empty otherwise, the first element in <paramref name="source" />.
        /// </returns>
        public static Maybe<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source) =>
            FirstOrNone(source, _ => true);

        /// <summary>
        /// Gets the item at the index.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <returns>Gets the item at the index if the list capacity is big enough else None.</returns>
        public static Maybe<TSource> IndexOrNone<TSource>(this IList<TSource> source, int index) =>
            source.Count > index
                ? source[index]
                : Maybe.None();

        /// <summary>Returns the last element of the sequence that satisfies a condition or <c>None</c> if no such element is found.</summary>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
        /// <returns>
        /// <c>None</c> if <paramref name="source" /> is empty or if no element passes the test specified by <paramref name="predicate" />; otherwise, the last element in <paramref name="source" /> that passes the test specified by <paramref name="predicate" />.
        /// </returns>
        public static Maybe<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(source, nameof(source))
                .Parameter(predicate, nameof(predicate))
                .ThrowWhenNull();

            var result = source.Where(predicate).ToList();
            return result.Any() ? result.Last() : Maybe.None();
        }

        /// <summary>Returns the last element of the sequence that satisfies a condition or <c>None</c> if no such element is found.</summary>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return an element from.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source" /> is <see langword="null" />.</exception>
        /// <returns>
        /// <c>None</c> if <paramref name="source" /> is empty otherwise, the last element in <paramref name="source" />.
        /// </returns>
        public static Maybe<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source) =>
            LastOrNone(source, _ => true);

        /// <summary>
        /// Returns the only element of a sequence, or None if the sequence is empty or more than one item is in the sequence.
        /// </summary>
        /// <param name="source">An &lt;see cref="T:System.Collections.Generic.IEnumerable`1" /&gt; to return the single element of.</param>
        /// <typeparam name="TSource">The type of the elements of &lt;paramref name="source" /&gt;.</typeparam>
        /// <returns>The single element of the input sequence, or <c>Maybe.None()</c> if the sequence contains no elements.</returns>
        public static Maybe<TSource> SingleOrNone<TSource>(this IEnumerable<TSource> source)
        {
            var array = source.ToArray();
            return array switch
            {
                { Length: 1 } => array[0],
                _ => Maybe.None<TSource>(),
            };
        }
    }
}