using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Link;
using ViCommon.Functional.Monads.MaybeMonad;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;

namespace Twizzar.Design.Ui.Parser.SyntaxTree
{
    /// <summary>
    /// Token for a link.
    /// </summary>
    public class ViLinkToken : ViToken, IViLinkToken
    {
        #region ctors

        private ViLinkToken(
            int start,
            int length,
            string containingText,
            Maybe<IViTypeToken> typeToken)
            : base(start, length, containingText)
        {
            this.TypeToken = typeToken;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public Maybe<IViTypeToken> TypeToken { get; }

        /// <inheritdoc />
        public IViLinkToken WithTypeToken(IViTypeToken typeToken) =>
            new ViLinkToken(
                this.Start,
                this.Length,
                typeToken.ContainingText,
                Some(typeToken));

        #endregion

        #region members

        /// <inheritdoc />
        public override IViToken With(int start, int length, string containingText) =>
            new ViLinkToken(start, length, containingText, this.TypeToken);

        /// <summary>
        /// Create a <see cref="ViLinkToken"/>.
        /// </summary>
        /// <param name="typeToken">The type token with whitespaces.</param>
        /// <param name="linkNameToken">The link name token with whitespaces.</param>
        /// <returns>A new instance of <see cref="ViLinkToken"/>.</returns>
        public static ViLinkToken Create(Maybe<IViTypeToken> typeToken, Maybe<IViLinkNameToken> linkNameToken)
        {
            var typeStart = typeToken.Map(token => token.Start);

            var start = typeStart.Match(
                i => i,
                () => linkNameToken.Map(token => token.Start).SomeOrProvided(0));

            var length = typeToken.Match(token => token.Length, 0) +
                         linkNameToken.Match(token => token.Length, 0);
            var containingText =
                typeToken.Match(token => token.ContainingText, string.Empty)
                + linkNameToken.Match(token => token.ContainingText, string.Empty);

            return new ViLinkToken(start, length, containingText, typeToken);
        }
        #endregion
    }
}