using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;

namespace Twizzar.Runtime.Core.FixtureItem.Definition.ValueDefinitions;

/// <summary>
/// Indicates that the value should be calculated by a delegate.
/// </summary>
/// <param name="ValueDelegate">The delegate, which should be a Func&lt;TParam1, TParam2, ..., TParamN, TReturn&gt;.</param>
public record DelegateValueDefinition(object ValueDelegate) : IDelegateValueDefinition;