namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration
{
    /// <summary>
    /// Represents a member configuration of the <see cref="IItemConfig{TFixtureItem}"/>.
    /// This must not be a direct child member of the FixtureItem, it also can be a deeper descendant.
    /// </summary>
    /// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
    public interface IItemMemberConfig<TFixtureItem>
    {
        /// <summary>
        /// Gets the path to this member. The path describes the tree like structure of the FixtureItem dependencies.
        /// </summary>
        public IMemberPath<TFixtureItem> Path { get; }
    }
}