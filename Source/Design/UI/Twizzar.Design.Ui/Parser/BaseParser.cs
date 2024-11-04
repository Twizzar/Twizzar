using Twizzar.Design.Ui.Interfaces.Parser;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.Functional.Monads.ResultMonad;
using ParserResult =
    ViCommon.Functional.Monads.ResultMonad.IResult<Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.IViToken,
        Twizzar.SharedKernel.CoreInterfaces.FunctionalParser.ParseFailure>;

namespace Twizzar.Design.Ui.Parser
{
    /// <summary>
    /// Base parser class for all parsers.
    /// </summary>
    public abstract class BaseParser : IParser
    {
        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <summary>
        /// Gets a Parser which parses a base type value.
        /// </summary>
        protected abstract ParserExtensions.Parser<IViToken> Parser { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public ParserResult Parse(string text)
        {
            var input = ParserPoint.New(text);

            var result = ConsumeToken.EmptyToken
                .Or(ConsumeToken.UndefinedToken)
                .Or(this.Parser)(input);

            // do not support multiple tokens in single input
            if (result.IsSuccess && result.OutputPoint().HasNext)
            {
                return Result.Failure<IViToken, ParseFailure>(
                    new ParseFailure("Multiple Tokens not supported", new ParserSpan(input, text.Length)));
            }

            // unpack the value of the parser success
            // on failure return a failure till the end of the text.
            return result
                .MapSuccess(success => success.Value)
                .MapFailure(
                    failure => failure.WithSpan(
                        new ParserSpan(
                            failure.Span.Start,
                            text.Length - failure.Span.Start.Position)));
        }

        #endregion
    }
}