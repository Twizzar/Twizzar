namespace Twizzar.Fixture;

// ReSharper disable UnusedTypeParameter
#pragma warning disable S2326 // Unused type parameters should be removed

/// <summary>
/// Marker interface used by the source generator to find object which provides path selections.
/// </summary>
/// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
/// <typeparam name="TPathProvider">The PathProvider type.</typeparam>
public interface IPathSelectionProvider<out TFixtureItem, TPathProvider> : IPathSelectionProvider<TFixtureItem>
{
}

/// <summary>
/// Marker interface used by the source generator to find object which provides path selections.
/// </summary>
/// <typeparam name="TFixtureItem">The fixture item type.</typeparam>
public interface IPathSelectionProvider<out TFixtureItem>
{
}