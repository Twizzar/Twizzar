using System;

namespace Twizzar.SharedKernel.NLog.Interfaces
{
    /// <summary>
    /// ILogger Interface to use in vi projects.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Writes the diagnostic message at the specified level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message.</param>
        void Log(LogLevel logLevel, string message);

        /// <summary>
        /// Writes the diagnostic message at the specified level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="logLevel">The log level.</param>
        /// <param name="value">The value.</param>
        void Log<T>(LogLevel logLevel, T value);

        /// <summary>
        /// Writes the diagnostic message at the specified level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="messageFunc">The function returning the log message. This function only gets executed if the log level is enabled.</param>
        void Log(LogLevel logLevel, Func<string> messageFunc);

        /// <summary>
        /// Writes the diagnostic message and the exception at the specified level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        void Log(LogLevel logLevel, string message, Exception exception);

        /// <summary>
        /// Writes the diagnostic message and the exception at the specified level.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="logLevel">The log level.</param>
        /// <returns>The exception provided.</returns>
        Exception Log(Exception exception, LogLevel logLevel = LogLevel.Error);

        /// <summary>
        /// Gets a value indicating whether logging is enabled for the specified level.
        /// </summary>
        /// <param name="logLevel">Log level to be checked.</param>
        /// <returns>
        ///   <c>true</c> if the specified level is enabled; otherwise, <c>false</c>.
        /// </returns>
        bool IsEnabled(LogLevel logLevel);
    }
}
