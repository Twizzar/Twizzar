using System.Collections.Immutable;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Base values for any description.
    /// </summary>
    public interface IBaseDescription
    {
        #region properties

        /// <summary>
        /// Gets the type fullname.
        /// </summary>
        public ITypeFullName TypeFullName { get; }

        /// <summary>
        /// Gets the access modifier of the type.
        /// </summary>
        public AccessModifier AccessModifier { get; }

        /// <summary>
        /// Gets a value indicating whether the ConcreteType is a class.
        /// </summary>
        public bool IsClass { get; }

        /// <summary>
        /// Gets a value indicating whether the ConcreteType is an interface.
        /// </summary>
        public bool IsInterface { get; }

        /// <summary>
        /// Gets a value indicating whether it is a enum type.
        /// </summary>
        public bool IsEnum { get; }

        /// <summary>
        /// Gets a value indicating whether it is a struct type.
        /// </summary>
        public bool IsStruct { get; }

        /// <summary>
        /// Gets a value indicating whether it is a array type.
        /// </summary>
        public bool IsArray { get; }

        /// <summary>
        /// Gets the array dimension.
        /// </summary>
        ImmutableArray<int> ArrayDimension { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a base type.
        /// </summary>
        public bool IsBaseType { get; }

        /// <summary>
        /// Gets a value indicating whether it is a nullable base type.
        /// </summary>
        public bool IsNullableBaseType { get; }

        /// <summary>
        /// Gets a value indicating whether this type is a type parameter in a generic type or generic method.
        /// </summary>
        public bool IsTypeParameter { get; }

        #endregion

        #region members

        /// <summary>
        /// Gets the return type description.
        /// This is
        /// * a description of the returned type for properties and methods.
        /// * a description of the filed type for fields.
        /// * a description of the parameter type for parameters.
        /// * the same as this for a type description.
        /// <remarks>This call is lazy to prevent recursion. When first called the result will be cached.</remarks>
        /// </summary>
        /// <returns>A new constructed type description.</returns>
        public ITypeDescription GetReturnTypeDescription();

        /// <summary>
        /// Gets the enum values if <see cref="IsEnum"/> else None.
        /// </summary>
        /// <returns>All enum values.</returns>
        public Maybe<string[]> GetEnumNames();

        /// <summary>
        /// Gets the friendly type full name.
        /// </summary>
        /// <returns>A string.</returns>
        public string GetFriendlyReturnTypeFullName();

        #endregion
    }
}