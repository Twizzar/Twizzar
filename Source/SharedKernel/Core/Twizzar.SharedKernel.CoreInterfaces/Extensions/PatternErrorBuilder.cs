using System;
using System.Diagnostics.CodeAnalysis;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// Class for building an <see cref="ArgumentOutOfRangeException"/> exception for patter match errors.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PatternErrorBuilder
    {
        private readonly string _caseName;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternErrorBuilder"/> class.
        /// </summary>
        /// <param name="caseName">The name of the case variable of the switch.</param>
        public PatternErrorBuilder(string caseName)
        {
            this._caseName = caseName;
        }

        /// <summary>
        /// Create a new builder.
        /// </summary>
        /// <param name="caseName">The name of the case variable of the switch.</param>
        /// <returns>A new instance of <see cref="PatternErrorBuilder"/>.</returns>
        public static PatternErrorBuilder PatternCase(string caseName) =>
            new(caseName);

        /// <summary>
        /// Returns a <see cref="ArgumentOutOfRangeException"/> with a meaningful message.
        /// </summary>
        /// <param name="matches">The valid matches.</param>
        /// <returns>A new <see cref="ArgumentOutOfRangeException"/>.</returns>
        public ArgumentOutOfRangeException IsNotOneOf(params string[] matches) =>
            new(
                $"Match failed: {this._caseName} is not one of: {string.Join(",", matches)}");
    }
}
