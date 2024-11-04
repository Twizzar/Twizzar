using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture.Member
{
    /// <inheritdoc cref="IItemUndefinedConfig{TFixtureItem}"/>
    internal class ItemUndefinedConfig<TFixtureItem> : IItemUndefinedConfig<TFixtureItem>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemUndefinedConfig{TFixtureItem}"/> class.
        /// </summary>
        /// <param name="path"></param>
        public ItemUndefinedConfig(IMemberPath<TFixtureItem> path)
        {
            this.Path = path;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public IMemberPath<TFixtureItem> Path { get; }

        #endregion
    }
}