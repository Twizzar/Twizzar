using System.Collections.Generic;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// Collection of KeyValuePair extension methods.
    /// </summary>
    public static class KeyValuePairExtensions
    {
        /// <summary>
        /// Deconstruct key value pair to easily use in for loops.
        /// </summary>
        /// <typeparam name="T1">Type parameter of the key element.</typeparam>
        /// <typeparam name="T2">Type parameter of the value element.</typeparam>
        /// <param name="tuple">The keyValue instance.</param>
        /// <param name="key">The key element.</param>
        /// <param name="value">The value element.</param>
        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
        {
            key = tuple.Key;
            value = tuple.Value;
        }
    }
}
