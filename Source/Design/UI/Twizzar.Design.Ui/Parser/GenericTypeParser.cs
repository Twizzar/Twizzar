using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;

namespace Twizzar.Design.Ui.Parser;

/// <summary>
/// A parser for parsing generic method return types.
/// </summary>
public class GenericTypeParser : BaseParser
{
    #region properties

    /// <inheritdoc />
    protected override ParserExtensions.Parser<IViToken> Parser =>
        ConsumeToken.NullToken
            .Or(ConsumeToken.NullToken) // Or a NullToken
            .Or(BoolParser.ParseToken)
            .Or(CharParser.ParseToken)
            .Or(NumericParser.ParseToken)
            .Or(StringParser.ParseToken)
            .Or(ComplexParser.ParseToken);

    #endregion
}