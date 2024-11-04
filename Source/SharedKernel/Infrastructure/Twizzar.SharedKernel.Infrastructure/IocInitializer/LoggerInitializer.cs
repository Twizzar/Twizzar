using System;
using Autofac;
using NLog.Layouts;
using NLog.Targets;
using Twizzar.SharedKernel.Infrastructure.AutofacModules;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;

namespace Twizzar.SharedKernel.Infrastructure.IocInitializer
{
    /// <summary>
    /// Initializes the logger.
    /// </summary>
    public static class LoggerInitializer
    {
        /// <summary>
        /// Initializes the logger.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="relativeFolder">For example <c>@"\twizzar\twizzarApi.log"</c>.</param>
        /// <param name="appName">The name of the app.</param>
        /// <param name="additionalTargets">Additional targets.</param>
        public static void Init(ContainerBuilder builder, string relativeFolder, string appName, params Target[] additionalTargets)
        {
            var logFile = Environment.ExpandEnvironmentVariables($"%AppData%{relativeFolder}");

            var configurationBuilder = new LoggerConfigurationBuilder()
#if DEBUG
                .AddTarget(new DebuggerTarget("Debug"))
#endif
                .WithMinLogLevel(LogLevel.Trace);

#if !NCRUNCH
            configurationBuilder.AddTarget(new FileTarget("logFile")
            {
                FileName = (Layout)logFile,
                ArchiveAboveSize = 8000000,
                MaxArchiveFiles = 2,
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${callsite:fileName=true}|${message:withexception=true}${newline}${stacktrace}",
            });
#endif

            foreach (var additionalTarget in additionalTargets)
            {
                configurationBuilder.AddTarget(additionalTarget);
            }

            LoggerFactory.SetConfig(configurationBuilder);

            builder.RegisterModule(new AutofacLoggingModule());
        }
    }
}