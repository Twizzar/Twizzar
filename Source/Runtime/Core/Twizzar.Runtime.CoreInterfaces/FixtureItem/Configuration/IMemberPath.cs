using System;

using ViCommon.Functional.Monads.MaybeMonad;

#pragma warning disable S2326 // Unused type parameters should be removed
#pragma warning disable SA1502 // Element should not be on a single line

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration
{
    /// <summary>
    /// The member path uniquely identifies a Member of the FixtureItem or a child Item of the FixtureItem.
    /// </summary>
    /// <typeparam name="TFixtureItem">The FixtureItem type.</typeparam>
    public interface IMemberPath<TFixtureItem>
    {
        /// <summary>
        /// Gets the name of the path.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the unique name of the path. (For Methods the return type is post fixed with _).
        /// </summary>
        string UniqueName { get; }

        /// <summary>
        /// Gets the member name of the path.
        /// </summary>
        string MemberName { get; }

        /// <summary>
        /// Gets the name of the member, in the case of a method this will be methodName__allParamFullTypeNamesCommaSeparated.
        /// </summary>
        string UniqueMemberName { get; }

        /// <summary>
        /// Gets the return type of the member.
        /// </summary>
        Type ReturnType { get; }

        /// <summary>
        /// Gets the declared type.
        /// </summary>
        Type DeclaredType { get; }

        /// <summary>
        /// Gets the parent of the path if the path is the root then this will be None.
        /// </summary>
        Maybe<IMemberPath<TFixtureItem>> Parent { get; }
    }

    /// <inheritdoc cref="IMemberPath{TFixtureItem}" />
    public interface IMemberPath<TFixtureItem, TReturnType> : IMemberPath<TFixtureItem> { }

    /// <summary>
    /// Path segment for a constructor parameter.
    /// </summary>
    /// <typeparam name="TFixtureItem">The FixtureItem type.</typeparam>
    public interface ICtorParamMemberPath<TFixtureItem> : IMemberPath<TFixtureItem> { }

    /// <summary>
    /// Path segment for a method member.
    /// </summary>
    /// <typeparam name="TFixtureItem">The FixtureItem type.</typeparam>
    public interface IMethodMemberPath<TFixtureItem> : IMemberPath<TFixtureItem>
    {
        /// <summary>
        /// Gets the parameters to select one or many methods.
        /// When none all method overloads should be considered.
        /// </summary>
        public ITzParameter[] Parameters { get; }

        /// <summary>
        /// Gets the generic arguments of this method, if this method is not generic this will return an empty array.
        /// </summary>
        public GenericTypeArgument[] GenericArguments { get; }
    }

    /// <summary>
    /// Path segment for a property member.
    /// </summary>
    /// <typeparam name="TFixtureItem">The FixtureItem type.</typeparam>
    public interface IPropertyMemberPath<TFixtureItem> : IMemberPath<TFixtureItem> { }

    /// <summary>
    /// Path segment for a field member.
    /// </summary>
    /// <typeparam name="TFixtureItem">The FixtureItem type.</typeparam>
    public interface IFieldMemberPath<TFixtureItem> : IMemberPath<TFixtureItem> { }
}