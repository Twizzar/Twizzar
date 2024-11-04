using System;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture.Member
{
    /// <inheritdoc cref="IItemUniqueConfig{TFixtureItem}"/>
    internal class ItemUniqueConfig<TFixtureItem> : IItemUniqueConfig<TFixtureItem>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemUniqueConfig{TFixtureItem}"/> class.
        /// </summary>
        /// <param name="path"></param>
        public ItemUniqueConfig(IMemberPath<TFixtureItem> path)
        {
            this.Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public IMemberPath<TFixtureItem> Path { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public override string ToString() =>
                $"{this.Path} is unique";

        #endregion
    }
}