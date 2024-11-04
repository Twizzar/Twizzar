using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// Structural equal extension methods.
    /// </summary>
    public static class StructuralExtensions
    {
        /// <summary>
        /// Is a structural equal to b.
        /// </summary>
        /// <param name="a">Object a.</param>
        /// <param name="b">Object b.</param>
        /// <typeparam name="T">Type of the objects.</typeparam>
        /// <returns>True if structural equal else false.</returns>
        public static bool StructuralEquals<T>(this T a, T b)
            where T : IStructuralEquatable =>
            a.Equals(b, StructuralComparisons.StructuralEqualityComparer);

        /// <summary>
        /// Check if all key are equals and the corresponding values are also equal.
        /// </summary>
        /// <typeparam name="TKey">Type of the dictionary keys.</typeparam>
        /// <typeparam name="TValue">Type of the dictionary values.</typeparam>
        /// <param name="a">Dictionary a.</param>
        /// <param name="b">Dictionary b.</param>
        /// <param name="valueComparer">Value comparer for comparing the values.</param>
        /// <returns>True when the dicts are structural equal.</returns>
        public static bool StructuralEquals<TKey, TValue>(
            this IImmutableDictionary<TKey, TValue> a,
            IImmutableDictionary<TKey, TValue> b,
            IEqualityComparer<TValue> valueComparer = null)
        {
            if (a == b)
                return true;
            if (a == null || b == null)
                return false;
            if (a.Count != b.Count)
                return false;

            valueComparer ??= EqualityComparer<TValue>.Default;

            foreach (var key in a.Keys)
            {
                if (b.ContainsKey(key))
                {
                    if (!valueComparer.Equals(a[key], b[key]))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compare the structure of two objects.
        /// </summary>
        /// <param name="a">Object a.</param>
        /// <param name="b">Object b.</param>
        /// <typeparam name="T">Type of the objects.</typeparam>
        /// <returns>1 if a is structural bigger than b, 0 if they are structural equal else -1.</returns>
        public static int StructuralCompare<T>(this T a, T b)
            where T : IStructuralComparable =>
            a.CompareTo(b, StructuralComparisons.StructuralComparer);

        /// <summary>
        /// Get the structural hash code.
        /// </summary>
        /// <typeparam name="T">Type of a.</typeparam>
        /// <param name="a">Object which implements <see cref="IStructuralEquatable"/>.</param>
        /// <returns>A hash code.</returns>
        public static int StructuralHashCode<T>(this T a)
            where T : IStructuralEquatable =>
            a.GetHashCode(StructuralComparisons.StructuralEqualityComparer);

        /// <summary>
        /// Get the structural hash code for a dictionary.
        /// </summary>
        /// <typeparam name="TKey">Type of the dictionary keys.</typeparam>
        /// <typeparam name="TValue">Type of the dictionary values.</typeparam>
        /// <param name="a">The dictionary.</param>
        /// <returns>The hashcode.</returns>
        public static int StructuralHashCode<TKey, TValue>(this IImmutableDictionary<TKey, TValue> a)
        {
            EnsureHelper.GetDefault.Parameter(a, nameof(a)).ThrowWhenNull();
            unchecked
            {
                int hashcode = 1430287;
                hashcode = hashcode * 7302013 ^ a.Keys.GetHashCodeOfElements();
                hashcode = hashcode * 7302013 ^ a.Values.GetHashCodeOfElements();
                return hashcode;
            }
        }
    }
}
