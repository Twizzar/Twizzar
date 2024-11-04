using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.Factory;

namespace Twizzar.SharedKernel.Infrastructure.Factory
{
    /// <summary>
    /// Factory for creating <see cref="IEnsureHelper"/>.
    /// </summary>
    public interface IEnsureHelperWithLoggingFactory : IEnsureHelperFactory
    {
        /// <summary>
        /// Create a new instance of <see cref="IEnsureHelper"/> and adds callbacks for logging.
        /// </summary>
        /// <param name="hasLogger">The <see cref="IHasLogger"/> instance.</param>
        /// <returns>An <see cref="IEnsureHelper"/>.</returns>
        IEnsureHelper Create(IHasLogger hasLogger);
    }
}