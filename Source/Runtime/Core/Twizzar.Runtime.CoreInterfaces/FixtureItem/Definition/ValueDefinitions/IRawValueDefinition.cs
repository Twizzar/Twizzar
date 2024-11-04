namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions
{
    /// <summary>
    /// The value is a 'raw value' which means it was set in the configuration and is stored in the definition.
    /// </summary>
    public interface IRawValueDefinition : IValueDefinition
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value { get; }
    }
}
