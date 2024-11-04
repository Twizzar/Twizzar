namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;

/// <summary>
/// Configuration for a delegate which calculates the return value.
/// </summary>
/// <typeparam name="TFixtureItem"></typeparam>
public interface IItemDelegateConfig<TFixtureItem> : IItemMemberConfig<TFixtureItem>
{
    /// <summary>
    /// Gets the delegate which calculates the return value.
    /// </summary>
    object DelegateValue { get; }
}