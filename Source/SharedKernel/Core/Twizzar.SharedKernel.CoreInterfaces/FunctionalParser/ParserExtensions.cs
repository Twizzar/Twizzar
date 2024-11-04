using System;
using System.Collections.Generic;
using System.Linq;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.FunctionalParser
{
    /// <summary>
    /// Extension methods used for parsing.
    /// </summary>
    public static class ParserExtensions
    {
        #region static fields and constants

        private static readonly IEnsureHelper Ensure = EnsureHelper.GetDefault;

        #endregion

        #region delegates

        /// <summary>
        /// Represents a parser function.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="input">The input to parse.</param>
        /// <returns>The result of the parser.</returns>
        public delegate IResult<IParseSuccess<T>, ParseFailure> Parser<out T>(ParserPoint input);

        /// <summary>
        /// Delegate describing a type converter.
        /// </summary>
        /// <typeparam name="TIn">The input type most likely a string.</typeparam>
        /// <typeparam name="TOut">The output/converted type.</typeparam>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        /// <returns>True when the conversion was successful; else false.</returns>
        public delegate bool TypeConverter<in TIn, TOut>(TIn input, out TOut output);

        #endregion

        #region members

        /// <summary>
        /// Parses the specified input string.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>The result of the parser.</returns>
        public static IResult<IParseSuccess<T>, ParseFailure> Parse<T>(this Parser<T> parser, string input)
        {
            Ensure.Many()
                .Parameter(parser, nameof(parser))
                .Parameter(input, nameof(input))
                .ThrowWhenNull();

            var point = ParserPoint.New(input);
            return parser(point);
        }

        /// <summary>
        /// Parse a stream of elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="parser">The parser which will be parse till failure.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<IEnumerable<T>> Many<T>(this Parser<T> parser)
        {
            Ensure.Parameter(parser, nameof(parser)).ThrowWhenNull();

            return i =>
            {
                var remainder = i;
                var output = new List<T>();
                var result = parser(i);

                while (result.IsSuccess)
                {
                    var r = result.GetSuccessUnsafe();

                    // break when parser does not advance to prevent infinite loop.
                    if (remainder.Equals(r.OutputPoint))
                        break;

                    output.Add(r.Value);
                    remainder = r.OutputPoint;
                    result = parser(remainder);
                }

                return Success(i, remainder, output);
            };
        }

        /// <summary>
        /// Parse until the stopCondition parser is true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TUntil"></typeparam>
        /// <param name="parser"></param>
        /// <param name="stopCondition"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<T>> Until<T, TUntil>(this Parser<T> parser, Parser<TUntil> stopCondition)
        {
            return i =>
            {
                var remainder = i;
                var output = new List<T>();
                var result = parser(i);
                var stopResult = stopCondition(i);

                while (remainder.HasNext && result.IsSuccess && stopResult.IsFailure)
                {
                    var r = result.GetSuccessUnsafe();
                    output.Add(r.Value);
                    remainder = r.OutputPoint;
                    result = parser(remainder);
                    stopResult = stopCondition(remainder);
                }

                return Success(i, remainder, output);
            };
        }

        /// <summary>
        /// Parse until the with parser is parsing successful, the result will be the all parsed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <param name="with"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<T>> UntilAndWith<T>(this Parser<T> parser, Parser<T> with)
        {
            return i =>
                from head in parser.Until(with)(i)
                from tail in with(head.OutputPoint)
                select ParseSuccess<IEnumerable<T>>.FromSpan(
                    head.ParsedSpan.Start,
                    tail.ParsedSpan.End,
                    head.Value.Append(tail.Value));
        }

        /// <summary>
        /// Parse until the with parser is parsing successful, the result will be the all parsed.
        /// </summary>
        /// <typeparam name="TParser"></typeparam>
        /// <typeparam name="TWith"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="parser"></param>
        /// <param name="with"></param>
        /// <param name="combineFunc"></param>
        /// <returns></returns>
        public static Parser<TResult> UntilAndWith<TParser, TWith, TResult>(
            this Parser<TParser> parser,
            Parser<TWith> with,
            Func<IEnumerable<TParser>, TWith, TResult> combineFunc)
        {
            return i =>
                from head in parser.Until(with)(i)
                from tail in with(head.OutputPoint)
                select ParseSuccess<TResult>.FromSpan(
                    head.ParsedSpan.Start,
                    tail.ParsedSpan.End,
                    combineFunc(head.Value, tail.Value));
        }

        /// <summary>
        /// Parse at least one element.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="parser">The parser which will be parse till failure.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<IEnumerable<T>> OneOrMore<T>(this Parser<T> parser)
        {
            Ensure.Parameter(parser, nameof(parser)).ThrowWhenNull();

            return parser.And(parser.Many());
        }

        /// <summary>
        /// Consume first the delimiter elements then the parser elements then again the delimiter elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="parser">The inner parser.</param>
        /// <param name="delimiter">The delimiter parser.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<IEnumerable<T>> DelimitedBy<T>(
            this Parser<IEnumerable<T>> parser,
            Parser<T> delimiter)
        {
            Ensure.Many()
                .Parameter(parser, nameof(parser))
                .Parameter(delimiter, nameof(delimiter))
                .ThrowWhenNull();

            return i => (
                from head in delimiter(i)
                from body in parser(head.OutputPoint)
                from tail in delimiter(body.OutputPoint)
                select ParseSuccess<IEnumerable<T>>.FromSpan(
                    head.ParsedSpan.Start,
                    tail.ParsedSpan.End,
                    body.Value.Prepend(head.Value).Append(tail.Value)))
                .MapFailure(failure => failure.WithSpan(new ParserSpan(i, failure.Span.End)));
        }

        /// <summary>
        /// Convert a parser with inner type <see cref="char"/> to IEnumerable&lt;char&gt;.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<IEnumerable<char>> ToEnumerable(this Parser<char> parser)
        {
            Ensure.Parameter(parser, nameof(parser)).ThrowWhenNull();

            return parser.Map(success => success.WithValue(new[] { success.Value }));
        }

        /// <summary>
        /// Consume all surrounding whitespaces. When there is no whitespace a the end, EOF is expected.
        /// </summary>
        /// <param name="parser">The inner parser.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<IEnumerable<char>> WithSurroundingWhitespaces(this Parser<IEnumerable<char>> parser)
        {
            Ensure.Parameter(parser, nameof(parser)).ThrowWhenNull();

            return i => (
                from head in Consume.WhiteSpace.Many()(i)
                from body in parser(head.OutputPoint)
                from tail in Consume.OneOrMoreWhitespacesOrEOF(body.OutputPoint)
                select ParseSuccess<IEnumerable<char>>.FromSpan(
                    head.ParsedSpan.Start,
                    tail.ParsedSpan.End,
                    head.Value.Concat(body.Value).Concat(tail.Value)))
                .MapFailure(failure => failure.WithSpan(new ParserSpan(i, failure.Span.End)));
        }

        /// <summary>
        /// Try consume first or else try consume second.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<T> Or<T>(this Parser<T> first, Parser<T> second)
        {
            Ensure.Many()
                .Parameter(first, nameof(first))
                .Parameter(second, nameof(second))
                .ThrowWhenNull();

            return i =>
                first(i).BindFailure(failure => second(i));
        }

        /// <summary>
        /// Consume first and then second.
        /// </summary>
        /// <typeparam name="TFirst">Type of first parser element.</typeparam>
        /// <typeparam name="TSecond">Type of second parser element.</typeparam>
        /// <typeparam name="TResult">Type of the result element.</typeparam>
        /// <param name="first">The first parser function.</param>
        /// <param name="second">The second parser function.</param>
        /// <param name="combineFunc">Function on how to combine the output of the first and the second parser.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<TResult> And<TFirst, TSecond, TResult>(
            this Parser<TFirst> first,
            Parser<TSecond> second,
            Func<TFirst, TSecond, TResult> combineFunc)
        {
            Ensure.Many()
                .Parameter(first, nameof(first))
                .Parameter(second, nameof(second))
                .ThrowWhenNull();

            return i => (
                from f in first(i)
                from s in second(f.OutputPoint)
                select ParseSuccess<TResult>.FromSpan(
                    f.ParsedSpan.Start,
                    s.ParsedSpan.End,
                    combineFunc(f.Value, s.Value)))
                .MapFailure(failure => failure.WithSpan(new ParserSpan(i, failure.Span.End)));
        }

        /// <summary>
        /// Consume first and then second.
        /// </summary>
        /// <typeparam name="T">Inner type.</typeparam>
        /// <param name="first">The first parser function.</param>
        /// <param name="second">The second parser function.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<IEnumerable<T>> And<T>(
            this Parser<IEnumerable<T>> first,
            Parser<IEnumerable<T>> second) =>
                first.And(second, (a, b) => a.Concat(b));

        /// <summary>
        /// Consume first and then second.
        /// </summary>
        /// <typeparam name="T">Inner type.</typeparam>
        /// <param name="first">The first parser function.</param>
        /// <param name="second">The second parser function.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<IEnumerable<T>> And<T>(
            this Parser<T> first,
            Parser<IEnumerable<T>> second) =>
                first.And(second, (a, b) => b.Prepend(a));

        /// <summary>
        /// Consume first and then second.
        /// </summary>
        /// <param name="first">The first parser function.</param>
        /// <param name="second">The second parser function.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<IEnumerable<char>> And(
            this Parser<char> first,
            Parser<char> second) =>
                first.And(second, (a, b) => new[] { a, b });

        /// <summary>
        /// Consume first and then second.
        /// </summary>
        /// <param name="first">The first parser function.</param>
        /// <param name="second">The second parser function.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<IEnumerable<char>> And(
            this Parser<IEnumerable<char>> first,
            Parser<char> second) =>
                first.And(second, (a, b) => a.Append(b));

        /// <summary>
        /// Consume first and then second.
        /// </summary>
        /// <param name="first">The first parser function.</param>
        /// <param name="second">The second parser function.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<IEnumerable<char>> And(
            this Parser<IEnumerable<char>> first,
            Parser<IEnumerable<char>> second) =>
                first.And(second, (a, b) => a.Concat(b));

        /// <summary>
        /// Monadic map.
        /// </summary>
        /// <typeparam name="T">Input inner type.</typeparam>
        /// <typeparam name="TMap">Output inner type. (Will be wrapped in a <see cref="IParseSuccess{T}"/>).</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="mapFunc">The map function.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<TMap> Map<T, TMap>(this Parser<T> parser, Func<IParseSuccess<T>, TMap> mapFunc)
        {
            Ensure.Many()
                .Parameter(parser, nameof(parser))
                .Parameter(mapFunc, nameof(mapFunc))
                .ThrowWhenNull();

            return parser.Map(s => s.WithValue(mapFunc(s)));
        }

        /// <summary>
        /// Monadic map.
        /// </summary>
        /// <typeparam name="T">Input inner type.</typeparam>
        /// <typeparam name="TMap">Output inner type.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="mapFunc">The map function.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<TMap> Map<T, TMap>(
            this Parser<T> parser,
            Func<IParseSuccess<T>, IParseSuccess<TMap>> mapFunc)
        {
            Ensure.Many()
                .Parameter(parser, nameof(parser))
                .Parameter(mapFunc, nameof(mapFunc))
                .ThrowWhenNull();

            return i => parser(i).MapSuccess(mapFunc);
        }

        /// <summary>
        /// Monadic bind.
        /// </summary>
        /// <typeparam name="T">Element type of the input parser.</typeparam>
        /// <typeparam name="TBind">Element type of the output parser.</typeparam>
        /// <param name="parser">The input parser.</param>
        /// <param name="bindFunc">The bind function.</param>
        /// <returns>A new parser.</returns>
        public static Parser<TBind> Bind<T, TBind>(
            this Parser<T> parser,
            Func<IParseSuccess<T>, IResult<IParseSuccess<TBind>, ParseFailure>> bindFunc)
        {
            Ensure.Many()
                .Parameter(parser, nameof(parser))
                .Parameter(bindFunc, nameof(bindFunc))
                .ThrowWhenNull();

            return i => parser(i).Bind(bindFunc);
        }

        /// <summary>
        /// Returns a successful parse when the parse fails otherwise a failed parse.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="failureMessage">The parse failure message when the parser successes.</param>
        /// <returns>A new parser.</returns>
        public static Parser<char> Not(this Parser<char> parser, string failureMessage)
        {
            Ensure.Parameter(parser, nameof(parser)).ThrowWhenNull();

            return i =>
                parser(i)
                    .Match(
                        success =>
                            Failure<char>(failureMessage, success.ParsedSpan),
                        failure =>
                            failure.Span.End.HasNext
                                ? Success(i, failure.Span.End, i.Current)
                                : Failure<char>("Unexpected end of input reached", failure.Span));
        }

        /// <summary>
        /// Returns a successful parse when the parse fails otherwise a failed parse.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="failureMessage">The parse failure message when the parser successes.</param>
        /// <returns>A new parser.</returns>
        public static Parser<IEnumerable<char>> Not(this Parser<IEnumerable<char>> parser, string failureMessage)
        {
            Ensure.Parameter(parser, nameof(parser)).ThrowWhenNull();

            return i =>
                parser(i)
                    .Match(
                        success =>
                            Failure<IEnumerable<char>>(failureMessage, success.ParsedSpan),
                        failure =>
                            Success(failure.Span.Start, failure.Span.End, failure.Span.Content));
        }

        /// <summary>
        /// Get the output point.
        /// </summary>
        /// <typeparam name="T">The inner type of the parser.</typeparam>
        /// <param name="result">The parser result.</param>
        /// <returns>The output point where the parser is currently located.</returns>
        public static ParserPoint OutputPoint<T>(this IResult<IParseSuccess<T>, ParseFailure> result)
        {
            Ensure.Parameter(result, nameof(result)).ThrowWhenNull();

            return result.Match(success => success.OutputPoint, failure => failure.OutputPoint);
        }

        /// <summary>
        /// Create a Success result with a start and a end point.
        /// </summary>
        /// <typeparam name="T">The inner type.</typeparam>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <param name="value">The value.</param>
        /// <returns>A new success result.</returns>
        public static IResult<IParseSuccess<T>, ParseFailure> Success<T>(
            ParserPoint start,
            ParserPoint end,
            T value)
        {
            Ensure.Many()
                .Parameter(start, nameof(start))
                .Parameter(end, nameof(end))
                .ThrowWhenNull();

            return Result.Success<IParseSuccess<T>, ParseFailure>(ParseSuccess<T>.FromSpan(start, end, value));
        }

        /// <summary>
        /// Create a Failure result.
        /// </summary>
        /// <typeparam name="T">The inner type.</typeparam>
        /// <param name="message">The failure message.</param>
        /// <param name="span">The span.</param>
        /// <returns>A new failure result.</returns>
        public static IResult<IParseSuccess<T>, ParseFailure> Failure<T>(
            string message,
            ParserSpan span)
        {
            Ensure.Many()
                .Parameter(span, nameof(span))
                .Parameter(message, nameof(message))
                .ThrowWhenNull();

            return Result.Failure<IParseSuccess<T>, ParseFailure>(new ParseFailure(message, span));
        }

        /// <summary>
        /// Parse first, and if successful, then parse second.
        /// </summary>
        /// <typeparam name="T">The old inner type.</typeparam>
        /// <typeparam name="TNew">The new inner type.</typeparam>
        /// <param name="first">First parser.</param>
        /// <param name="second">Second parser.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<TNew> Then<T, TNew>(this Parser<T> first, Func<IParseSuccess<T>, Parser<TNew>> second)
        {
            Ensure.Many()
                .Parameter(first, nameof(first))
                .Parameter(second, nameof(second))
                .ThrowWhenNull();

            return i => first(i).Bind(s => second(s)(s.OutputPoint));
        }

        /// <summary>
        /// Construct a parser that indicates that the given parser
        /// is optional. The returned parser will succeed on
        /// any input no matter whether the given parser
        /// succeeds or not.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The parser to wrap.</param>
        /// <returns>An optional version of the given parser.</returns>
        public static Parser<Maybe<T>> Optional<T>(this Parser<T> parser)
        {
            Ensure.Parameter(parser, nameof(parser)).ThrowWhenNull();

            return i =>
                parser(i)
                    .Match(
                        onSuccess: s =>
                            Success(
                                s.ParsedSpan.Start,
                                s.ParsedSpan.End,
                                Maybe.Some(s.Value)),
                        onFailure: _ =>
                            Success(i, i, Maybe.None<T>()));
        }

        /// <summary>
        /// Consume all surrounding whitespaces.
        /// </summary>
        /// <param name="parser">The inner parser.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<IEnumerable<char>> WithSurroundingWhiteSpaces(this Parser<IEnumerable<char>> parser)
        {
            EnsureHelper.GetDefault.Parameter(parser, nameof(parser)).ThrowWhenNull();

            return i => (
                    from head in Consume.WhiteSpace.Many()(i)
                    from body in parser(head.OutputPoint)
                    from tail in Consume.WhiteSpace.Many()(body.OutputPoint)
                    select ParseSuccess<IEnumerable<char>>.FromSpan(
                        head.ParsedSpan.Start,
                        tail.ParsedSpan.End,
                        head.Value.Concat(body.Value).Concat(tail.Value)))
                        .MapFailure(failure => failure.WithSpan(new ParserSpan(i, failure.Span.End)));
        }

        /// <summary>
        /// Consume all surrounding whitespaces.
        /// </summary>
        /// <param name="parser">The inner parser.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<IEnumerable<char>> WithSurroundingWhiteSpaces(this Parser<char> parser)
        {
            EnsureHelper.GetDefault.Parameter(parser, nameof(parser)).ThrowWhenNull();
            return parser.ToEnumerable().WithSurroundingWhiteSpaces();
        }

        /// <summary>
        /// Try to convert the inner element of a parser.
        /// </summary>
        /// <typeparam name="TIn">The input inner element type.</typeparam>
        /// <typeparam name="TOut">The output inner element type.</typeparam>
        /// <param name="parser">The input parser.</param>
        /// <param name="converter">The converter.</param>
        /// <returns>A new parser with the converted inner type.</returns>
        public static Parser<TOut> TryConvert<TIn, TOut>(this Parser<TIn> parser, TypeConverter<TIn, TOut> converter) =>
            parser.Bind(
                success =>
                    converter(success.Value, out var result)
                        ? Result.Success<IParseSuccess<TOut>, ParseFailure>(
                            success.WithValue(result))
                        : Failure<TOut>(
                            $"Cannot parse {success.Value} to a boolean", success.ParsedSpan));

        /// <summary>
        /// Convert the inner element of type IEnumerable char to string.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<string> ConvertToString(this Parser<IEnumerable<char>> parser) =>
            parser.TryConvert(
                (IEnumerable<char> input, out string output) =>
                {
                    output = input.AsString();
                    return true;
                });

        /// <summary>
        /// Convert the char enumerable to a string.
        /// </summary>
        /// <param name="chars"></param>
        /// <returns>A new string.</returns>
        public static string AsString(this IEnumerable<char> chars)
        {
            Ensure.Parameter(chars, nameof(chars)).ThrowWhenNull();

            return new string(chars.ToArray());
        }

        /// <summary>
        /// Flatten a Parser with inner type IEnumerable&lt;IEnumerable&lt;T&gt;&gt; to IEnumerable&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T">The IEnumerable inner type.</typeparam>
        /// <param name="self">The parser.</param>
        /// <returns>A new parser function.</returns>
        public static Parser<IEnumerable<T>> Flatten<T>(this Parser<IEnumerable<IEnumerable<T>>> self) =>
            self.Map(success => success.WithValue(success.Value.SelectMany(x => x)));

        #endregion
    }
}