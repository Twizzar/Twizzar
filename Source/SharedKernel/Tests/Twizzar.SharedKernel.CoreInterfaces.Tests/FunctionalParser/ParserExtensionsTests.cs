using FluentAssertions;
using NUnit.Framework;

using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;

namespace Twizzar.SharedKernel.CoreInterfaces.Tests.FunctionalParser;

[TestFixture, TestOf(typeof(ParserExtensions))]
public class ParserExtensionsTests
{
    [TestCase("This is a test", "This")]
    [TestCase("Test ", "Test")]
    [TestCase(" ", "")]
    public void Until_works_correctly(string text, string expectedParseResult)
    {
        var parser = Consume.AnyChar.Until(Consume.WhiteSpace);

        var result = parser.Parse(text);

        var parseSuccess = AssertAndUnwrapSuccess(result);
        parseSuccess.Value.AsString().Should().Be(expectedParseResult);
        parseSuccess.ParsedSpan.Start.Position.Should().Be(0);
        parseSuccess.ParsedSpan.End.Position.Should().Be(expectedParseResult.Length);
    }
}