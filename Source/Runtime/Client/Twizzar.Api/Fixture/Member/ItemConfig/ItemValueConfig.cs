using System;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture.Member
{
    /// <inheritdoc cref="IItemValueConfig{TFixtureItem}"/>
    internal class ItemValueConfig<TFixtureItem> : IItemValueConfig<TFixtureItem>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemValueConfig{TFixtureItem}"/> class.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        public ItemValueConfig(IMemberPath<TFixtureItem> path, object value)
        {
            this.Path = path ?? throw new ArgumentNullException(nameof(path));
            this.Value = value;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public IMemberPath<TFixtureItem> Path { get; }

        /// <inheritdoc />
        public object Value { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public override string ToString() =>
                $"{this.Path} = {this.Value}";

        #endregion
    }
}