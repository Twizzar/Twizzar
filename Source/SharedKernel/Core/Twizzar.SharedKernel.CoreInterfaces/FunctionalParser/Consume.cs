using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;
using static Twizzar.SharedKernel.CoreInterfaces.FunctionalParser.ParserExtensions;

namespace Twizzar.SharedKernel.CoreInterfaces.FunctionalParser
{
    /// <summary>
    /// Helper class for consuming chars in the parser.
    /// </summary>
    public static class Consume
    {
        #region static fields and constants

        private static readonly IEnsureHelper Ensure = EnsureHelper.GetDefault;

        #endregion

        #region properties

        /// <summary>
        /// Gets a parser for a single white space character.
        /// </summary>
        public static Parser<char> WhiteSpace =>
            Char(char.IsWhiteSpace, "No whitespace character");

        /// <summary>
        /// Gets a parser which consume a End of File.
        /// </summary>
        public static Parser<Unit> EOF =>
            i =>
                i.HasNext
                    ? Failure<Unit>($"Expected EOF got {i.Current}", new ParserSpan(i, i))
                    : Success(i, i, Unit.New);

        /// <summary>
        /// Gets a parser which consume one or more whitespaces or a EOF.
        /// When there is no whitespace and the pointer is not at the end this will fail.
        /// </summary>
        public static Parser<IEnumerable<char>> OneOrMoreWhitespacesOrEOF =>
            WhiteSpace.OneOrMore().Or(EOF.Map(_ => Enumerable.Empty<char>()));

        /// <summary>
        /// Gets a parser which consume many whitespaces followed by EOF.
        /// When there is no EOF or whitespaces and EOF his will fail.
        /// </summary>
        public static Parser<IEnumerable<char>> ManyWhitespacesAndEOF =>
            WhiteSpace.Many().And(EOF.Map(_ => Enumerable.Empty<char>()));

        /// <summary>
        /// Gets a parse which consumes a line end.
        /// </summary>
        public static Parser<string> LineEnd =>
            String(Environment.NewLine, "line end");

        /// <summary>
        /// Gets a parser which consume many whitespaces followed by a LineEnd.
        /// When there is no LineEnd or whitespaces and LineEnd this will fail.
        /// </summary>
        public static Parser<string> ManyWhitespacesAndLineEnd =>
            WhiteSpace
                .Until(LineEnd)
                .And(LineEnd, (whiteSpaces, lineEnd) => whiteSpaces.AsString() + lineEnd);

        /// <summary>
        /// Gets a parser for parsing a digit (0 - 9).
        /// </summary>
        public static Parser<char> Digit => Char(char.IsDigit, "digit character");

        /// <summary>
        /// Gets a parser for parsing any char.
        /// </summary>
        public static Parser<char> AnyChar => Char(c => true, "any char");

        /// <summary>
        /// Gets a parser for parsing a numeric (0, 0.5, 10.0).
        /// </summary>
        public static Parser<IEnumerable<char>> Numeric =>
            i =>
                from negativeSign in OptionalNegativeSign(i)
                from prefixDigit in Digit.OneOrMore()(negativeSign.OutputPoint)
                from postfixDigit in PostfixDigit(prefixDigit.OutputPoint)
                let absoluteNumber =
                    prefixDigit.Value.Concat(postfixDigit.Value.SomeOrProvided(Enumerable.Empty<char>()))
                let number = negativeSign.Value.Match(absoluteNumber.Prepend, () => absoluteNumber)
                select
                    ParseSuccess<IEnumerable<char>>.FromSpan(
                        negativeSign.ParsedSpan.Start,
                        postfixDigit.ParsedSpan.End,
                        number);

        /// <summary>
        /// Gets a parser which consumes a valid c# type.
        /// </summary>
        public static Parser<IEnumerable<char>> CSharpType =>
            HeadCSharpType.ToEnumerable() // First character has different rules.
                .And(TailCSharpType.Many());

        /// <summary>
        /// Gets a parser which consumes a valid c# full type.
        /// </summary>
        public static Parser<IEnumerable<char>> CSharpFullType =>
            input =>
                from ns in CSharpType.And(Char('.')).Many().Flatten()(input)
                from typeName in CSharpType(ns.OutputPoint)
                from genericArguments in GenericArguments.Optional()(typeName.OutputPoint)
                from arrayBrackets in ArrayBrackets.Optional()(genericArguments.OutputPoint)
                select ParseSuccess<IEnumerable<char>>.FromSpan(
                    input,
                    arrayBrackets.ParsedSpan.End,
                    Concatenate(
                        ns.Value,
                        typeName.Value,
                        genericArguments.Value.SomeOrProvided(Enumerable.Empty<char>()),
                        arrayBrackets.Value.SomeOrProvided(Enumerable.Empty<char>())));

