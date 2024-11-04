using System.Collections.Immutable;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions
{
    /// <summary>
    /// The value is a sequence of many raw values see also: <see cref="IRawValueDefinition"/>.
    /// </summary>
    public interface IRawValuesDefinition : IValueDefinition
    {
        /// <summary>
        /// Gets the values.
        /// </summary>
        public ImmutableArray<string> Values { get; }
    }
}
