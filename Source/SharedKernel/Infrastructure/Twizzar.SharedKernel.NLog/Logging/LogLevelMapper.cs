using LogLevel = Twizzar.SharedKernel.NLog.Interfaces.LogLevel;
using NLogLevel = NLog.LogLevel;

namespace Twizzar.SharedKernel.NLog.Logging
{
    /// <summary>
    /// Maps the NLog log level to internal log level.
    /// </summary>
    public static class LogLevelMapper
    {
        /// <summary>
        /// Maps the log level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>The NLog.LogLevel which wil be mapped to.</returns>
        public static NLogLevel MapLogLevel(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => NLogLevel.Trace,
                LogLevel.Debug => NLogLevel.Debug,
                LogLevel.Info => NLogLevel.Info,
                LogLevel.Warn => NLogLevel.Warn,
                LogLevel.Error => NLogLevel.Error,
                LogLevel.Fatal => NLogLevel.Fatal,
                _ => NLogLevel.Fatal,
            };
        }
    }
}
