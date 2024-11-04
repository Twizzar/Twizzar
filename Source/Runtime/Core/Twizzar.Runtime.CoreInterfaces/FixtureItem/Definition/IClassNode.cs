using System.Collections.Immutable;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition
{
    /// <summary>
    /// Class definition node contains all information to create an instance of a class.
    /// </summary>
    public interface IClassNode : IMockNode
    {
        /// <summary>
        /// Gets the parameters of the constructor.
        /// </summary>
        public ImmutableArray<IParameterDefinition> ConstructorParameters { get; }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        public ImmutableArray<IFieldDefinition> Fields { get; }
    }
}