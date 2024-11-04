using System.Collections.Generic;
using System.Linq;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;

namespace Twizzar.Design.Ui.Parser.SyntaxTree.Literals
{
    /// <inheritdoc cref="IViStringToken" />
    public class ViStringToken : ViToken, IViStringToken
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViStringToken"/> class.
        /// </summary>
        /// <param name="start">The start index.</param>
        /// <param name="length">The length of the token.</param>
        /// <param name="containingText">The containing text with the double quotes and white space.</param>
        /// <param name="text">The text surrounded by the double quotes.</param>
        private ViStringToken(int start, int length, string containingText, string text)
            : base(start, length, containingText)
        {
            this.Text = text;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public string Text { get; }

        #endregion

        #region members

        /// <summary>
        /// Crate a <see cref="ViStringToken"/> where the containingText start with a double quotes and ends with a double quotes.
        /// </summary>
        /// <param name="start">The start index.</param>
        /// <param name="length">The length of the token.</param>
        /// <param name="containingText">The containing text with the double quotes.</param>
        /// <returns>A new instance of <see cref="ViStringToken"/>.</returns>
        public static ViStringToken CreateWithoutWhitespaces(int start, int length, string containingText)
            => new(
                start,
                length,
                containingText,
                containingText.Substring(1, containingText.Length - 2));

        /// <summary>
        /// Create a new token.
        /// </summary>
        /// <param name="start">The start of the span.</param>
        /// <param name="end">The end of the span. (exclusive).</param>
        /// <returns>A new instance of <see cref="ViStringToken"/>.</returns>
        public static ViStringToken Create(ParserPoint start, ParserPoint end) =>
            CreateWithoutWhitespaces(
                start.Position,
                end.Position - start.Position,
                ParserPoint.GetContent(start, end));

        /// <inheritdoc />
        public override IViToken With(int start, int length, string containingText) =>
            new ViStringToken(start, length, containingText, this.Text);

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() =>
            base.GetEqualityComponents().Append(this.Text);

        #endregion
    }
}