using System;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture.Member
{
    /// <summary>
    /// Configuration for configuring a type.
    /// </summary>
    /// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
    internal class ItemTypeConfig<TFixtureItem> : IItemTypeConfig<TFixtureItem>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemTypeConfig{TFixtureItem}"/> class.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        internal ItemTypeConfig(IMemberPath<TFixtureItem> path, Type type)
        {
            this.Path = path ?? throw new ArgumentNullException(nameof(path));
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public Type Type { get; }

        /// <inheritdoc />
        public IMemberPath<TFixtureItem> Path { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public override string ToString() =>
                $"{this.Path} is {this.Type}";

        #endregion
    }
}