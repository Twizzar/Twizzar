using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac.Core;
using Twizzar.Runtime.Infrastructure.AutofacServices.Factory;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Runtime.Infrastructure.AutofacServices.Registration
{
    /// <summary>
    /// Implementation of the <see cref="IRegistrationSource"/>  Interface.
    /// The class is used to create instance of types when not registered explicitly
    /// (which are all types in our use case).
    /// </summary>
    public class RegistrationSource : IRegistrationSource, IHasLogger, IHasEnsureHelper
    {
        #region fields

        private readonly IComponentRegistrationFactory _componentRegistrationFactory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationSource"/> class.
        /// The class will be used for on the fly registration of types in the AutoFac container.
        /// </summary>
        /// <param name="componentRegistrationFactory">The component registration factory.</param>
        public RegistrationSource(IComponentRegistrationFactory componentRegistrationFactory)
        {
            this._componentRegistrationFactory =
                componentRegistrationFactory ??
                throw new ArgumentNullException(nameof(componentRegistrationFactory));
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public bool IsAdapterForIndividualComponents => false;

        #endregion

        #region members

        /// <summary>
        /// Registration of the components.
        /// </summary>
        /// <param name="service">The autofac service.</param>
        /// <param name="registrationAccessor">The registration accessor for already available registrations.</param>
        /// <returns>The newly created component registration.</returns>
        public IEnumerable<IComponentRegistration> RegistrationsFor(
            Service service,
            Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            this.EnsureMany()
                .Parameter(service, nameof(service))
                .Parameter(registrationAccessor, nameof(registrationAccessor))
                .ThrowWhenNull();

            // check for registrations in the container and return if available.
            var registrations = registrationAccessor(service);

            if (registrations.Any())
            {
                return registrations;
            }

            if (!(service is IServiceWithType swt))
            {
                // It's not a request for the base handler type, so skip it.
                return Enumerable.Empty<IComponentRegistration>();
            }

            var typeInfo = swt.ServiceType.GetTypeInfo();

            if (IsInsideAutofac(typeInfo))
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            string scope = null;

            if (service is KeyedService ks)
            {
                scope = ks.ServiceKey.ToString();
            }

            // create the component registration with the activator and creator included.
            return this._componentRegistrationFactory.Create(service, swt.ServiceType, scope);
        }

        private static bool IsInsideAutofac(Type type) =>
            typeof(Autofac.Core.IRegistrationSource).Assembly == type.Assembly;

        #endregion
    }
}