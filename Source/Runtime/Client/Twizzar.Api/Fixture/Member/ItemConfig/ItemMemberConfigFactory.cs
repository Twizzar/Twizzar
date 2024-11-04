using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture.Member
{
    /// <summary>
    /// Static factory for creating <see cref="MemberConfig{TFixtureItem}"/>.
    /// </summary>
    internal static class ItemMemberConfigFactory
    {
        #region members

        /// <summary>
        /// Creates a <see cref="MemberConfig{TFixtureItem}"/> which wraps an <see cref="ItemValueConfig{TFixtureItem}"/>.
        /// </summary>
        /// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
        /// <typeparam name="TReturnType">The return type.</typeparam>
        /// <param name="path"></param>
        /// <param name="value"></param>
        /// <returns>A new instance of <see cref="MemberConfig{TFixtureItem}"/>.</returns>
        public static MemberConfig<TFixtureItem> Value<TFixtureItem, TReturnType>(
            IMemberPath<TFixtureItem> path,
            TReturnType value) =>
            Wrap(new ItemValueConfig<TFixtureItem>(path, value));

        /// <summary>
        /// Creates a <see cref="MemberConfig{TFixtureItem}"/>.  which wraps an <see cref="ItemUniqueConfig{TFixtureItem}"/>.
        /// </summary>
        /// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
        /// <param name="path"></param>
        /// <returns>A new instance of <see cref="MemberConfig{TFixtureItem}"/>.</returns>
        public static MemberConfig<TFixtureItem> Unique<TFixtureItem>(
            IMemberPath<TFixtureItem> path) =>
            Wrap(new ItemUniqueConfig<TFixtureItem>(path));

        /// <summary>
        /// Creates a <see cref="MemberConfig{TFixtureItem}"/> which wraps an <see cref="ItemTypeConfig{TFixtureItem}"/>.
        /// </summary>
        /// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
        /// <typeparam name="TType">The type configured.</typeparam>
        /// <param name="path"></param>
        /// <returns>A new instance of <see cref="MemberConfig{TFixtureItem}"/>.</returns>
        public static MemberConfig<TFixtureItem> ViType<TFixtureItem, TType>(
            IMemberPath<TFixtureItem> path) =>
            Wrap(new ItemTypeConfig<TFixtureItem>(path, typeof(TType)));

        /// <summary>
        /// Creates a <see cref="MemberConfig{TFixtureItem}"/> which wraps an <see cref="ItemUndefinedConfig{TFixtureItem}"/>.
        /// </summary>
        /// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
        /// <param name="path"></param>
        /// <returns>A new instance of <see cref="MemberConfig{TFixtureItem}"/>.</returns>
        public static MemberConfig<TFixtureItem> Undefined<TFixtureItem>(
            IMemberPath<TFixtureItem> path) =>
            Wrap(new ItemUndefinedConfig<TFixtureItem>(path));

        /// <summary>
        /// Creates a <see cref="MemberConfig{TFixtureItem}"/> which wraps an <see cref="ItemDelegateConfig{TFixtureItem}"/>.
        /// </summary>
        /// <typeparam name="TFixtureItem"></typeparam>
        /// <param name="path"></param>
        /// <param name="delegateValue"></param>
        /// <returns></returns>
        public static MemberConfig<TFixtureItem> ViDelegate<TFixtureItem>(
            IMemberPath<TFixtureItem> path,
            object delegateValue) =>
            Wrap(new ItemDelegateConfig<TFixtureItem>(path, delegateValue));

        /// <summary>
        /// Creates a <see cref="MemberConfig{TFixtureItem}"/> which wraps an <see cref="ItemCallbackConfig{TFixtureItem}"/>.
        /// </summary>
        /// <typeparam name="TFixtureItem"></typeparam>
        /// <param name="path"></param>
        /// <param name="callbackDelegate"></param>
        /// <returns></returns>
        public static MemberConfig<TFixtureItem> ViCallback<TFixtureItem>(
            IMemberPath<TFixtureItem> path,
            object callbackDelegate) =>
            Wrap(new ItemCallbackConfig<TFixtureItem>(path, callbackDelegate));

        private static MemberConfig<TFixtureItem> Wrap<TFixtureItem>(
            IItemMemberConfig<TFixtureItem> itemMemberConfig) =>
            new(itemMemberConfig);

        #endregion
    }
}