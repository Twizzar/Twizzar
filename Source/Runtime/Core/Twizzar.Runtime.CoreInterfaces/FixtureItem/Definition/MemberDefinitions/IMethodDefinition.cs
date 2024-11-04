using System.Collections.Generic;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions
{
    /// <summary>
    /// Defines a method of the fixture item.
    /// </summary>
    public interface IMethodDefinition : IMemberDefinition
    {
        /// <summary>
        /// Gets the method description.
        /// </summary>
        IRuntimeMethodDescription MethodDescription { get; }

        /// <summary>
        /// Gets the callback delegates for this method.
        /// </summary>
        IEnumerable<object> Callbacks { get; }
    }
}