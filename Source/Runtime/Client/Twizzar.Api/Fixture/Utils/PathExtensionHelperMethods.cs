using Twizzar.Fixture.Member;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture.Utils
{
    /// <summary>
    /// Provides helper methods for <see cref="IMemberPath{TFixtureItem}"/>.
    /// </summary>
    internal static class PathExtensionHelperMethods
    {
        #region members

        /// <summary>
        /// Configure as unique.
        /// </summary>
        /// <typeparam name="TFixtureItem">The type of the fixture item.</typeparam>
        /// <param name="self"></param>
        /// <returns>A <see cref="MemberConfig{TFixtureItem}"/>.</returns>
        public static MemberConfig<TFixtureItem> Unique<TFixtureItem>(IBaseTypePath<TFixtureItem> self) =>
            ItemMemberConfigFactory.Unique((IMemberPath<TFixtureItem>)self);

        /// <summary>
        /// Configure as undefined.
        /// </summary>
        /// <typeparam name="TFixtureItem">The type of the fixture item.</typeparam>
        /// <param name="self"></param>
        /// <returns>A <see cref="MemberConfig{TFixtureItem}"/>.</returns>
        public static MemberConfig<TFixtureItem> Undefined<TFixtureItem>(IInstancePath<TFixtureItem> self) =>
            ItemMemberConfigFactory.Undefined((IMemberPath<TFixtureItem>)self);

        /// <summary>
        /// Configure as an delegate which calculates the return value.
        /// </summary>
        /// <typeparam name="TFixtureItem"></typeparam>
        /// <param name="self"></param>
        /// <param name="delegateValue"></param>
        /// <returns></returns>
        public static MemberConfig<TFixtureItem> Delegate<TFixtureItem>(
            IMemberPath<TFixtureItem> self,
            object delegateValue) =>
                ItemMemberConfigFactory.ViDelegate(self, delegateValue);

        /// <summary>
        /// Register a callback to an methods.
        /// </summary>
        /// <typeparam name="TFixtureItem"></typeparam>
        /// <param name="self"></param>
        /// <param name="delegateValue"></param>
        /// <returns></returns>
        public static MemberConfig<TFixtureItem> Callback<TFixtureItem>(
            IMemberPath<TFixtureItem> self,
            object delegateValue) =>
                ItemMemberConfigFactory.ViCallback(self, delegateValue);

        #endregion
    }
}