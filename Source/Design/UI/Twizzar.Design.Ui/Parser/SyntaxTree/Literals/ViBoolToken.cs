using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;

namespace Twizzar.Design.Ui.Parser.SyntaxTree.Literals
{
    /// <inheritdoc cref="IViBoolToken" />
    public class ViBoolToken : ViToken, IViBoolToken
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViBoolToken"/> class.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="containingText"></param>
        /// <param name="boolean"></param>
        public ViBoolToken(int start, int length, string containingText, bool boolean)
            : base(start, length, containingText)
        {
            this.Boolean = boolean;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public bool Boolean { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public override IViToken With(int start, int length, string containingText) =>
            new ViBoolToken(start, length, containingText, this.Boolean);

        #endregion
    }
}