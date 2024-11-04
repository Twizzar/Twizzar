using System.Collections.Generic;
using System.Collections.Immutable;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration
{
    /// <summary>
    /// Represents a configuration of a FixtureItem.
    /// The member configurations are stored flat and there hierarchical structure is represented over the
    /// <see cref="IMemberPath{TFixtureItem}"/> in the <see cref="IItemMemberConfig{TFixtureItem}"/>.
    /// </summary>
    /// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
    public interface IItemConfig<TFixtureItem>
    {
        /// <summary>
        /// Gets the name of the fixture item.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the member configurations.
        /// </summary>
        IImmutableDictionary<IMemberPath<TFixtureItem>, IItemMemberConfig<TFixtureItem>> MemberConfigurations { get; }

        /// <summary>
        /// Gets the method callbacks.
        /// </summary>
        IImmutableDictionary<IMemberPath<TFixtureItem>, IList<object>> Callbacks { get; }
    }
}