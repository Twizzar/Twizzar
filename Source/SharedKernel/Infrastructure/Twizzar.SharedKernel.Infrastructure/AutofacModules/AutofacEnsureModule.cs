using System;
using System.Linq;
using System.Reflection;
using Autofac.Core;
using Autofac.Core.Registration;
using Twizzar.SharedKernel.Infrastructure.Factory;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.Extensions;
using Module = Autofac.Module;

namespace Twizzar.SharedKernel.Infrastructure.AutofacModules
{
    /// <summary>
    /// Autofac module for injection <see cref="IEnsureHelper"/> properties automatically.
    /// </summary>
    public class AutofacEnsureModule : Module
    {
        private readonly IEnsureHelperWithLoggingFactory _ensureHelperFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacEnsureModule"/> class.
        /// </summary>
        /// <param name="factory">Factory to create an <see cref="IEnsureHelper"/>.</param>
        public AutofacEnsureModule(IEnsureHelperWithLoggingFactory factory)
        {
            this._ensureHelperFactory = EnsureHelper.GetDefault
                .EnsureParameterIsNotNullThenReturn(factory, nameof(factory));
        }

        /// <inheritdoc />
        protected override void AttachToComponentRegistration(
            IComponentRegistryBuilder componentRegistry,
            IComponentRegistration registration)
        {
            if (registration is null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            // Handle properties.
            registration.Activated += (sender, e) => this.InjectLoggerProperties(e.Instance);
        }

        private void InjectLoggerProperties(object instance)
        {
            var instanceType = instance.GetType();

            // Get all the injectable properties to set.
            var property = instanceType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => p.PropertyType == typeof(IEnsureHelper) && p.CanWrite && p.GetIndexParameters().Length == 0);

            if (property != null)
            {
                var ensureHelper = typeof(IHasLogger).IsAssignableFrom(instanceType)
                    ? this._ensureHelperFactory.Create((IHasLogger)instance)
                    : this._ensureHelperFactory.Create();

                property.SetValue(instance, ensureHelper, null);
            }
        }
    }
}
