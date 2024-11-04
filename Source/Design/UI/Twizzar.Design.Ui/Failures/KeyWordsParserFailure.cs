using System.Diagnostics.CodeAnalysis;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Ui.Failures
{
    /// <summary>
    /// Failure when parsing a key word in the ui.
    /// </summary>
    /// <seealso cref="ViCommon.Functional.Monads.ResultMonad.Failure" />
    [ExcludeFromCodeCoverage]
    public class KeyWordsParserFailure : Failure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyWordsParserFailure"/> class.
        /// </summary>
        /// <param name="message">The failure message.</param>
        public KeyWordsParserFailure(string message)
            : base(message)
        {
        }
    }
}
