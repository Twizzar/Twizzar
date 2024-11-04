namespace Twizzar.Fixture
{
    /// <summary>
    /// Builder for configuring a fixture item then building a concrete instance of it.
    /// </summary>
    /// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
    public class ItemBuilder<TFixtureItem> : ItemBuilderBase<TFixtureItem>, IItemBuilder<TFixtureItem>, IPathSelectionProvider<TFixtureItem>
    {
    }
}