using System;
using Autofac.Core;

namespace Twizzar.Runtime.Infrastructure.AutofacServices.Factory
{
    /// <summary>
    /// Interface for creating the autofac main activation handler.
    /// </summary>
    public interface IMainActivatorFactory
    {
        /// <summary>
        /// Create the main activator instance for the given type and scope.
        /// </summary>
        /// <param name="type">The type for the activator.</param>
        /// <param name="scope">The scope for the activator, can be null.</param>
        /// <returns>An Autofac IInstanceActivator.</returns>
        IInstanceActivator Create(Type type, string scope);
    }
}