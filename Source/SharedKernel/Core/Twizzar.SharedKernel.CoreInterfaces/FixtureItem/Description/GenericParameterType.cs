using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Data class for generic parameter type.
    /// </summary>
    /// <param name="TypeFullName">
    /// Gets the type full name. Is Some when <see cref="IsClosedConstructed"/> else None.
    /// </param>
    /// <param name="Name">Gets the generic parameter name. For example T.</param>
    /// <param name="Position">Gets the position of the type parameter.</param>
    /// <param name="Constrains"></param>
    /// <param name="HasNotNullConstraint"></param>
    /// <param name="HasConstructorConstraint"></param>
    /// <param name="HasReferenceTypeConstraint"></param>
    /// <param name="HasUnmanagedTypeConstraint"></param>
    /// <param name="HasValueTypeConstraint"></param>
    /// <param name="IsClosedConstructed">
    /// Gets a value indicating whether the parameter close constructed. Which means all type variables are set to a specific type.
    /// </param>
    public record struct GenericParameterType(
        Maybe<ITypeFullName> TypeFullName,
        string Name,
        int Position,
        ImmutableArray<ITypeFullName> Constrains,
        bool HasNotNullConstraint,
        bool HasConstructorConstraint,
        bool HasReferenceTypeConstraint,
        bool HasUnmanagedTypeConstraint,
        bool HasValueTypeConstraint,
        bool IsClosedConstructed)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericParameterType"/> class.
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="constrains"></param>
        /// <param name="hasNotNullConstraint"></param>
        /// <param name="hasConstructorConstraint"></param>
        /// <param name="hasReferenceTypeConstraint"></param>
        /// <param name="hasUnmanagedTypeConstraint"></param>
        /// <param name="hasValueTypeConstraint"></param>
        public GenericParameterType(
            Maybe<ITypeFullName> typeFullName,
            string name,
            int position,
            ImmutableArray<ITypeFullName> constrains,
            bool hasNotNullConstraint = false,
            bool hasConstructorConstraint = false,
            bool hasReferenceTypeConstraint = false,
            bool hasUnmanagedTypeConstraint = false,
            bool hasValueTypeConstraint = false)
            : this(
                typeFullName,
                name,
                position,
                constrains,
                hasNotNullConstraint,
                hasConstructorConstraint,
                hasReferenceTypeConstraint,
                hasUnmanagedTypeConstraint,
                hasValueTypeConstraint,
                typeFullName.IsSome)
        {
        }

        /// <summary>
        /// Get all constrains as string.
        /// This will also returns constrains like struct, class or notnull.
        /// </summary>
        /// <param name="getTypeFullName">Delegate for getting the type full name.</param>
        /// <returns></returns>
        public IEnumerable<string> GetAllConstrainsAsString(Func<ITypeFullName, string> getTypeFullName)
        {
            if (this.HasUnmanagedTypeConstraint)
            {
                yield return "unmanaged";
            }

            if (this.HasValueTypeConstraint)
            {
                yield return "struct";
            }

            if (this.HasNotNullConstraint)
            {
                yield return "notnull";
            }

            if (this.HasConstructorConstraint)
            {
                yield return "new()";
            }

            if (this.HasReferenceTypeConstraint)
            {
                yield return "class";
            }

            foreach (var typeFullName in this.Constrains)
            {
                yield return getTypeFullName(typeFullName);
            }
        }
    }
}