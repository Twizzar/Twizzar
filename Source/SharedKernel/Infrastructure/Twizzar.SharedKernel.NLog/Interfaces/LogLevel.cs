namespace Twizzar.SharedKernel.NLog.Interfaces
{
    /// <summary>
    /// The log levels defined for the ILogger.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// trace.
        /// </summary>
        Trace = 0,

        /// <summary>
        /// debug.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// information.
        /// </summary>
        Info = 2,

        /// <summary>
        /// warning.
        /// </summary>
        Warn = 3,

        /// <summary>
        /// error.
        /// </summary>
        Error = 4,

        /// <summary>
        /// fatal.
        /// </summary>
        Fatal = 5,
    }
}
