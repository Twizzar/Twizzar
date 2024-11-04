using System;

using NLog;

using ILogger = Twizzar.SharedKernel.NLog.Interfaces.ILogger;
using LogLevel = Twizzar.SharedKernel.NLog.Interfaces.LogLevel;
using NILogger = NLog.ILogger;

namespace Twizzar.SharedKernel.NLog.Logging
{
    /// <summary>
    /// The implementation of the ILogger which wrapped the NLog logger
    /// infrastructure to the vi-Logger.
    /// </summary>
    public class NLogLoggerWrapper : ILogger
    {
        #region fields

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly NILogger _logger;

        #endregion

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogLoggerWrapper" /> class.
        /// </summary>
        /// <param name="context">The logger context.</param>
        public NLogLoggerWrapper(string context)
        {
            this._logger = LogManager.GetLogger(context);
        }

        #endregion

        #region methods

        /// <summary>
        /// Writes the diagnostic message and the exception at the specified level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        public void Log(LogLevel logLevel, string message, Exception exception)
        {
            this._logger.Log(LogLevelMapper.MapLogLevel(logLevel), exception, message, null);
        }

        /// <summary>
        /// Writes the diagnostic message at the specified level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message.</param>
        public void Log(LogLevel logLevel, string message)
        {
            this._logger.Log(LogLevelMapper.MapLogLevel(logLevel), message);
        }

        /// <inheritdoc />
        public Exception Log(Exception exception, LogLevel logLevel = LogLevel.Error)
        {
            this._logger.Log(LogLevelMapper.MapLogLevel(logLevel), exception, exception?.Message, null);
            return exception;
        }

        /// <summary>
        /// Writes the diagnostic message at the specified level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="logLevel">The log level.</param>
        /// <param name="value">The value.</param>
        public void Log<T>(LogLevel logLevel, T value)
        {
            this._logger.Log(LogLevelMapper.MapLogLevel(logLevel), value);
        }

        /// <summary>
        /// Writes the diagnostic message at the specified level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="messageFunc">The function returning the log message. This function only gets executed if the log level is enabled.</param>
        public void Log(LogLevel logLevel, Func<string> messageFunc)
        {
            if (this.IsEnabled(logLevel) && messageFunc != null)
            {
                this.Log(logLevel, messageFunc());
            }
        }

        /// <summary>
        /// Gets a value indicating whether logging is enabled for the specified level.
        /// </summary>
        /// <param name="logLevel">Log level to be checked.</param>
        /// <returns>
        ///   <c>true</c> if the specified level is enabled; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return this._logger.IsEnabled(LogLevelMapper.MapLogLevel(logLevel));
        }

        #endregion
    }
}
