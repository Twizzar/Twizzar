using System.Collections.Generic;
using System.Linq;

namespace Twizzar.SharedKernel.CoreInterfaces.Util
{
    /// <summary>
    /// Helper methods for the <see cref="HashEqualityComparer{T}"/>.
    /// </summary>
    public static class HashEqualityComparer
    {
        /// <summary>
        /// Combine many hashes to one.
        /// </summary>
        /// <param name="hashCodes"></param>
        /// <returns></returns>
        public static int CombineHashes(params int[] hashCodes) =>
            CombineHashes((IEnumerable<int>)hashCodes);

        /// <summary>
        /// Combine many hashes to one.
        /// </summary>
        /// <param name="hashCodes"></param>
        /// <returns></returns>
        public static int CombineHashes(IEnumerable<int> hashCodes) =>
            hashCodes.Aggregate(17, (aggregate, hash) =>
            {
                unchecked
                {
                    return (aggregate * 23) + hash;
                }
            });
    }
}