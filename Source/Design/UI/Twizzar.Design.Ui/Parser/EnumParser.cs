using System.Collections.Generic;
using System.Linq;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree.Literals;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Ui.Parser
{
    /// <summary>
    /// Get the parser for an enum.
    /// </summary>
    public class EnumParser : BaseTypeParser
    {
        #region properties

        /// <summary>
        /// Gets a parser for a enum.
        /// </summary>
        public static ParserExtensions.Parser<IViToken> ParseToken =>
            EnumNameAndTypeParser
                .Map(
                    success =>
                        ViEnumToken.CreateWithoutWhitespaces(
                            success.ParsedSpan.Start,
                            success.ParsedSpan.Length,
                            success.Value.EnumName,
                            success.Value.EnumDeclaredType,
                            success.Value.EnumType))
                .WithSurroundingWhitespaces();

        /// <inheritdoc />
        protected override ParserExtensions.Parser<IViToken> ValueParser => ParseToken;

        private static ParserExtensions.Parser<(Maybe<string> EnumType, Maybe<string> EnumDeclaredType, string EnumName)>
            EnumNameAndTypeParser =>
                i =>
                    from typeSegments in Consume.CSharpType.And(Consume.Char('.')).Many()(i)
                    from enumName in Consume.CSharpType(typeSegments.OutputPoint)
                    select ParseSuccess<(Maybe<string> EnumType, Maybe<string> EnumDeclaredType, string EnumName)>.FromSpan(
                        typeSegments.ParsedSpan.Start,
                        enumName.ParsedSpan.End,
                        (
                            EnumType: typeSegments.Value.LastOrNone().Map(chars => chars.AsString().TrimEnd('.')),
                            EnumDeclaredType: GetDeclaredType(typeSegments.Value.ToList()),
                            EnumName: enumName.Value.AsString()));

        #endregion

        #region members

        private static Maybe<string> GetDeclaredType(ICollection<IEnumerable<char>> parsedTypeSegments)
        {
            var n = parsedTypeSegments.Count;

            if (n < 2)
            {
                return Maybe.None();
            }
            else
            {
                return parsedTypeSegments.Take(n - 1)
                    .Select(chars => chars.AsString())
                    .ToDisplayString(".")
                    .TrimEnd('.');
            }
        }

        #endregion
    }
}