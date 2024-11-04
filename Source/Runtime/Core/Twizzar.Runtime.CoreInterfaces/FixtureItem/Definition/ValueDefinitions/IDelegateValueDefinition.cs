namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;

/// <summary>
/// Indicates that the value should be calculated by a delegate.
/// </summary>
public interface IDelegateValueDefinition : IValueDefinition
{
    /// <summary>
    /// Gets the delegate, which should be a Func&lt;TParam1, TParam2, ..., TParamN, TReturn&gt;.
    /// </summary>
    object ValueDelegate { get; }
}