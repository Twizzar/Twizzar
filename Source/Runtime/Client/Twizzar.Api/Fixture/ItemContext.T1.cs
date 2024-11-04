using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;

namespace Twizzar.Fixture;

/// <inheritdoc cref="IItemContext{TFixtureItem}"/>
internal class ItemContext<TFixtureItem> : IItemContext<TFixtureItem>
{
    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemContext{TFixtureItem}"/> class.
    /// </summary>
    /// <param name="instanceCacheQuery"></param>
    internal ItemContext(IInstanceCacheQuery instanceCacheQuery)
    {
        this.InstanceCacheQuery = instanceCacheQuery;
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets the instance cache query service.
    /// </summary>
    internal IInstanceCacheQuery InstanceCacheQuery { get; }

    #endregion
}