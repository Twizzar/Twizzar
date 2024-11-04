using System;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services
{
    /// <summary>
    /// Interface for resolving a type in to an instance or value.
    /// </summary>
    public interface IResolver : IService
    {
        /// <summary>
        /// Retrieve a instance from the context.
        /// </summary>
        /// <typeparam name="T">Type of the instance.</typeparam>
        /// <returns> The instance that provides the type.</returns>
        T Resolve<T>();

        /// <summary>
        /// Retrieve a instance from the type.
        /// </summary>
        /// <param name="type">The type to lookup.</param>
        /// <returns> The instance that provides the type.</returns>
        object Resolve(Type type);

        /// <summary>
        /// Retrieve a named instance from the context.
        /// </summary>
        /// <typeparam name="T">Type of the instance.</typeparam>
        /// <param name="typeName">The type name. </param>
        /// <returns>The instance that provides the type.</returns>
        /// <exception cref="ArgumentNullException">When the typeName is empty or null.</exception>
        T ResolveNamed<T>(string typeName);
    }
}