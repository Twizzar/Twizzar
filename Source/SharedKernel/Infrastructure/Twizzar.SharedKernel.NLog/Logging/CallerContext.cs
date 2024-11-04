using System.Runtime.CompilerServices;

namespace Twizzar.SharedKernel.NLog.Logging
{
    /// <summary>
    /// Caller context for providing the calling class as context to the logger.
    /// </summary>
    public record CallerContext
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CallerContext"/> class.
        /// </summary>
        /// <param name="callerMemberName">The caller member name.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="line">The line.</param>
        private CallerContext(string callerMemberName, string filePath, int line)
        {
            this.CallerMemberName = callerMemberName;
            this.FilePath = filePath;
            this.Line = line;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the caller member name.
        /// </summary>
        public string CallerMemberName { get; }

        /// <summary>
        /// Gets the file path of the caller.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the line of the caller member.
        /// </summary>
        public int Line { get; }

        #endregion

        #region members

        /// <summary>
        /// Format the caller context to a string.
        /// </summary>
        /// <returns>Formatted string.</returns>
        public string Format() => LogContextFormatter.Format(this.CallerMemberName, this.FilePath, this.Line);

        /// <summary>
        /// Create a new caller context. The context is the calling class.
        /// </summary>
        /// <param name="callerMemberName">Member name, set by the constructor.</param>
        /// <param name="callerFilePath">File path, set by the constructor.</param>
        /// <param name="callerLineNumber">LineNumber, set by the constructor.</param>
        /// <returns>A new instance of <see cref="CallerContext"/>.</returns>
        public static CallerContext Create(
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0) =>
            new(callerMemberName, callerFilePath, callerLineNumber);

        #endregion
    }
}