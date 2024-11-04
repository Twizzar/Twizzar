using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition
{
    /// <summary>
    /// Definition node of a base type fixture item.
    /// </summary>
    public interface IBaseTypeNode : IFixtureItemDefinitionNode
    {
        /// <summary>
        /// Gets the value definition which describes how the value is constructed.
        /// </summary>
        public IValueDefinition ValueDefinition { get; }

        /// <summary>
        /// Gets a value indicating whether the base type is nullable.
        /// </summary>
        public bool IsNullable { get; }
    }
}