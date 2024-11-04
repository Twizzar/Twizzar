using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree.Literals;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;

namespace Twizzar.Design.Ui.Parser
{
    /// <summary>
    /// Parser for parsing booleans.
    /// </summary>
    public class BoolParser : BaseTypeParser
    {
        #region properties

        /// <summary>
        /// Gets a parser for a bool.
        /// </summary>
        public static ParserExtensions.Parser<IViToken> ParseToken => TrueParser.Or(FalseParser);

        /// <inheritdoc />
        protected override ParserExtensions.Parser<IViToken> ValueParser => ParseToken;

        private static ParserExtensions.Parser<IViToken> TrueParser =>
            Consume.String("true")
                .TryConvert<string, bool>(bool.TryParse)
                .Map(MapToViBoolToken)
                .WithSurroundingWhitespaces();

        private static ParserExtensions.Parser<IViToken> FalseParser =>
            Consume.String("false")
                .TryConvert<string, bool>(bool.TryParse)
                .Map(MapToViBoolToken)
                .WithSurroundingWhitespaces();

        #endregion

        #region members

        private static IParseSuccess<ViToken> MapToViBoolToken(IParseSuccess<bool> success) =>
            success.WithValue(
                new ViBoolToken(
                    success.ParsedSpan.Start,
                    success.ParsedSpan.Length,
                    success.ParsedSpan.Content,
                    success.Value));

        #endregion
    }
}