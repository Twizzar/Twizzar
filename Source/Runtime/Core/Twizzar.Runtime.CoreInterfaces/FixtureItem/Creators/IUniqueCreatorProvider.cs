using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators
{
    /// <summary>
    /// Provider for the base type creators.
    /// </summary>
    public interface IUniqueCreatorProvider : IService
    {
        /// <summary>
        /// Gets the creator for the given type.
        /// </summary>
        /// <typeparam name="T">Specifying the type of the unique creator which will be provided.</typeparam>
        /// <returns>An instance of the <see cref="IUniqueCreator{T}"/> supporting the input type.</returns>
        IUniqueCreator<T> GetUniqueCreator<T>();
    }
}