using System.Collections.Generic;
using System.Collections.Immutable;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Runtime.Core.FixtureItem.Configuration
{
    /// <inheritdoc cref="IItemConfig{TFixtureItem}"/>
    public record ItemConfig<TFixtureItem>(
        string Name,
        IImmutableDictionary<IMemberPath<TFixtureItem>, IItemMemberConfig<TFixtureItem>> MemberConfigurations,
        IImmutableDictionary<IMemberPath<TFixtureItem>, IList<object>> Callbacks)
        : IItemConfig<TFixtureItem>;
}