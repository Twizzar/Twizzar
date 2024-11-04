using System;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Member description with type.
    /// </summary>
    public interface IRuntimeMemberDescription : IMemberDescription
    {
        /// <summary>
        /// Gets the type name.
        /// </summary>
        public Type Type { get; }
    }
}
