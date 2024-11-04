using System;
using System.Collections.Generic;
using Twizzar.Design.Ui.Helpers;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree.Keywords;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;

namespace Twizzar.Design.Ui.Parser
{
    /// <summary>
    /// Helper class for consuming chars in the parser.
    /// </summary>
    public static class ConsumeToken
    {
        #region properties

        /// <summary>
        /// Gets a parser which consume one or more whitespaces or a EOF
        /// and returns an ViEmptyToken.
        /// </summary>
        public static ParserExtensions.Parser<IViToken> EmptyToken =>
            Consume.ManyWhitespacesAndEOF
                .Map(success =>
                    new ViEmptyToken(success.ParsedSpan.Start, success.ParsedSpan.Length, success.Value.AsString()));

        /// <summary>
        /// Gets a parser for a unique token.
        /// </summary>
        public static ParserExtensions.Parser<IViToken> UniqueToken =>
            KeywordToken(
                KeyWords.Unique,
                success =>
                    new ViUniqueKeywordToken(
                        success.ParsedSpan,
                        success.Value.AsString()));

        /// <summary>
        /// Gets a parser for a null token.
        /// </summary>
        public static ParserExtensions.Parser<IViToken> NullToken =>
            KeywordToken(
                KeyWords.Null,
                success =>
                    new ViNullKeywordToken(
                        success.ParsedSpan,
                        success.Value.AsString()));

        /// <summary>
        /// Gets a parser for a null token.
        /// </summary>
        public static ParserExtensions.Parser<IViToken> UndefinedToken =>
            KeywordToken(
                KeyWords.Undefined,
                success =>
                    new ViUndefinedToken(
                        success.ParsedSpan,
                        success.Value.AsString()));

        /// <summary>
        /// Gets a parser for a default token.
        /// </summary>
        public static ParserExtensions.Parser<IViToken> DefaultToken =>
            KeywordToken(
                KeyWords.Default,
                success =>
                    new ViDefaultKeywordToken(
                        success.ParsedSpan,
                        success.Value.AsString()));

        #endregion

        #region members

        /// <summary>
        /// Get a parser which consumes a keyword and uses the factory to create a token.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <param name="factory">The factory for creating a token.</param>
        /// <returns>A new parser function.</returns>
        public static ParserExtensions.Parser<IViToken> KeywordToken(
            string keyword,
            Func<IParseSuccess<IEnumerable<char>>, IViToken> factory) =>
            Consume.String(keyword)
                .WithSurroundingWhitespaces()
                .Map(
                    success =>
                        success.WithValue(factory(success)));

        #endregion
    }
}