using System;

using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Common.Util
{
    /// <summary>
    /// A scoped service provider. This should only be used when a inject of services in the constructor is no possible.
    /// </summary>
    public interface IScopedServiceProvider : IDisposable
    {
        /// <summary>
        /// Get a service by its type.
        /// </summary>
        /// <typeparam name="T">The type of the service.</typeparam>
        /// <returns>The service.</returns>
        Maybe<T> GetService<T>()
            where T : class;
    }
}