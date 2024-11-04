namespace Twizzar.SharedKernel.NLog.Logging
{
    /// <summary>
    /// Formatter for formatting the context in a log format.
    /// </summary>
    public static class LogContextFormatter
    {
        /// <summary>
        /// Format the caller information.
        /// </summary>
        /// <param name="methodName">The method name.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns>A formatted string.</returns>
        public static string Format(string methodName, string filePath, int lineNumber) =>
            $"Method {methodName} in {filePath} at line {lineNumber}";
    }
}