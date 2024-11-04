namespace Twizzar.Fixture;

/// <summary>
/// Static helper class for <see cref="IItemContext{TFixtureItem}"/>.
/// </summary>
public static class ItemContext
{
    /// <summary>
    /// Create a scope with path information from a scope without the path information.
    /// </summary>
    /// <param name="context"></param>
    /// <typeparam name="TFixtureItem"></typeparam>
    /// <typeparam name="TPathProvider"></typeparam>
    /// <returns></returns>
    public static IItemContext<TFixtureItem, TPathProvider> FromScope<TFixtureItem, TPathProvider>(IItemContext<TFixtureItem> context)
        where TPathProvider : new() =>
            new ItemContext<TFixtureItem, TPathProvider>(((ItemContext<TFixtureItem>)context).InstanceCacheQuery);
}