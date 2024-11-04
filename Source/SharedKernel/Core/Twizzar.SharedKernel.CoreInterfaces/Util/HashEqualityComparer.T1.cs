using System;
using System.Collections.Generic;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.SharedKernel.CoreInterfaces.Util
{
    /// <summary>
    /// Compares two object over the given hash code function.
    /// </summary>
    /// <typeparam name="T">The type to compare.</typeparam>
    public class HashEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, int> _hashCodeFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashEqualityComparer{T}"/> class.
        /// </summary>
        /// <param name="hashCodeFunc"></param>
        public HashEqualityComparer(Func<T, int> hashCodeFunc)
        {
            EnsureHelper.GetDefault.Parameter(hashCodeFunc, nameof(hashCodeFunc)).ThrowWhenNull();

            this._hashCodeFunc = hashCodeFunc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashEqualityComparer{T}"/> class.
        /// </summary>
        /// <param name="hashCodeFunc"></param>
        public HashEqualityComparer(Func<T, int[]> hashCodeFunc)
        {
            EnsureHelper.GetDefault.Parameter(hashCodeFunc, nameof(hashCodeFunc)).ThrowWhenNull();

            this._hashCodeFunc = x => HashEqualityComparer.CombineHashes(hashCodeFunc(x));
        }

        /// <inheritdoc />
        public bool Equals(T x, T y) =>
            y != null && x != null && this.GetHashCode(x) == this.GetHashCode(y);

        /// <inheritdoc />
        public int GetHashCode(T obj) =>
            this._hashCodeFunc(obj);
    }
}