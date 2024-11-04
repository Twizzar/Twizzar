using System;
using System.IO;

using NLog;
using NLog.Config;

using ILogger = Twizzar.SharedKernel.NLog.Interfaces.ILogger;

namespace Twizzar.SharedKernel.NLog.Logging
{
    /// <summary>
    /// The class can be used to create loggers.
    /// </summary>
    public static class LoggerFactory
    {
        /// <summary>
        /// Indicates if the logger configuration is already established and therefore it is only done once.
        /// </summary>
        private static bool _isLoggerConfigured = false;

        /// <summary>
        /// Sets the logger configuration for the factory.
        /// </summary>
        /// <param name="builder">The configured builder instance.</param>
        public static void SetConfig(LoggerConfigurationBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var config = builder.Build();
            LogManager.Configuration = config;
            _isLoggerConfigured = true;
        }

        /// <summary>
        /// Sets the logging configuration by config file.
        /// </summary>
        /// <param name="pathToConfig">The path to the NLog config file. if none is provided .Logging\NLog.config will set as path.</param>
        public static void SetConfig(string pathToConfig)
        {
            if (!File.Exists(pathToConfig))
            {
                throw new ArgumentException("Config file not found:" + pathToConfig);
            }

            var config = new XmlLoggingConfiguration(pathToConfig, true);
            if (config.InitializeSucceeded == false)
            {
                throw new ArgumentException("Invalid logger configuration");
            }

            LogManager.Configuration = config;
            _isLoggerConfigured = true;
        }

        /// <summary>
        /// Create a logger instance with the given type.
        /// </summary>
        /// <param name="type">The Logger type.</param>
        /// <returns>Returns a logger for the given type.</returns>
        public static ILogger GetLogger(Type type) =>
            GetLogger(type?.ToString());

        /// <summary>
        /// Create a logger instance with the given type.
        /// </summary>
        /// <typeparam name="T">The logger type.</typeparam>
        /// <returns>Returns a logger for the given type.</returns>
        public static ILogger GetLogger<T>() =>
            GetLogger(typeof(T));

        /// <summary>
        /// Create a logger instance with the given type.
        /// </summary>
        /// <param name="context">The Logger context.</param>
        /// <returns>Returns a logger for the given context.</returns>
        public static ILogger GetLogger(string context)
        {
            if (!_isLoggerConfigured)
            {
                throw new InvalidOperationException("Logger is not yet configured, please first call method SetConfig.");
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return new NLogLoggerWrapper(context);
        }
    }
}