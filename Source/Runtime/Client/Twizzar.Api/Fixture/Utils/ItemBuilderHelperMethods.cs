using System;

using Twizzar.Fixture.Member;

namespace Twizzar.Fixture.Utils
{
    /// <summary>
    /// Helper methods for the <see cref="ItemBuilder{TFixtureItem}"/>.
    /// </summary>
    public static class ItemBuilderHelperMethods
    {
        /// <summary>
        /// Provides the <see cref="ItemBuilder{TFixtureItem,TPathProvider}.With"/> methods for <see cref="ItemBuilder{TFixtureItem}"/>.
        /// </summary>
        /// <typeparam name="TFixtureItem">The type of the fixture item.</typeparam>
        /// <typeparam name="TPathProvider">The type of the path provider.</typeparam>
        /// <param name="builder"></param>
        /// <param name="func"></param>
        /// <returns>The provides builder.</returns>
        public static ItemBuilderBase<TFixtureItem> With<TFixtureItem, TPathProvider>(
            ItemBuilderBase<TFixtureItem> builder,
            Func<TPathProvider, MemberConfig<TFixtureItem>> func)
            where TPathProvider : new() =>
                builder.AddMemberConfig(func(new TPathProvider()));
    }
}
