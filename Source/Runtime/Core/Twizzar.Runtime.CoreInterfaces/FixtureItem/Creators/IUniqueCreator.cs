using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators
{
    /// <summary>
    /// Interface for the unique value generator.
    /// </summary>
    /// <typeparam name="T">The type of the values which will be created.</typeparam>
    public interface IUniqueCreator<out T> : IService
    {
        /// <summary>
        /// Get next value according to the specified algorithm.
        /// </summary>
        /// <returns>The next value as of the specified generator.</returns>
        T GetNextValue();
    }
}
