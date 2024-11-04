using System;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Gets the attributes for this parameter.
    /// </summary>
    public interface IRuntimeParameterDescription : IParameterDescription
    {
        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        public Type Type { get; }
    }
}