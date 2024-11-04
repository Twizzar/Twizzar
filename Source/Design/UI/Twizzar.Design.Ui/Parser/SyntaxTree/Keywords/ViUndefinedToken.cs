using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Keywords;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;

namespace Twizzar.Design.Ui.Parser.SyntaxTree.Keywords
{
    /// <inheritdoc cref="IViUndefinedKeywordToken" />
    public class ViUndefinedToken : ViToken, IViUndefinedKeywordToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViUndefinedToken"/> class.
        /// </summary>
        /// <param name="start">The start index.</param>
        /// <param name="length">The length of the token.</param>
        /// <param name="containingText">The containing text.</param>
        public ViUndefinedToken(int start, int length, string containingText)
            : base(start, length, containingText)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViUndefinedToken"/> class.
        /// </summary>
        /// <param name="span">The span of the token.</param>
        /// <param name="containingText">The containing text.</param>
        public ViUndefinedToken(ParserSpan span, string containingText)
            : base(span.Start, span.Length, containingText)
        {
        }

        #region Overrides of ViToken

        /// <inheritdoc />
        public override IViToken With(int start, int length, string containingText) =>
            new ViUndefinedToken(start, length, containingText);

        #endregion
    }
}