using System;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators
{
    /// <summary>
    /// Interface for the unique value generator of the base Types.
    /// </summary>
    public interface IBaseTypeUniqueCreator : IService
    {
        /// <summary>
        /// Get next value according to the specified algorithm.
        /// </summary>
        /// <param name="type">The given type of the generator next value.</param>
        /// <returns>The next value as of the specified generator.</returns>
        object GetNextValue(Type type);
    }
}