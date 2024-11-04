using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture
{
    /// <summary>
    /// Marks a member as a specific value.
    /// </summary>
    /// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
    public interface IItemValueConfig<TFixtureItem> : IItemMemberConfig<TFixtureItem>
    {
        /// <summary>
        /// Gets the value configured.
        /// </summary>
        object Value { get; }
    }
}