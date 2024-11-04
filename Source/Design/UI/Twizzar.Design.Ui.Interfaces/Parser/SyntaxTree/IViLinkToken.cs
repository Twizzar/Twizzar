using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Link;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree
{
    /// <summary>
    /// Gets the token representing a link with type and name.
    /// </summary>
    public interface IViLinkToken : IViToken
    {
        #region properties

        /// <summary>
        /// Gets the type token.
        /// </summary>
        Maybe<IViTypeToken> TypeToken { get; }

        /// <summary>
        /// Create a new <see cref="IViLinkToken"/> with a new type token.
        /// </summary>
        /// <param name="typeToken"></param>
        /// <returns>A new instance of <see cref="IViLinkToken"/>.</returns>
        IViLinkToken WithTypeToken(IViTypeToken typeToken);

        #endregion
    }
}