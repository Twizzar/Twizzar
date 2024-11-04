using System.Collections.Immutable;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition
{
    /// <summary>
    /// Interface definition node contains all information to create an instance of a interface.
    /// </summary>
    public interface IMockNode : IFixtureItemDefinitionNode
    {
        /// <summary>
        /// Gets all properties definitions of the interface.
        /// </summary>
        public ImmutableArray<IPropertyDefinition> Properties { get; }

        /// <summary>
        /// Gets all method definitions of the interface.
        /// </summary>
        public ImmutableArray<IMethodDefinition> Methods { get; }
    }
}
