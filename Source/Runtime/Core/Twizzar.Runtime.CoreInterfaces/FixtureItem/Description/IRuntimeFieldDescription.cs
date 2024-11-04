using System;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Discovers the attributes of a field and provides access to field metadata.
    /// </summary>
    public interface IRuntimeFieldDescription : IFieldDescription
    {
        /// <summary>
        /// Gets the type name.
        /// </summary>
        public Type Type { get; }
    }
}