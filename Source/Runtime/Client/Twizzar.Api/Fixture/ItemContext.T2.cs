using System;
using Moq;
using Twizzar.Fixture.Member;
using Twizzar.Fixture.MethodVerifier;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using Twizzar.Runtime.CoreInterfaces.Resources;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Fixture
{
    /// <inheritdoc cref="IItemContext{TFixtureItem,TPathProvider}"/>
    internal class ItemContext<TFixtureItem, TPathProvider> : IItemContext<TFixtureItem, TPathProvider>
        where TPathProvider : new()
    {
        #region static fields and constants

        private static readonly TPathProvider PathProvider = new();

        #endregion

        #region fields

        private readonly IInstanceCacheQuery _instanceCacheQuery;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemContext{TFixtureItem,TPathProvider}"/> class.
        /// </summary>
        /// <param name="instanceCacheQuery"></param>
        public ItemContext(IInstanceCacheQuery instanceCacheQuery)
        {
            EnsureHelper.GetDefault
                .Parameter(instanceCacheQuery, nameof(instanceCacheQuery))
                .ThrowWhenNull();

            this._instanceCacheQuery = instanceCacheQuery;
        }

        #endregion

        #region members

        /// <inheritdoc />
        public TReturnType Get<TReturnType>(Func<TPathProvider, MemberPath<TFixtureItem, TReturnType>> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var path = selector(PathProvider).ToString();

            return this._instanceCacheQuery.GetInstance(path)
                .Map(o => (TReturnType)o)
                .SomeOrProvided(
                    () => throw new InvalidOperationException(
                        string.Format(
                            ErrorMessagesRuntime.ItemScope_Get_A_instance_for_the_path__0__cannot_be_found_,
                            path)));
        }

        /// <inheritdoc />
        public Mock<TReturnType> GetAsMoq<TReturnType>(
            Func<TPathProvider, MemberPath<TFixtureItem, TReturnType>> selector)
            where TReturnType : class =>
            Mock.Get(this.Get(selector));

        /// <inheritdoc />
        public IMethodVerifier<TFixtureItem, TPathProvider, TMethodMemberPath> Verify<TMethodMemberPath>(
            Func<TPathProvider, TMethodMemberPath> selector)
            where TMethodMemberPath : IMethodMemberPath<TFixtureItem>
        {
            var p = selector(new TPathProvider());
            return new MethodVerifier<TFixtureItem, TPathProvider, TMethodMemberPath>(p, this._instanceCacheQuery);
        }

        /// <inheritdoc />
        public IPropertyVerifier<TReturnType> Verify<TReturnType>(
            Func<TPathProvider, PropertyMemberPath<TFixtureItem, TReturnType>> selector)
        {
            var p = selector(new TPathProvider());
            return new PropertyVerifier<TFixtureItem, TReturnType, TPathProvider>(p, this._instanceCacheQuery);
        }

        #endregion
    }
}