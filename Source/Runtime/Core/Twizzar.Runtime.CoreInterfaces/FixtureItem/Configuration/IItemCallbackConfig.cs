namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

/// <summary>
/// Configuration for a callback.
/// </summary>
/// <typeparam name="TFixtureItem"></typeparam>
public interface IItemCallbackConfig<TFixtureItem> : IItemMemberConfig<TFixtureItem>
{
    /// <summary>
    /// Gets the callback, which should be an Action&lt;TParam1, TParam2, ..., TParamN&gt;.
    /// </summary>
    object Callback { get; }
}