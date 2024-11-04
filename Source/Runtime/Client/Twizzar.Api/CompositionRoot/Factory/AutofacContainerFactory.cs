using System.Diagnostics.CodeAnalysis;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Twizzar.SharedKernel.Infrastructure.Factory;

namespace Twizzar.CompositionRoot.Factory
{
    /// <inheritdoc cref="IAutofacContainerFactory"/>
    [ExcludeFromCodeCoverage]
    internal class AutofacContainerFactory : IAutofacContainerFactory
    {
        private readonly IRegistrationSource _registrationSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacContainerFactory"/> class.
        /// </summary>
        /// <param name="registrationSource">The registration source.</param>
        public AutofacContainerFactory(IRegistrationSource registrationSource)
        {
            this._registrationSource = registrationSource;
        }

        /// <inheritdoc />
        public IContainer Create()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterSource(this._registrationSource);
            return containerBuilder.Build(ContainerBuildOptions.ExcludeDefaultModules);
        }
    }
}
