using System;
using System.Linq;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Link;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Design.Ui.Parser.SyntaxTree.Link
{
    /// <summary>
    /// Token for a type.
    /// </summary>
    public class ViTypeToken : ViToken, IViTypeToken
    {
        #region ctors

        private ViTypeToken(int start, int length, string containingText, ITypeFullNameToken token)
            : base(start, length, containingText)
        {
            this.TypeFullNameToken = token;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ITypeFullNameToken TypeFullNameToken { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public override IViToken With(int start, int length, string containingText) =>
            new ViTypeToken(start, length, containingText, this.TypeFullNameToken);

        /// <summary>
        /// Create a new <see cref="ViTypeToken"/> without any whitespaces.
        /// </summary>
        /// <param name="span">The span of the token.</param>
        /// <param name="containingText">The containing text (also the typeFullName).</param>
        /// <param name="token">The type full name token.</param>
        /// <returns>A new instance of <see cref="ViTypeToken"/>.</returns>
        /// <exception cref="ArgumentException">When containingText contains a whitespace.</exception>
        public static ViTypeToken CreateWithoutWhitespaces(
            ParserSpan span,
            string containingText,
            ITypeFullNameToken token)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(span, nameof(span))
                .Parameter(containingText, nameof(containingText))
                .ThrowWhenNull();

            if (char.IsWhiteSpace(containingText.First()) || char.IsWhiteSpace(containingText.Last()))
            {
                throw new ArgumentException(
                    "the containing text should not contain any whitespaces",
                    nameof(containingText));
            }

            return new ViTypeToken(span.Start, span.Length, containingText, token);
        }

        /// <summary>
        /// Create a token for the validator.
        /// </summary>
        /// <param name="token">The type full name token.</param>
        /// <returns>A new instance of <see cref="ViTypeToken"/>.</returns>
        public static ViTypeToken Create(ITypeFullNameToken token) =>
            new(0, 0, token.ToFriendlyCSharpTypeName(), token);

        /// <inheritdoc />
        public IViTypeToken WithToken(ITypeFullNameToken typeFullNameToken) =>
            new ViTypeToken(this.Start, this.Length, this.ContainingText, typeFullNameToken);

        #endregion
    }
}