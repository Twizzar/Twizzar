using System;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators.Setup
{
    /// <summary>
    /// Describes a setup filter with the desired return value.
    /// </summary>
    public interface IMethodSetupFilter
    {
        /// <summary>
        /// Gets the method name or none if all names should be considered.
        /// </summary>
        Maybe<string> MethodName { get; }

        /// <summary>
        /// Gets the return value which should be configured.
        /// </summary>
        object ReturnValue { get; }

        /// <summary>
        /// Gets the parameter types of the method or none.
        /// </summary>
        Maybe<Type[]> Parameters { get; }

        /// <summary>
        /// Check if the method descriptions matches the current filter.
        /// </summary>
        /// <param name="methodDescription">The method description.</param>
        /// <returns>True when they match; otherwise false.</returns>
        bool IsMatching(IRuntimeMethodDescription methodDescription);
    }
}