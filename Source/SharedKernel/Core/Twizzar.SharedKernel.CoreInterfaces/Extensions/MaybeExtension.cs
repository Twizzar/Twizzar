using System;
using System.Collections.Generic;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Maybe"/>.
    /// </summary>
    public static class MaybeExtension
    {
        /// <summary>
        /// Convert the <see cref="Maybe{TValue}"/> to a <see cref="Result{TValue, TFailure}"/>
        /// where TFailure = <see cref="Failure"/>.
        /// </summary>
        /// <typeparam name="T">The maybe value type.</typeparam>
        /// <param name="self"></param>
        /// <param name="message">The message to use for the <see cref="Failure"/>.</param>
        /// <returns>A new <see cref="Result{TValue, Failure}"/>.</returns>
        public static IResult<T, Failure> ToResult<T>(this Maybe<T> self, string message) =>
            self.ToResult(new Failure(message));

        /// <summary>
        /// Get some value of a dictionary by key if key exists else none.
        /// </summary>
        /// <typeparam name="TValue">Type of the dictionary values.</typeparam>
        /// <typeparam name="TKey">Type of the dictionary keys.</typeparam>
        /// <param name="self">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>Maybe a value.</returns>
        public static Maybe<TValue> GetValueMaybe<TValue, TKey>(
            this IDictionary<TKey, TValue> self,
            TKey key)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));
            return !self.ContainsKey(key) ? Maybe.None() : Maybe.Some(self[key]);
        }
    }
}
