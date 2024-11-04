using System;
using System.Collections.Generic;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services
{
    /// <summary>
    /// The base type service interface.
    /// </summary>
    public interface IBaseTypeService : IService
    {
        #region properties

        /// <summary>
        /// Gets the registered base types.
        /// </summary>
        IEnumerable<Type> BaseTypes { get; }

        #endregion

        #region members

        /// <summary>
        /// Determines whether the given type full name is
        /// a supported base type or not.
        /// </summary>
        /// <param name="typeFullName">The type full name which will be checked.</param>
        /// <returns><c>true</c> if the type full name is a supported base type.</returns>
        bool IsBaseType(ITypeFullName typeFullName);

        /// <summary>
        /// Checks whether the given base type is a nullable base type.
        /// </summary>
        /// <param name="typeFullName">The type full name which will be checked.</param>
        /// <returns><c>true</c> if the type description represents a nullable type of any supported base types.</returns>
        bool IsNullableBaseType(ITypeFullName typeFullName);

        /// <summary>
        /// Get the kind of the base type.
        /// </summary>
        /// <param name="baseDescription">The description of the type.</param>
        /// <returns>The <see cref="BaseTypeKind"/>.</returns>
        BaseTypeKind GetKind(IBaseDescription baseDescription);

        #endregion
    }
}