using System;
using System.Collections.Generic;
using System.Linq;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Design.Ui.Parser.SyntaxTree.Literals
{
    /// <inheritdoc cref="IViCharToken" />
    public class ViCharToken : ViToken, IViCharToken
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViCharToken"/> class.
        /// </summary>
        /// <param name="start">The start index.</param>
        /// <param name="length">The length of the token.</param>
        /// <param name="containingText">The containing text with the quotes and white space.</param>
        /// <param name="character">The character surrounded by the quotes.</param>
        private ViCharToken(int start, int length, string containingText, char character)
            : base(start, length, containingText)
        {
            this.Character = character;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public char Character { get; }

        #endregion

        #region members

        /// <summary>
        /// Crate a <see cref="ViCharToken"/> where the containingText start with a quotes and ends with a quotes.
        /// </summary>
        /// <param name="start">The start index.</param>
        /// <param name="containingText">The containing text with the quotes.</param>
        /// <returns>A new instance of <see cref="ViCharToken"/>.</returns>
        public static ViCharToken CreateWithoutWhitespaces(int start, string containingText)
        {
            ViCommon.EnsureHelper.EnsureHelper.GetDefault
                .Parameter(containingText, nameof(containingText))
                .IsNotNull()
                .IsNotEmpty()
                .IsTrue(s => (s.Length == 4 && s[1] == '\\') || s.Length == 3, "The containingText needs to have a length of 3")
                .ThrowOnFailure();

            return new ViCharToken(
                start,
                containingText.Length,
                containingText,
                containingText.Substring(1, containingText.Length - 2)[0]);
        }

        /// <summary>
        /// Create a new token.
        /// </summary>
        /// <param name="start">The start of the span.</param>
        /// <param name="end">The end of the span. (exclusive).</param>
        /// <returns>A new instance of <see cref="ViCharToken"/>.</returns>
        public static ViCharToken Create(ParserPoint start, ParserPoint end)
        {
            ViCommon.EnsureHelper.EnsureHelper.GetDefault
                .Many()
                .Parameter(start, nameof(start))
                .Parameter(end, nameof(end))
                .ThrowWhenNull();

            if (end - start != 3)
            {
                throw new ArgumentException(
                    "The start and end should be exactly 3 character separated",
                    nameof(start));
            }

            return CreateWithoutWhitespaces(
                start.Position,
                ParserPoint.GetContent(start, end));
        }

        /// <inheritdoc />
        public override IViToken With(int start, int length, string containingText) =>
            new ViCharToken(start, length, containingText, this.Character);

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() =>
            base.GetEqualityComponents().Append(this.Character);

        #endregion
    }
}