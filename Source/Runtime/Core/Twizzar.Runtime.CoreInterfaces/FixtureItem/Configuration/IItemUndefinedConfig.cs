using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture
{
    /// <summary>
    /// Marks the member as Undefined. This is only available for instance members.
    /// </summary>
    /// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
    public interface IItemUndefinedConfig<TFixtureItem> : IItemMemberConfig<TFixtureItem>
    {
    }
}