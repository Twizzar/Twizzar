using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture.Member;

/// <inheritdoc cref="IItemDelegateConfig{TFixtureItem}"/>
public class ItemDelegateConfig<TFixtureItem> : IItemDelegateConfig<TFixtureItem>
{
    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemDelegateConfig{TFixtureItem}"/> class.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="delegateValue"></param>
    public ItemDelegateConfig(IMemberPath<TFixtureItem> path, object delegateValue)
    {
        this.Path = path;
        this.DelegateValue = delegateValue;
    }

    #endregion

    #region properties

    /// <inheritdoc />
    public IMemberPath<TFixtureItem> Path { get; }

    /// <inheritdoc />
    public object DelegateValue { get; }

    #endregion
}