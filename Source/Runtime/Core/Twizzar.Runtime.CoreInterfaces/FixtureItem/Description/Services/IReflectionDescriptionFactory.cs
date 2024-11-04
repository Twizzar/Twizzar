using System;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Description.Services
{
    /// <summary>
    /// Factory for creating a <see cref="IRuntimeTypeDescription"/>.
    /// </summary>
    public interface IReflectionDescriptionFactory
    {
        /// <summary>
        /// Create a description form a type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>A new <see cref="IRuntimeTypeDescription"/>.</returns>
        IRuntimeTypeDescription Create(Type type);
    }
}