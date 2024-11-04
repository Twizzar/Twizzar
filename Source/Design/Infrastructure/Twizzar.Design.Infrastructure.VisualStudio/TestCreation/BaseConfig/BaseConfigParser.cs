using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Twizzar.Design.CoreInterfaces.TestCreation.BaseConfig;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.Functional.Monads.ResultMonad;
using static Twizzar.SharedKernel.CoreInterfaces.FunctionalParser.ParserExtensions;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.BaseConfig;

/// <inheritdoc cref="IBaseConfigParser" />
public class BaseConfigParser : IBaseConfigParser
{
    #region properties

    /// <summary>
    /// Gets a parser which parses the entire document.
    /// </summary>
    private static Parser<ConfigSyntax> ParseDocument =>
        i =>
            from leadingComments in ParseComment.Many()(i)
            from entries in ParseTagAndContent.OneOrMore()(leadingComments.OutputPoint)
            select entries.Map(configEntries =>
                new ConfigSyntax(new Version(1, 0), configEntries.ToImmutableList()));

    /// <summary>
    /// Gets a parse which parses a tag and the leading content.
    /// </summary>
    private static Parser<ConfigEntry> ParseTagAndContent =>
        i =>
            from tag in ParseTag(i)
            from content in ParseContent(tag.OutputPoint)
            select ParseSuccess<ConfigEntry>.FromSpan(
                tag.ParsedSpan.Start,
                content.ParsedSpan.End,
                new ConfigEntry(tag.Value, content.Value.AsString()));

    /// <summary>
    /// Gets a parser which parses a tag in the format [SomeTagName:].
    /// </summary>
    private static Parser<string> ParseTag =>
        i =>
            from whitespaces in Consume.WhiteSpace.Many()(i)
            from openBracket in Consume.Char('[')(whitespaces.OutputPoint)
            from tag in Consume.AnyChar.Until(Consume.String(":]").Or(EndOfStatement)).WithSurroundingWhiteSpaces()(
                openBracket.OutputPoint)
            from closingBracket in Consume.String(":]")(tag.OutputPoint)
            from lineEndAndWhiteSpaces in Consume.ManyWhitespacesAndLineEnd(closingBracket.OutputPoint)
            select ParseSuccess<string>.FromSpan(
                whitespaces.ParsedSpan.Start,
                lineEndAndWhiteSpaces.ParsedSpan.End,
                tag.Value.AsString());

    /// <summary>
    /// Gets a Parser which parses all lines and ignore comment lines, until a Tag is reached.
    /// </summary>
    private static Parser<string> ParseContent =>
        ParseComment.Or(ValidContentLine)
            .Until(ParseTag)
            .Flatten()
            .ConvertToString();

    /// <summary>
    /// Gets a parse which parses a valid content line.
    /// If comment lines are present they will be ignored.
    /// </summary>
    private static Parser<IEnumerable<char>> ValidContentLine =>
        Consume.AnyChar.UntilAndWith(EndOfStatement, (anyChars, endLine) => anyChars.AsString() + endLine);

    private static Parser<string> EndOfStatement =>
        Consume.LineEnd
            .Or(Consume.EOF.Map(success => success.Map(unit => string.Empty)));

    /// <summary>
    /// Gets a Parser which parses a comment and return an Empty char sequence.
    /// </summary>
    private static Parser<IEnumerable<char>> ParseComment =>
        i =>
            from leadingWhiteSpace in Consume.WhiteSpace.Many()(i)
            from tag in Consume.String("!!")(leadingWhiteSpace.OutputPoint)
            from any in Consume.AnyChar.Until(EndOfStatement)(tag.OutputPoint)
            from lineEnd in Consume.LineEnd(any.OutputPoint)
            select ParseSuccess<IEnumerable<char>>.FromSpan(
                leadingWhiteSpace.ParsedSpan.Start,
                lineEnd.ParsedSpan.End,
                Enumerable.Empty<char>());

    #endregion

    #region members

    /// <inheritdoc />
    public IResult<ConfigSyntax, ParseFailure> ParseBaseConfig(string text) =>
        ParseDocument.Parse(text)
            .MapSuccess(success => success.Value);

    #endregion
}