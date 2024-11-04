using System.Collections.Generic;

using NLog.Config;
using NLog.Targets;

using Twizzar.SharedKernel.NLog.Interfaces;

using NLogLevel = NLog.LogLevel;

namespace Twizzar.SharedKernel.NLog.Logging
{
    /// <summary>
    /// The logger configuration builder to specify the used configuration.
    /// </summary>
    public class LoggerConfigurationBuilder
    {
        #region fields

        private readonly NLogLevel _maxLogLevel = NLogLevel.Fatal;
        private readonly List<Target> _targets = new();
        private NLogLevel _minLogLevel = NLogLevel.Trace;
        private bool _consoleLogging;
        private bool _fileLogging;
        private string _logFilePath;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerConfigurationBuilder"/> class.
        /// Console logging will be activated as default and trace as log level.
        /// </summary>
        public LoggerConfigurationBuilder()
        {
            this._consoleLogging = true;
            this._fileLogging = false;
            this._logFilePath = string.Empty;
        }

        #endregion

        #region members

        /// <summary>
        /// Configures the file logging.
        /// </summary>
        /// <param name="filePath">The path to the log file.</param>
        /// <returns>Builder which can be configured further.</returns>
        public LoggerConfigurationBuilder WithFileLogging(string filePath)
        {
            this._fileLogging = true;
            this._logFilePath = filePath;
            return this;
        }

        /// <summary>
        /// Deactivates file logging.
        /// </summary>
        /// <returns>Builder which can be configured further.</returns>
        public LoggerConfigurationBuilder WithoutFileLogging()
        {
            this._fileLogging = false;
            this._logFilePath = string.Empty;
            return this;
        }

        /// <summary>
        /// Deactivates console logging.
        /// </summary>
        /// <returns>Builder which can be configured further.</returns>
        public LoggerConfigurationBuilder WithoutConsoleLogging()
        {
            this._consoleLogging = false;
            return this;
        }

        /// <summary>
        /// Defined the log level of the configuration.
        /// </summary>
        /// <param name="logLevel">The log level parameter.</param>
        /// <returns>Builder which can be configured further.</returns>
        public LoggerConfigurationBuilder WithMinLogLevel(LogLevel logLevel)
        {
            this._minLogLevel = LogLevelMapper.MapLogLevel(logLevel);
            return this;
        }

        /// <summary>
        /// Add a target to the config.
        /// </summary>
        /// <param name="target">The logger target.</param>
        /// <returns>Builder which can be configured further.</returns>
        public LoggerConfigurationBuilder AddTarget(Target target)
        {
            this._targets.Add(target);
            return this;
        }

        /// <summary>
        /// Build the LoggingConfiguration instance.
        /// </summary>
        /// <returns>The NLog Logging configuration.</returns>
        internal LoggingConfiguration Build()
        {
            var config = new LoggingConfiguration();

            // Targets where to log to: File and Console
            if (this._fileLogging)
            {
                var logFile = new FileTarget("logFile") { FileName = this._logFilePath };
                config.AddRule(this._minLogLevel, this._maxLogLevel, logFile);
            }

            if (this._consoleLogging)
            {
                var logConsole = new ConsoleTarget("logConsole");
                config.AddRule(this._minLogLevel, this._maxLogLevel, logConsole);
            }

            foreach (var target in this._targets)
            {
                config.AddRule(this._minLogLevel, this._maxLogLevel, target);
            }

            return config;
        }

        #endregion
    }
}