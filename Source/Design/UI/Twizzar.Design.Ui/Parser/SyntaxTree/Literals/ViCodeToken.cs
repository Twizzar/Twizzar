using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;

namespace Twizzar.Design.Ui.Parser.SyntaxTree.Literals
{
    /// <inheritdoc cref="IViCodeToken" />
    public class ViCodeToken : ViToken, IViCodeToken
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViCodeToken"/> class.
        /// </summary>
        /// <param name="start">The start index.</param>
        /// <param name="length">The length of the token.</param>
        /// <param name="containingText">The containing text.</param>
        public ViCodeToken(int start, int length, string containingText)
            : base(start, length, containingText)
        {
        }

        #endregion

        #region members

        /// <inheritdoc />
        public override IViToken With(int start, int length, string containingText) =>
            new ViCtorToken(start, length, containingText);

        #endregion
    }
}
