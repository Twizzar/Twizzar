using Twizzar.Fixture.Member;

namespace Twizzar.Fixture
{
    /// <summary>
    /// Provides member paths. Is used for configure an <see cref="ItemBuilder{TFixtureItem,TPathProvider}"/>.
    /// </summary>
    /// <typeparam name="TFixtureItem">The type for which member paths will be generated.</typeparam>
    public abstract class PathProvider<TFixtureItem>
    {
        /// <summary>
        /// Gets the config root path.
        /// </summary>
        protected static readonly MemberPath<TFixtureItem> RootPath =
            new RootPath<TFixtureItem>(typeof(TFixtureItem).FullName);
    }
}