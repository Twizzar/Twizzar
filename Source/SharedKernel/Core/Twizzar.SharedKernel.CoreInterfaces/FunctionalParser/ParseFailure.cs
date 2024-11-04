using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.FunctionalParser
{
    /// <summary>
    /// Failure when a parse failed.
    /// </summary>
    public class ParseFailure : Failure
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseFailure"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="span">The span in the string where the failure occurred.</param>
        public ParseFailure(string message, ParserSpan span)
            : base(message)
        {
            this.Span = span;
            this.OutputPoint = span.End;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the position the parse is after the failure.
        /// </summary>
        public ParserPoint OutputPoint { get; }

        /// <summary>
        /// Gets the span where the failure occurred.
        /// </summary>
        public ParserSpan Span { get; }

        #endregion

        #region members

        /// <summary>
        /// Create a new failure with a different span.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <returns>A new instance of <see cref="ParseFailure"/>.</returns>
        public ParseFailure WithSpan(ParserSpan span) =>
            new(this.Message, span);

        #endregion
    }
}