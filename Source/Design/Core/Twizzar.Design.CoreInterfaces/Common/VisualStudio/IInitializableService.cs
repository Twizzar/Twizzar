using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.CoreInterfaces.Common.VisualStudio
{
    /// <summary>
    /// Service which needs to be initialized.
    /// </summary>
    /// <seealso cref="IService" />
    public interface IInitializableService : IService
    {
        /// <summary>
        /// Initializes this service.
        /// </summary>
        void Initialize();
    }
}