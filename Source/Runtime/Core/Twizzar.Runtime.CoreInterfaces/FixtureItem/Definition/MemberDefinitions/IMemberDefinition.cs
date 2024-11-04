using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions
{
    /// <summary>
    /// Defines a member of the fixture item.
    /// </summary>
    public interface IMemberDefinition : IValueObject
    {
        /// <summary>
        /// Gets the value definition which describes how the value is constructed.
        /// </summary>
        public IValueDefinition ValueDefinition { get; }
    }
}