        /// <summary>
        /// Gets a parser which consumes the first character of a c# type.
        /// <remarks>
        /// see https://stackoverflow.com/questions/950616/what-characters-are-allowed-in-c-sharp-class-name
        /// and https://www.ecma-international.org/publications-and-standards/standards/ecma-334/
        /// </remarks>
        /// </summary>
        public static Parser<char> HeadCSharpType =>
            Char(
                    UnicodeCategory.UppercaseLetter, // https://www.fileformat.info/info/unicode/category/Lu/list.htm
                    UnicodeCategory.LowercaseLetter, // https://www.fileformat.info/info/unicode/category/Ll/list.htm
                    UnicodeCategory.TitlecaseLetter, // https://www.fileformat.info/info/unicode/category/Lt/list.htm
                    UnicodeCategory.ModifierLetter, // https://www.fileformat.info/info/unicode/category/Lm/list.htm
                    UnicodeCategory.OtherLetter)
                .Or(Char('_')); // https://www.fileformat.info/info/unicode/category/Lo/list.htm

        /// <summary>
        /// Gets a parser which consumes a character of a c# type expect the first one.
        /// <remarks>
        /// see https://stackoverflow.com/questions/950616/what-characters-are-allowed-in-c-sharp-class-name
        /// and https://www.ecma-international.org/publications-and-standards/standards/ecma-334/
        /// </remarks>
        /// </summary>
        public static Parser<char> TailCSharpType =>
            Char(
                UnicodeCategory.UppercaseLetter, // https://www.fileformat.info/info/unicode/category/Lu/list.htm
                UnicodeCategory.LowercaseLetter, // https://www.fileformat.info/info/unicode/category/Ll/list.htm
                UnicodeCategory.TitlecaseLetter, // https://www.fileformat.info/info/unicode/category/Lt/list.htm
                UnicodeCategory.ModifierLetter, // https://www.fileformat.info/info/unicode/category/Lm/list.htm
                UnicodeCategory.OtherLetter, // https://www.fileformat.info/info/unicode/category/Lo/list.htm
                UnicodeCategory.LetterNumber, // https://www.fileformat.info/info/unicode/category/Nl/list.htm
                UnicodeCategory.NonSpacingMark, // https://www.fileformat.info/info/unicode/category/Mn/list.htm
                UnicodeCategory.SpacingCombiningMark, // https://www.fileformat.info/info/unicode/category/Mc/list.htm
                UnicodeCategory.DecimalDigitNumber, // https://www.fileformat.info/info/unicode/category/Nd/list.htm
                UnicodeCategory.ConnectorPunctuation, // https://www.fileformat.info/info/unicode/category/Pc/list.htm
                UnicodeCategory.Format); // https://www.fileformat.info/info/unicode/category/Cf/list.htm

        private static Parser<IEnumerable<char>> ArrayBrackets =>
            Char('[')
                .WithSurroundingWhiteSpaces()
                .And(Char(']'));

        private static Parser<IEnumerable<char>> GenericArguments =>
            input =>
                from genericStart in Char('<').WithSurroundingWhiteSpaces()(input)
                from genericTypes in CSharpFullType.And(Char(',').WithSurroundingWhiteSpaces())
                    .Many()
                    .Flatten()(genericStart.OutputPoint)
                from genericLastType in CSharpFullType(genericTypes.OutputPoint)
                from genericEnd in WhiteSpace.Many().And(Char('>'))(genericLastType.OutputPoint)
                select ParseSuccess<IEnumerable<char>>.FromSpan(
                    input,
                    genericEnd.ParsedSpan.End,
                    Concatenate(genericStart.Value, genericTypes.Value, genericLastType.Value, genericEnd.Value));

        private static Parser<Maybe<IEnumerable<char>>> PostfixDigit =>
            Char('.') // consume a .
                .And(
                    Digit.OneOrMore(), // then consume many digits
                    (c, enumerable) => enumerable.Prepend(c))
                .Optional(); // make it optional

        private static Parser<Maybe<char>> OptionalNegativeSign => Char('-').Optional();

        #endregion

        #region members

        /// <summary>
        /// Consume a sequence which starts with the delimiter and ends with the delimiter.
        /// When the delimiter is escaped  with \ it is not considered a delimiter.
        /// </summary>
        /// <param name="delimiter">The delimiter to use.</param>
        /// <param name="bodyParserFunc">
        /// A function to append to the body parser.
        /// The body parser only parses one character or one escaped character.
        /// To parse many use <c>parser => parser.Many().Flatten()</c>.
        /// To parse only one char use <c>parser => parser</c>.
        /// </param>
        /// <returns>A new parser function which has parsed the token. (delimiters included).</returns>
        public static Parser<IEnumerable<char>> Token(
            Parser<char> delimiter,
            Func<Parser<IEnumerable<char>>, Parser<IEnumerable<char>>> bodyParserFunc)
        {
            Ensure.Parameter(delimiter, nameof(delimiter)).ThrowWhenNull();

            var notDelimiter = delimiter.Not($"Expected any char expect the delimiter. escape the delimiter with \\");

            var bodyParser =
                bodyParserFunc(
                    Char('\\')
                        .And(AnyChar)
                        .Or(notDelimiter.ToEnumerable()));

            return start =>
                from head in delimiter(start)
                from body in bodyParser(head.OutputPoint)
                from tail in delimiter(body.OutputPoint)
                select ParseSuccess<IEnumerable<char>>.FromSpan(
                    start,
                    tail.OutputPoint,
                    body.Value.Prepend(head.Value).Append(tail.Value));
        }

