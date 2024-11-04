using System;
using System.Linq;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Link;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Design.Ui.Parser.SyntaxTree.Link
{
    /// <summary>
    /// Token for a link name.
    /// </summary>
    public class ViLinkNameToken : ViToken, IViLinkNameToken
    {
        #region ctors

        private ViLinkNameToken(int start, int length, string containingText, string name)
            : base(start, length, containingText)
        {
            this.Name = name;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public string Name { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public override IViToken With(int start, int length, string containingText) =>
            new ViLinkNameToken(start, length, containingText, this.Name);

        /// <summary>
        /// Create a new <see cref="ViLinkNameToken"/> without any whitespaces.
        /// </summary>
        /// <param name="span">The span of the token.</param>
        /// <param name="containingText">The containing text. Should start with # and should not contain any whitespaces.</param>
        /// <returns>A new instance of <see cref="ViLinkNameToken"/>.</returns>
        /// <exception cref="ArgumentException">When containingText does not start with #.</exception>
        /// <exception cref="ArgumentException">When containingText contains a whitespace.</exception>
        public static ViLinkNameToken CreateWithoutWhitespace(ParserSpan span, string containingText)
        {
            ViCommon.EnsureHelper.EnsureHelper.GetDefault.Many()
                .Parameter(span, nameof(span))
                .Parameter(containingText, nameof(containingText))
                .ThrowWhenNull();

            ViCommon.EnsureHelper.EnsureHelper.GetDefault.Parameter(containingText, nameof(containingText))
                .IsTrue(s => s.StartsWith("#"), "text should start with #")
                .IsTrue(s => !s.Any(char.IsWhiteSpace), "text should not contain any whitespaces")
                .ThrowOnFailure();

            return new ViLinkNameToken(
                span.Start,
                span.Length,
                containingText,
                containingText.TrimStart('#'));
        }
        #endregion
    }
}