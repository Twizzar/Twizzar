using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions
{
    /// <summary>
    /// Defines a property of the fixture item.
    /// </summary>
    public interface IPropertyDefinition : IMemberDefinition
    {
        /// <summary>
        /// Gets the property description.
        /// </summary>
        public IRuntimePropertyDescription PropertyDescription { get; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name { get; }
    }
}
