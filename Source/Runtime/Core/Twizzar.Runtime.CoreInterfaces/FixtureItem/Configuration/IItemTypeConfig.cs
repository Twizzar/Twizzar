using System;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture
{
    /// <summary>
    /// Marks a member as using a specified type (InstanceOf or Stub).
    /// This can only be used by non base types.
    /// </summary>
    /// <typeparam name="TFixtureItem">The Fixture item type.</typeparam>
    public interface IItemTypeConfig<TFixtureItem> : IItemMemberConfig<TFixtureItem>
    {
        /// <summary>
        /// Gets the type configured. This can be the member type or a assignable type.
        /// </summary>
        Type Type { get; }
    }
}