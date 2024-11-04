using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Keywords;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;

namespace Twizzar.Design.Ui.Parser.SyntaxTree.Keywords
{
    /// <inheritdoc cref="IViNullKeywordToken" />
    public class ViNullKeywordToken : ViToken, IViNullKeywordToken
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViNullKeywordToken"/> class.
        /// </summary>
        /// <param name="span">The span of the token.</param>
        /// <param name="containingText">The containing text.</param>
        public ViNullKeywordToken(ParserSpan span, string containingText)
            : base(span.Start, span.Length, containingText)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViNullKeywordToken"/> class.
        /// </summary>
        /// <param name="start">The start index.</param>
        /// <param name="length">The length of the token.</param>
        /// <param name="containingText">The containing text.</param>
        public ViNullKeywordToken(int start, int length, string containingText)
            : base(start, length, containingText)
        {
        }

        #endregion

        #region members

        /// <inheritdoc />
        public override IViToken With(int start, int length, string containingText) =>
            new ViNullKeywordToken(start, length, containingText);

        #endregion
    }
}