using System;
using System.Collections.Generic;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// An extension class for ICollection.
    /// </summary>
    public static class ICollectionExtensions
    {
        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The collection the items add to.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="items">The items who should be added to the end of the <see cref="ICollection{T}"></see>. The items itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        /// <exception cref="ArgumentNullException">The collection ist null.</exception>
        /// <exception cref="ArgumentNullException">The items collection is null.</exception>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (collection is List<T> asList)
            {
                asList.AddRange(items);
            }
            else
            {
                foreach (var item in items)
                {
                    collection.Add(item);
                }
            }
        }

        /// <summary>
        /// Adds the item to the collection, if it is not already in the collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if the item is added and it was not already in the collection, <c>false</c> otherwise.</returns>
        public static bool AddIfNotExists<T>(this ICollection<T> collection, T item)
        {
            if (collection?.Contains(item) == false)
            {
                collection.Add(item);
                return true;
            }

            return false;
        }
    }
}
