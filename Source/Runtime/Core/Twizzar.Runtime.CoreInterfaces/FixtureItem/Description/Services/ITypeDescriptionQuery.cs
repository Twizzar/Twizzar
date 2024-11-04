using System;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Description.Services
{
    /// <summary>
    /// Query for getting a <see cref="IRuntimeTypeDescription"/>.
    /// </summary>
    public interface ITypeDescriptionQuery : IQuery
    {
        /// <summary>
        /// Gets a <see cref="ITypeDescription"/> for a certain type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A new <see cref="ITypeDescription"/>.</returns>
        IRuntimeTypeDescription GetTypeDescription(Type type);

        /// <summary>
        /// Gets a <see cref="ITypeDescription"/> for a certain type.
        /// </summary>
        /// <param name="typeFullName">The type full name.</param>
        /// <returns>A new <see cref="ITypeDescription"/>.</returns>
        IRuntimeTypeDescription GetTypeDescription(ITypeFullName typeFullName);
    }
}
