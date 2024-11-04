using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions
{
    /// <summary>
    /// Defines a field of the fixture item.
    /// </summary>
    public interface IFieldDefinition : IMemberDefinition
    {
        /// <summary>
        /// Gets the property description.
        /// </summary>
        public IRuntimeFieldDescription FieldDescription { get; }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        public string Name { get; }
    }
}
