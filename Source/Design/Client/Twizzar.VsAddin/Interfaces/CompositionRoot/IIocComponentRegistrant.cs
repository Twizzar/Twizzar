using Autofac;

namespace Twizzar.VsAddin.Interfaces.CompositionRoot
{
    /// <summary>
    /// A component registrant registers one or many components for the ioc container.
    /// </summary>
    public interface IIocComponentRegistrant
    {
        /// <summary>
        /// Register components with a container builder.
        /// </summary>
        /// <param name="builder">The autofac builder.</param>
        void RegisterComponents(ContainerBuilder builder);
    }
}
