using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions
{
    /// <summary>
    /// Defines a parameter of the fixture type.
    /// </summary>
    public interface IParameterDefinition : IMemberDefinition
    {
        /// <summary>
        /// Gets parameter description.
        /// </summary>
        public IRuntimeParameterDescription ParameterDescription { get; }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; }
    }
}
