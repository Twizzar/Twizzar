using System;
using System.Collections.Immutable;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// ConcreteType Description value object.
    /// </summary>
    public interface IRuntimeTypeDescription : ITypeDescription
    {
        /// <summary>
        /// Gets the defined type.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets a dictionary with the generic type position as key and the ConcreteType as value.
        /// </summary>
        public IImmutableDictionary<int, Type> GenericRuntimeTypeArguments { get; }
    }
}
