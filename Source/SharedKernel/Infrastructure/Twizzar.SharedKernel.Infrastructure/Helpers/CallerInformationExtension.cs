using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.EnsureHelper;

namespace Twizzar.SharedKernel.Infrastructure.Helpers
{
    /// <summary>
    /// <see cref="CallerInformation"/> extensions.
    /// </summary>
    public static class CallerInformationExtension
    {
        /// <summary>
        /// Convert the caller information to a log context for logging.
        /// </summary>
        /// <param name="self">The caller information.</param>
        /// <returns>A context string.</returns>
        public static string ToLogContext(this CallerInformation self) =>
            LogContextFormatter.Format(self.CallerMemberName, self.FilePath, self.Line);
    }
}