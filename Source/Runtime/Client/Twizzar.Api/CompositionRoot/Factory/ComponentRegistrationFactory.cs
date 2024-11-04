using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Twizzar.Runtime.Infrastructure.AutofacServices.Factory;
using Twizzar.SharedKernel.Factories;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.CompositionRoot.Factory
{
    /// <summary>
    /// Implementation of the <see cref="IComponentRegistrationFactory"/>  Interface.
    /// It maps the custom Activator to the factory methods for creating the registration with the given activator.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class ComponentRegistrationFactory : FactoryBase, IComponentRegistrationFactory
    {
        private readonly IMainActivatorFactory _activatorFactory;

        private readonly FactoryDelegate _factoryDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentRegistrationFactory"/> class.
        /// </summary>
        /// <param name="componentContext">The autofac component context.</param>
        /// <param name="factoryDelegate">The factory for autofac.</param>
        /// <param name="activatorFactory">The activator factory.</param>
        public ComponentRegistrationFactory(
            IComponentContext componentContext,
            FactoryDelegate factoryDelegate,
            IMainActivatorFactory activatorFactory)
            : base(componentContext)
        {
            this.EnsureMany()
                .Parameter(activatorFactory, nameof(activatorFactory))
                .Parameter(activatorFactory, nameof(activatorFactory))
                .ThrowWhenNull();

            this._activatorFactory = activatorFactory;
            this._factoryDelegate = factoryDelegate;
        }

#pragma warning disable SA1600 // Elements should be documented
        public delegate IComponentRegistration FactoryDelegate(
            Guid id,
            IInstanceActivator activator,
            IComponentLifetime lifetime,
            InstanceSharing sharing,
            InstanceOwnership ownership,
            IEnumerable<Service> services,
            IDictionary<string, object> metadata);
#pragma warning restore SA1600 // Elements should be documented

        #region Implementation of IComponentRegistrationFactory

        /// <inheritdoc />
        public IComponentRegistration[] Create(
            Service autofacService,
            Type type,
            string scope = null)
        {
            this.EnsureMany()
                .Parameter(autofacService, nameof(autofacService))
                .Parameter(type, nameof(type))
                .ThrowWhenNull();

            // get activator from activator factory
            var activator = this._activatorFactory.Create(type, scope);

            return this.CreateComponentRegistration(autofacService, activator);
        }

        #endregion

        #region private helpers

        private IComponentRegistration[] CreateComponentRegistration(
            Service service,
            IInstanceActivator activator)
        {
            var componentRegistration = this._factoryDelegate(
                Guid.NewGuid(),
                activator,
                new CurrentScopeLifetime(),
                InstanceSharing.None,
                InstanceOwnership.OwnedByLifetimeScope,
                new[] { service },
                new Dictionary<string, object>());

            return new[] { componentRegistration };
        }

        #endregion
    }
}
