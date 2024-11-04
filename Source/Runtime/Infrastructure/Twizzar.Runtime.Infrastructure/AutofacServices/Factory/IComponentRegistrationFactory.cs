using System;
using Autofac.Core;

namespace Twizzar.Runtime.Infrastructure.AutofacServices.Factory
{
    /// <summary>
    /// Interface to create Autofac component registrations with specified types.
    /// </summary>
    public interface IComponentRegistrationFactory
    {
        /// <summary>
        /// Creates the bases type component registration for their default values.
        /// </summary>
        /// <param name="autofacService">The autofac service.</param>
        /// <param name="type">The type which need to be registered.</param>
        /// <param name="scope">The scope of the lookup.</param>
        /// <returns>A newly created component registration for the given type.</returns>
        IComponentRegistration[] Create(Service autofacService, Type type, string scope = null);
    }
}