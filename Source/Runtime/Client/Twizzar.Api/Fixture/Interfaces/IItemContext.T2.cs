using System;
using Moq;
using Twizzar.Fixture.Member;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture
{
    /// <summary>
    /// The item scope contains information gathered while creating the fixture item.
    /// </summary>
    /// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
    /// <typeparam name="TPathProvider">The type of the path provider.</typeparam>
    public interface IItemContext<TFixtureItem, TPathProvider> : IPathSelectionProvider<TFixtureItem, TPathProvider>
        where TPathProvider : new()
    {
        /// <summary>
        /// Get a configured member or dependencies of the fixture item.
        /// </summary>
        /// <typeparam name="TReturnType">The return type.</typeparam>
        /// <param name="selector">Function for selecting a member.</param>
        /// <returns>The instance of the requested member or dependencies.</returns>
        TReturnType Get<TReturnType>(Func<TPathProvider, MemberPath<TFixtureItem, TReturnType>> selector);

        /// <summary>
        /// Gets a configured member or dependencies of the fixture item as an <see cref="Mock{T}"/>.
        /// </summary>
        /// <typeparam name="TReturnType">The return type.</typeparam>
        /// <param name="selector">Function for selecting a member.</param>
        /// <returns>A mock of the instance of the requested member or dependencies.</returns>
        Mock<TReturnType> GetAsMoq<TReturnType>(Func<TPathProvider, MemberPath<TFixtureItem, TReturnType>> selector)
            where TReturnType : class;

        /// <summary>
        /// Start the verification of a method.
        ///
        /// <example>
        /// <code>
        /// var sut = new ItemBuilder&lt;IMyType&gt;()
        ///     .Build(out var scope);
        ///
        /// sut.MyMethod();
        ///
        /// scope.Verify(p => p.MyMethod)
        ///     .Called();
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="TMethodMemberPath"></typeparam>
        /// <param name="selector">Function for selecting a method.</param>
        /// <returns>Verifier for further configuration.</returns>
        public IMethodVerifier<TFixtureItem, TPathProvider, TMethodMemberPath> Verify<
            TMethodMemberPath>(Func<TPathProvider, TMethodMemberPath> selector)
            where TMethodMemberPath : IMethodMemberPath<TFixtureItem>;

        /// <summary>
        /// Start the verification of a property.
        ///
        /// <example>
        /// <code>
        /// var sut = new ItemBuilder&lt;IMyType&gt;()
        ///     .Build(out var scope);
        ///
        /// var a = sut.MyProp;
        ///
        /// scope.Verify(p => p.MyProp)
        ///     .Get()
        ///     .Called();
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="TReturnType"></typeparam>
        /// <param name="selector"></param>
        /// <returns>Verifier for further configuration.</returns>
        public IPropertyVerifier<TReturnType> Verify<TReturnType>(
            Func<TPathProvider, PropertyMemberPath<TFixtureItem, TReturnType>> selector);
    }
}