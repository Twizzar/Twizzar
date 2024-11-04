using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Ui.Parser
{
    /// <summary>
    /// Extension method for the <see cref="ParserExtensions.Parser{T}"/> class for parsing <see cref="IViToken"/>s.
    /// </summary>
    public static class TokenParserExtensions
    {
        #region members

        /// <summary>
        /// Consume all surrounding whitespaces. When there is no whitespace a the end, EOF is expected.
        /// </summary>
        /// <typeparam name="T">The inner type.</typeparam>
        /// <param name="parser">The inner parser.</param>
        /// <returns>A new parser function.</returns>
        [SuppressMessage(
            "StyleCop.CSharp.ReadabilityRules",
            "SA1118:Parameter should not span multiple lines",
            Justification = "Should not be triggerd")]
        public static ParserExtensions.Parser<T> WithSurroundingWhitespaces<T>(this ParserExtensions.Parser<T> parser)
            where T : IViToken
        {
            EnsureHelper.GetDefault.Parameter(parser, nameof(parser)).ThrowWhenNull();

            return i => (
                    from head in Consume.WhiteSpace.Many()(i)
                    from body in parser(head.OutputPoint)
                    from tail in Consume.OneOrMoreWhitespacesOrEOF(body.OutputPoint)
                    select ParseSuccess<T>.FromSpan(
                        head.ParsedSpan.Start,
                        tail.ParsedSpan.End,
                        (T)body.Value.With(// add the whitespaces to the token.
                            head.ParsedSpan.Start,
                            tail.ParsedSpan.End - head.ParsedSpan.Start,
                            head.Value.AsString() + body.Value.ContainingText + tail.Value.AsString())))
                .MapFailure(failure => failure.WithSpan(new ParserSpan(i, failure.Span.End)));
        }

        #endregion
    }
}