        /// <summary>
        /// Consume a sequence of many chars which starts with the delimiter " and ends with the delimiter ".
        /// When the " is escaped  with \" it is not considered a delimiter.
        /// </summary>
        /// <returns>A new parser function which has parsed the token. (delimiters included).</returns>
        public static Parser<IEnumerable<char>> StringToken() =>
            Token(Char('\"'), parser => parser.Many().Flatten());

        /// <summary>
        /// Consume a char which starts with the delimiter ' and ends with the delimiter '.
        /// When the ' is escaped  with \' it is not considered a delimiter.
        /// </summary>
        /// <returns>A new parser function which has parsed the token. (delimiters included).</returns>
        public static Parser<IEnumerable<char>> CharToken() =>
            Token(Char('\''), parser => parser);

        /// <summary>
        /// Consumes a singe character which is in one of the unicode categories.
        /// </summary>
        /// <param name="unicodeCategories">A array of unicode categories.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<char> Char(params UnicodeCategory[] unicodeCategories) =>
            Char(
                c => unicodeCategories.Contains(char.GetUnicodeCategory(c)),
                $"Character of one of the unicode categories {string.Join(",", unicodeCategories.Select(category => category.ToString()))}");

        /// <summary>
        /// Consume a single character c.
        /// </summary>
        /// <param name="c">The character to parse.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<char> Char(char c) =>
            Char(ch => c == ch, char.ToString(c));

        /// <summary>
        /// Consume a single character c. Which is one of the specified characters.
        /// </summary>
        /// <param name="cs">The characters to parse.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<char> Char(params char[] cs) =>
            Char(cs.Contains, string.Join(",", cs));

        /// <summary>
        /// Try consume a single character matching predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="message">Message on failure.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<char> Char(Predicate<char> predicate, string message)
        {
            Ensure.Many()
                .Parameter(predicate, nameof(predicate))
                .Parameter(message, nameof(message))
                .ThrowWhenNull();

            return i =>
            {
                if (i.HasNext)
                {
                    if (predicate(i.Current))
                    {
                        return Result.Success<IParseSuccess<char>, ParseFailure>(
                            ParseSuccess<char>.FromSpan(i, i.Next(), i.Current));
                    }
                    else
                    {
                        return Result.Failure<IParseSuccess<char>, ParseFailure>(
                            new ParseFailure($"Expected {message} got {i.Current}", new ParserSpan(i, i.Next())));
                    }
                }

                return Result.Failure<IParseSuccess<char>, ParseFailure>(
                    new ParseFailure("Unexpected end of input reached", new ParserSpan(i, i)));
            };
        }

        /// <summary>
        /// Consume a single character except those matching <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Characters not to match.</param>
        /// <param name="message">Message on failure.</param>
        /// <returns>A parser for characters except those matching <paramref name="predicate"/>.</returns>
        public static Parser<char> CharExcept(Predicate<char> predicate, string message)
        {
            Ensure.Many()
                .Parameter(predicate, nameof(predicate))
                .Parameter(message, nameof(message))
                .ThrowWhenNull();

            return Char(c => !predicate(c), "any character except " + message);
        }

        /// <summary>
        /// Consume a single character except c.
        /// </summary>
        /// <param name="c"></param>
        /// <returns>A new parser function.</returns>
        public static Parser<char> CharExcept(char c) =>
            CharExcept(ch => c == ch, char.ToString(c));

        /// <summary>
        /// Consume a single character except the given chars.
        /// </summary>
        /// <param name="chars"></param>
        /// <returns>A new parser function.</returns>
        public static Parser<char> CharExcept(params char[] chars) =>
            CharExcept(
                chars.Contains,
                string.Join(", ", chars.Select(c => c.ToString())));

        /// <summary>
        /// Try consume a string.
        /// </summary>
        /// <param name="s">The string to consume.</param>
        /// <param name="message">Message on failure.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<string> String(string s, string message = null)
        {
            Ensure.Parameter(s, nameof(s)).ThrowWhenNull();
            message ??= s;

            return i =>
            {
                var textPoint = ParserPoint.New(s);
                var current = i;

                while (current.HasNext && textPoint.HasNext && current.Current == textPoint.Current)
                {
                    current = current.Next();
                    textPoint = textPoint.Next();
                }

                return textPoint.HasNext
                    ? Result.Failure<IParseSuccess<string>, ParseFailure>(
                        new ParseFailure(
                            $"The text {ParserPoint.GetContent(i, current)} does not match {message}",
                            new ParserSpan(i, current)))
                    : Success(i, current, s);
            };
        }

        /// <summary>
        /// Concatenate many enumerables to one.
        /// </summary>
        /// <param name="lists">A list fo <see cref="IEnumerable{T}"/>s.</param>
        /// <typeparam name="T">The inner type.</typeparam>
        /// <returns>A flatten <see cref="IEnumerable{T}"/>.</returns>
        private static IEnumerable<T> Concatenate<T>(params IEnumerable<T>[] lists) =>
            lists.SelectMany(x => x);

        #endregion
    }
}