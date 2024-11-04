using System;
using System.Linq;
using System.Reflection;
using Autofac.Core;
using Autofac.Core.Registration;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;
using Module = Autofac.Module;

namespace Twizzar.SharedKernel.Infrastructure.AutofacModules
{
    /// <summary>
    /// Autofac module for injection logger properties automatically.
    /// </summary>
    public class AutofacLoggingModule : Module
    {
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
            registration.Activated += (sender, e) => InjectLoggerProperties(e.Instance);
        }

        private static void InjectLoggerProperties(object instance)
        {
            var instanceType = instance.GetType();

            // Get all the injectable properties to set.
            var properties = instanceType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(ILogger) && p.CanWrite && p.GetIndexParameters().Length == 0);

            // Set the properties located.
            foreach (var propToSet in properties)
            {
                propToSet.SetValue(instance, LoggerFactory.GetLogger(instanceType), null);
            }
        }
    }
}
