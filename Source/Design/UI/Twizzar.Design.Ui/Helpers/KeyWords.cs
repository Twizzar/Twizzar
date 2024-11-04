using System.Diagnostics.CodeAnalysis;

namespace Twizzar.Design.Ui.Helpers
{
    /// <summary>
    /// Keywords for transforming from ui to backend.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class KeyWords
    {
        /// <summary>
        /// Unique keyword.
        /// </summary>
        public const string Unique = "unique";

        /// <summary>
        /// Default keyword.
        /// </summary>
        public const string Default = "default";

        /// <summary>
        /// Undefined keyword.
        /// </summary>
        public const string Undefined = "undefined";

        /// <summary>
        /// Null keyword.
        /// </summary>
        public const string Null = "null";

        /// <summary>
        /// Gets an array containing all keywords.
        /// </summary>
        public static string[] All =>
            new[]
            {
                Unique,
                Default,
                Undefined,
                Null,
            };
    }
}
