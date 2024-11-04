using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree.Literals;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;

namespace Twizzar.Design.Ui.Parser
{
    /// <summary>
    /// Parser for parsing a single character.
    /// </summary>
    public class CharParser : BaseTypeParser
    {
        #region properties

        /// <summary>
        /// Gets a parser for a char.
        /// </summary>
        public static ParserExtensions.Parser<IViToken> ParseToken =>
            Consume.CharToken()
                .Map(
                    success =>
                        ViCharToken.CreateWithoutWhitespaces(
                            success.ParsedSpan.Start,
                            success.Value.AsString()))
                .WithSurroundingWhitespaces();

        /// <summary>
        /// Gets a Parser which parses characters in quotes like 'c'.
        /// </summary>
        protected override ParserExtensions.Parser<IViToken> ValueParser => ParseToken;

        #endregion
    }
}