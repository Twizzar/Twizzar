// ReSharper disable once TypeParameterCanBeVariant

namespace Twizzar.Fixture
{
    /// <summary>
    /// The item scope contains information gathered while creating the fixture item.
    /// </summary>
    /// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
    public interface IItemContext<TFixtureItem> : IPathSelectionProvider<TFixtureItem>
    {
    }
}