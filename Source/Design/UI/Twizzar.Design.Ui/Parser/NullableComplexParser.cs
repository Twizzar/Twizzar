using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;

namespace Twizzar.Design.Ui.Parser
{
    /// <summary>
    /// Complex parser which allows null values.
    /// </summary>
    public class NullableComplexParser : ComplexParser
    {
        /// <inheritdoc />
        protected override ParserExtensions.Parser<IViToken> Parser =>
            ConsumeToken.NullToken
                .Or(base.Parser);
    }
}
