using System;
using System.Reflection;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Description
{
    /// <inheritdoc />
    public interface IRuntimeMethodDescription : IMethodDescription
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Get the method info if it is a method; else None (for constructor).
        /// </summary>
        /// <returns></returns>
        Maybe<MethodInfo> GetMethodInfo();
    }
}
