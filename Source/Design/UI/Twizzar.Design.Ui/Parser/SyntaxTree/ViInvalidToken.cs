using System.Collections.Generic;
using System.Linq;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;

namespace Twizzar.Design.Ui.Parser.SyntaxTree
{
    /// <summary>
    /// Token which represents a invalid text.
    /// </summary>
    public class ViInvalidToken : ViToken, IViInvalidToken
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViInvalidToken"/> class.
        /// </summary>
        /// <param name="start">The start of the token.</param>
        /// <param name="length">The length.</param>
        /// <param name="containingText">The containing text.</param>
        /// <param name="message">The message why this is invalid.</param>
        public ViInvalidToken(int start, int length, string containingText, string message)
            : base(start, length, containingText)
        {
            this.Message = message;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public string Message { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public override IViToken With(int start, int length, string containingText) =>
            new ViInvalidToken(start, length, containingText, this.Message);

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() =>
            base.GetEqualityComponents().Append(this.Message);

        #endregion
    }
}