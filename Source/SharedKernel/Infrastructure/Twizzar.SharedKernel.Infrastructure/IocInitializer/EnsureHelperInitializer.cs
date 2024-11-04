using Autofac;
using Twizzar.SharedKernel.Infrastructure.AutofacModules;
using Twizzar.SharedKernel.Infrastructure.Factory;
using Twizzar.SharedKernel.Infrastructure.Helpers;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.EnsureHelper;

namespace Twizzar.SharedKernel.Infrastructure.IocInitializer
{
    /// <summary>
    /// Initializes the ensure helper.
    /// </summary>
    public static class EnsureHelperInitializer
    {
        #region members

        /// <summary>
        /// Initializes the ensure helper.
        /// </summary>
        /// <param name="builder"></param>
        public static void Init(ContainerBuilder builder)
        {
            var defaultEnsureHelper = new EnsureHelper();

            defaultEnsureHelper.RegisterOnThrowCallbacks(
                (exception, information) =>
                    LoggerFactory
                        .GetLogger(information.ToLogContext())
                        .Log(LogLevel.Error, exception));

            EnsureHelper.ConfigureDefault(defaultEnsureHelper);
            builder.RegisterModule(new AutofacEnsureModule(new EnsureHelperWithLoggingFactory()));
        }

        #endregion
    }
}