using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture.Member
{
    /// <summary>
    /// A configuration of a member.
    /// </summary>
    /// <typeparam name="TFixtureItem">The root fixture item type.</typeparam>
    public class MemberConfig<TFixtureItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberConfig{TFixtureItem}"/> class.
        /// </summary>
        /// <param name="itemMemberConfig"></param>
        internal MemberConfig(IItemMemberConfig<TFixtureItem> itemMemberConfig)
        {
            this.ItemMemberConfig = itemMemberConfig;
        }

        /// <summary>
        /// Gets the item member config.
        /// </summary>
        internal IItemMemberConfig<TFixtureItem> ItemMemberConfig { get; }
    }
}