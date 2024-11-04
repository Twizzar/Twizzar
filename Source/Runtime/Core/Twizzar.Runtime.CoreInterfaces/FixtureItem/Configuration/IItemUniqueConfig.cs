using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture
{
    /// <summary>
    /// Marks the member as Unique. This is only available for base types.
    /// </summary>
    /// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
    public interface IItemUniqueConfig<TFixtureItem> : IItemMemberConfig<TFixtureItem>
    {
    }
}