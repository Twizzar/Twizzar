using System;

using Twizzar.SharedKernel.NLog.Interfaces;

namespace Twizzar.SharedKernel.NLog.Logging
{
    /// <summary>
    /// Static methods for logging.
    /// </summary>
    public static class ViLog
    {
        #region members

        /// <summary>
        /// Writes the diagnostic message and the exception at the specified level.
        /// </summary>
        /// <param name="self">The instance calling Log.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="logLevel">The log level.</param>
        /// <typeparam name="T">The type of the instance.</typeparam>
        public static void Log<T>(this T self, string message, LogLevel logLevel = LogLevel.Info)
        {
            Get(self).Log(logLevel, message);
        }

        /// <summary>
        /// Writes the diagnostic message and the exception at the specified level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="logLevel">The log level.</param>
        /// <typeparam name="T">The type of the instance.</typeparam>
        public static void Log<T>(string message, LogLevel logLevel = LogLevel.Info)
        {
            Get<T>().Log(logLevel, message);
        }

        /// <summary>
        /// Writes the diagnostic message and the exception at the specified level.
        /// </summary>
        /// <param name="callerContext">Use <see cref="CallerContext.Create"/>.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="logLevel">The log level.</param>
        public static void Log(
            CallerContext callerContext,
            string message,
            LogLevel logLevel = LogLevel.Info)
        {
            Get(callerContext.Format()).Log(logLevel, message);
        }

        /// <summary>
        /// Writes the diagnostic message and the exception at the specified level.
        /// </summary>
        /// <param name="self">The instance calling Log.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="logLevel">The log level.</param>
        /// <typeparam name="T">The type of the instance.</typeparam>
        public static void Log<T>(this T self, string message, Exception exception, LogLevel logLevel = LogLevel.Error)
        {
            Get(self).Log(logLevel, message, exception);
        }

        /// <summary>
        /// Writes the diagnostic message and the exception at the specified level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="logLevel">The log level.</param>
        /// <typeparam name="T">The type of the calling class.</typeparam>
        public static void Log<T>(string message, Exception exception, LogLevel logLevel = LogLevel.Error)
        {
            Get<T>().Log(logLevel, message, exception);
        }

        /// <summary>
        /// Writes the diagnostic message and the exception at the specified level.
        /// </summary>
        /// <param name="callerContext">Use <see cref="CallerContext.Create"/>.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="logLevel">The log level.</param>
        public static void Log(
            CallerContext callerContext,
            string message,
            Exception exception,
            LogLevel logLevel = LogLevel.Error)
        {
            Get(callerContext.Format()).Log(logLevel, message, exception);
        }

        /// <summary>
        /// Writes the diagnostic message and the exception at the specified level.
        /// </summary>
        /// <param name="self">The instance calling Log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="logLevel">The log level.</param>
        /// <typeparam name="T">The type of the calling class.</typeparam>
        public static void Log<T>(this T self, Exception exception, LogLevel logLevel = LogLevel.Error)
        {
            Get(self).Log(logLevel, exception);
        }

        /// <summary>
        /// Writes the diagnostic message and the exception at the specified level.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="logLevel">The log level.</param>
        /// <typeparam name="T">The type of the calling class.</typeparam>
        public static void Log<T>(Exception exception, LogLevel logLevel = LogLevel.Error)
        {
            Get<T>().Log(exception, logLevel);
        }

        /// <summary>
        /// Writes the diagnostic message and the exception at the specified level.
        /// </summary>
        /// <param name="callerContext">Use <see cref="CallerContext.Create"/>.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="logLevel">The log level.</param>
        public static void Log(CallerContext callerContext, Exception exception, LogLevel logLevel = LogLevel.Error)
        {
            Get(callerContext.Format()).Log(exception, logLevel);
        }

        private static ILogger Get<T>(T instance) =>
            instance is IHasLogger hasLogger && hasLogger?.Logger != null
                ? hasLogger.Logger
                : LoggerFactory.GetLogger<T>();

        private static ILogger Get<T>() => LoggerFactory.GetLogger<T>();

        private static ILogger Get(string context) => LoggerFactory.GetLogger(context);

        #endregion
    }
}