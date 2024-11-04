using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Link;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree.Link;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.Parser
{
    /// <summary>
    /// A parser for parsing complex types.
    /// </summary>
    public class ComplexParser : BaseParser
    {
        #region properties

        /// <summary>
        /// Gets a parser for a complex type.
        /// </summary>
        public static ParserExtensions.Parser<IViToken> ParseToken =>
            ConsumeToken.DefaultToken
                .Or(TypeParser.Map(success => ViLinkToken.Create(Maybe.Some(success.Value), Maybe.None())));

        /// <inheritdoc />
        protected override ParserExtensions.Parser<IViToken> Parser => ParseToken;

        /// <summary>
        /// Gets the type parser.
        /// </summary>
        protected static ParserExtensions.Parser<IViTypeToken> TypeParser =>

            // Consume every letter or digit or a point.
            TypeFullNameParser.FriendlyTypeParser

                // Map to ViTypeToken
                .Map(
                    t =>
                        ViTypeToken.CreateWithoutWhitespaces(t.ParsedSpan, t.Value.ContainingText, t.Value))

                // Consume all optional surrounding whitespaces.
                .WithSurroundingWhitespaces();

        #endregion
    }
}