using Twizzar.SharedKernel.Infrastructure.Helpers;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.Factory;

using LogLevel = Twizzar.SharedKernel.NLog.Interfaces.LogLevel;

namespace Twizzar.SharedKernel.Infrastructure.Factory
{
    /// <summary>
    /// Factory for creating an <see cref="IEnsureHelperWithLoggingFactory"/> which logs on throw with the log level error.
    /// </summary>
    public class EnsureHelperWithLoggingFactory : EnsureHelperFactory, IEnsureHelperWithLoggingFactory
    {
        /// <inheritdoc />
        public IEnsureHelper Create(IHasLogger hasLogger)
        {
            var helper = new EnsureHelper();
            helper.RegisterOnThrowCallbacks((exception, information) =>
            {
                if (hasLogger?.Logger != null)
                {
                    hasLogger.Logger.Log(LogLevel.Error, exception);
                }
                else
                {
                    LoggerFactory.GetLogger(information.ToLogContext()).Log(exception);
                }
            });
            return helper;
        }
    }
}
