using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace AcceptanceCriteriaTests.Description
{
    /// <summary>
    /// ConcreteType Description value object.
    /// </summary>
    internal interface ITypeDescription 
    {

        #region properties

        public IList<int> GetIntList();

        public IList<int> GetList { get; }

        /// <summary>
        /// Gets the given BaseType fullname. Can be null, for example for interfaces.
        /// </summary>
        public ITypeFullName BaseType { get; }

        /// <summary>
        /// Gets a list of the implemented interfaces.
        /// </summary>
        public ImmutableArray<string> ImplementedInterfaceNames { get; }

        /// <summary>
        /// Gets a value indicating whether the ConcreteType is a delegate.
        /// </summary>
        public bool IsDelegate { get; }

        /// <summary>
        /// Gets a value indicating whether the ConcreteType is abstract and must be overridden.
        /// </summary>
        public bool IsAbstract { get; }

        /// <summary>
        /// Gets a value indicating whether the current ConcreteType object represents a type whose definition is nested
        /// inside the definition of another type.
        /// </summary>
        /// <remarks>
        /// Some information about nested and access modifier (internal, protected, ...) can be found:
        /// https://stackoverflow.com/questions/4971213/how-to-use-reflection-to-determine-if-a-class-is-internal.
        /// </remarks>
        public bool IsNested { get; }

        /// <summary>
        /// Gets a value indicating whether the ConcreteType is declared sealed.
        /// </summary>
        public bool IsSealed { get; }

        /// <summary>
        /// Gets a value indicating whether the ConcreteType is declared static.
        /// </summary>
        public bool IsStatic { get; }

        /// <summary>
        /// Gets a value indicating whether the type contains generic parameters.
        /// </summary>
        public bool IsGeneric { get; }

        /// <summary>
        /// Gets a value indicating whether the type is inherited from <see cref="ICollection"/>.
        /// </summary>
        public bool IsInheritedFromICollection { get; }



        #endregion

        #region members

        /// <summary>
        /// Gets a list of the constructor descriptions.
        /// <remarks>This call is lazy to prevent recursion. When first called the result will be cached.</remarks>
        /// </summary>
        /// <returns>An immutable array.</returns>
        public ImmutableArray<IMethodDescription> GetDeclaredConstructors();

        /// <summary>
        /// Gets a list of the field descriptions.
        /// <remarks>This call is lazy to prevent recursion. When first called the result will be cached.</remarks>
        /// </summary>
        /// <returns>An immutable array.</returns>
        public ImmutableArray<IFieldDescription> GetDeclaredFields();

        /// <summary>
        /// Gets a list of the method descriptions.
        /// <remarks>This call is lazy to prevent recursion. When first called the result will be cached.</remarks>
        /// </summary>
        /// <returns>An immutable array.</returns>
        public ImmutableArray<IMethodDescription> GetDeclaredMethods();

        /// <summary>
        /// Gets a list of the constructor descriptions.
        /// <remarks>This call is lazy to prevent recursion. When first called the result will be cached.</remarks>
        /// </summary>
        /// <returns>An immutable array.</returns>
        public ImmutableArray<IPropertyDescription> GetDeclaredProperties();

        /// <summary>
        /// Gets the generic argument as an <see cref="ITypeDescription"/> at the given position.
        /// </summary>
        /// <param name="position">The argument position.</param>
        /// <returns>The type description.</returns>
        public ITypeDescription GetGenericTypeArgumentDescription(int position);

        #endregion
    }
}