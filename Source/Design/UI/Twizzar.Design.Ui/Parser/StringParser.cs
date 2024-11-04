using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree.Literals;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;

namespace Twizzar.Design.Ui.Parser
{
    /// <summary>
    /// Parser for parsing a single string.
    /// </summary>
    public class StringParser : BaseTypeParser
    {
        /// <summary>
        /// Gets a parser for a string.
        /// </summary>
        public static ParserExtensions.Parser<IViToken> ParseToken =>
            Consume.StringToken()
                .Map(
                    success =>
                        ViStringToken.CreateWithoutWhitespaces(
                            success.ParsedSpan.Start.Position,
                            success.ParsedSpan.Length,
                            success.Value.AsString()))
                .WithSurroundingWhitespaces();

        /// <summary>
        /// Gets a Parser which parses characters in double quotes like "AnythingExceptDoubleQuotes".
        /// </summary>
        protected override ParserExtensions.Parser<IViToken> ValueParser => ParseToken;
    }
}