using System.Collections.Immutable;
using Twizzar.SharedKernel.CoreInterfaces.Util;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IImmutableDictionary{TKey,TValue}"/>.
    /// </summary>
    public static class ImmutableDictionaryExtension
    {
        /// <summary>
        /// Updates all items in a with b.
        /// If the item key exists in a and b then:
        ///     If item a and item b implements <see cref="IMergeable"/> invoke a.Merge(b).
        ///     Else Overwrite item a with b.
        /// If the item key only exists in b it will be ignored.
        /// If the item key only exists in a leave it as it is.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="a">First dictionary.</param>
        /// <param name="b">Second dictionary.</param>
        /// <returns>A new dictionary with all key of a and b.</returns>
        public static IImmutableDictionary<TKey, TValue> UpdateWith<TKey, TValue>(
            this IImmutableDictionary<TKey, TValue> a,
            IImmutableDictionary<TKey, TValue> b)
        {
            EnsureHelper.GetDefault.Parameter(b, nameof(b)).ThrowWhenNull();

            var builder = ImmutableDictionary.CreateBuilder<TKey, TValue>();
            builder.AddRange(a);
            foreach (var bKey in b.Keys)
            {
                if (builder.ContainsKey(bKey))
                {
                    if (builder[bKey] is IMergeable aMergeable && b[bKey] is IMergeable bMergeable)
                    {
                        builder[bKey] = (TValue)aMergeable.Merge(bMergeable);
                    }
                    else
                    {
                        builder[bKey] = b[bKey];
                    }
                }
                else
                {
                    // Ignore the entry
                }
            }

            return builder.ToImmutable();
        }

        /// <summary>
        /// Adds or updates a value in a dictionary.
        /// If the key does not exists in the dictionary it will be added.
        /// If the key exists and it implements <see cref="IMergeable"/> the on the existing item <see cref="IMergeable.Merge"/> will be called with the new value;
        /// else the value in the dictionary will be overwritten.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="self"></param>
        /// <param name="key">The key.</param>
        /// <param name="value">The new value.</param>
        /// <returns>A new dictionary with the updated value.</returns>
        public static IImmutableDictionary<TKey, TValue> AddOrUpdate<TKey, TValue>(this IImmutableDictionary<TKey, TValue> self, TKey key, TValue value)
        {
            if (self.ContainsKey(key))
            {
                if (self[key] is IMergeable aMergeable && value is IMergeable bMergeable)
                {
                    return self.SetItem(key, (TValue)aMergeable.Merge(bMergeable));
                }
                else
                {
                    return self.SetItem(key, value);
                }
            }
            else
            {
                return self.Add(key, value);
            }
        }

        /// <summary>
        /// Merges dictionary a with dictionary b.
        /// When both have the same key take the value form b.
        /// When both a and b implement <see cref="IMergeable"/> a will not be overwritten by b, <see cref="IMergeable.Merge"/> will be called.
        /// B ∪ (A \ B).
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>A new dictionary with.</returns>
        public static IImmutableDictionary<TKey, TValue> Merge<TKey, TValue>(
            this IImmutableDictionary<TKey, TValue> a,
            IImmutableDictionary<TKey, TValue> b)
        {
            EnsureHelper.GetDefault.Parameter(a, nameof(a)).ThrowWhenNull();
            EnsureHelper.GetDefault.Parameter(b, nameof(b)).ThrowWhenNull();

            var builder = ImmutableDictionary.CreateBuilder<TKey, TValue>();
            builder.AddRange(a);
            foreach (var bKey in b.Keys)
            {
                if (builder.ContainsKey(bKey))
                {
                    if (builder[bKey] is IMergeable aMergeable && b[bKey] is IMergeable bMergeable)
                    {
                        builder[bKey] = (TValue)aMergeable.Merge(bMergeable);
                    }
                    else
                    {
                        builder[bKey] = b[bKey];
                    }
                }
                else
                {
                    builder.Add(bKey, b[bKey]);
                }
            }

            return builder.ToImmutable();
        }

        /// <summary>
        /// Add or override an entry with the given key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="self">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to update or add.</param>
        /// <returns>A new immutable array if the value does not exist or is different, else returns self.</returns>
        public static IImmutableDictionary<TKey, TValue> AddOrOverride<TKey, TValue>(
            this IImmutableDictionary<TKey, TValue> self,
            TKey key,
            TValue value)
        {
            EnsureHelper.GetDefault.Parameter(self, nameof(self)).ThrowWhenNull();
            return self.ContainsKey(key) ? self.SetItem(key, value) : self.Add(key, value);
        }
    }
}
