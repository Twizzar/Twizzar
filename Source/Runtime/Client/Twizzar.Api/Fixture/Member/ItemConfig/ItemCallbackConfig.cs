using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

namespace Twizzar.Fixture.Member;

/// <inheritdoc cref="IItemCallbackConfig{TFixtureItem}"/>
public class ItemCallbackConfig<TFixtureItem> : IItemCallbackConfig<TFixtureItem>
{
    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemCallbackConfig{TFixtureItem}"/> class.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public ItemCallbackConfig(IMemberPath<TFixtureItem> path, object callback)
    {
        this.Callback = callback;
        this.Path = path;
    }

    #endregion

    #region properties

    /// <inheritdoc />
    public object Callback { get; }

    /// <inheritdoc />
    public IMemberPath<TFixtureItem> Path { get; }

    #endregion
}