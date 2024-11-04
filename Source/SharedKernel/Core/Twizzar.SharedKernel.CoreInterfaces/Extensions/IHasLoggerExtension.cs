using System;
using System.Diagnostics.CodeAnalysis;

using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// Extenstion methods for IHasLogger.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    [ExcludeFromCodeCoverage]
    public static class IHasLoggerExtension
    {
        /// <summary>
        /// Log to the logger and return the logged exception.
        /// </summary>
        /// <param name="hasLogger">The instance which has the logger.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="logLevel">Optional log level default is <see cref="LogLevel.Error"/>.</param>
        /// <returns>The exception.</returns>
        public static Exception LogAndReturn(this IHasLogger hasLogger, Exception exception, LogLevel logLevel = LogLevel.Error)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(hasLogger, nameof(hasLogger))
                .Parameter(exception, nameof(exception))
                .ThrowWhenNull();

            hasLogger.Log(exception, logLevel);
            return exception;
        }
    }
}
