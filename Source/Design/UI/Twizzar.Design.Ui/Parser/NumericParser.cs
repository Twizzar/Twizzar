using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree.Literals;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;

namespace Twizzar.Design.Ui.Parser
{
    /// <summary>
    /// Parser for parsing a single numeric.
    /// </summary>
    public class NumericParser : BaseTypeParser
    {
        #region properties

        /// <summary>
        /// Gets a parser for a numeric type.
        /// </summary>
        public static ParserExtensions.Parser<IViToken> ParseToken =>
            Consume.Numeric
                .And(
                    Consume.Char('f', 'F', 'd', 'D', 'm', 'M').Optional(),
                    (numeric, suffix) => new NumericWithSuffix(numeric.AsString(), suffix))
                .Map(MapToViNumericToken)
                .WithSurroundingWhitespaces();

        /// <inheritdoc />
        protected override ParserExtensions.Parser<IViToken> ValueParser => ParseToken;

        #endregion

        #region members

        private static IParseSuccess<IViToken> MapToViNumericToken(IParseSuccess<NumericWithSuffix> success) =>
            success.WithValue(
                ViNumericToken.Create(success.ParsedSpan, success.Value));

        #endregion
    }
}