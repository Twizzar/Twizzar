using Autofac;

namespace Twizzar.SharedKernel.Infrastructure.Factory
{
    /// <summary>
    /// Factory for creating an autofac <see cref="IContainer"/>.
    /// </summary>
    public interface IAutofacContainerFactory
    {
        /// <summary>
        /// Create a new instance of the autofac <see cref="IContainer"/>.
        /// </summary>
        /// <returns>An instance of <see cref="IContainer"/>.</returns>
        IContainer Create();
    }
}
