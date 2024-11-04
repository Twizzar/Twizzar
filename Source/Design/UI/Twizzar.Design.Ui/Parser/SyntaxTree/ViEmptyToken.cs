using System;
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;

namespace Twizzar.Design.Ui.Parser.SyntaxTree
{
    /// <summary>
    /// An empty token for an empty string.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ViEmptyToken : ViToken
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViEmptyToken"/> class.
        /// </summary>
        /// <param name="start">The start position.</param>
        /// <param name="length">The length of the empty token.</param>
        /// <param name="containingText">The containing text.</param>
        public ViEmptyToken(int start = 0, int length = 0, string containingText = "")
            : base(start, length, containingText)
        {
        }

        #endregion

        #region members

        /// <inheritdoc />
        public override IViToken With(int start, int length, string containingText) =>
            throw new NotImplementedException();

        #endregion
    }
}