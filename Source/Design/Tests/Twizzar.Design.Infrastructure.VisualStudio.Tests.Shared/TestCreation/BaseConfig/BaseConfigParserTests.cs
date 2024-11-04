using System;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.BaseConfig;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.TestCreation.BaseConfig;

[TestFixture]
public class BaseConfigParserTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<BaseConfigParser>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void Valid_text_is_parsed_correctly()
    {
        // arrange
        var sut = new ItemBuilder<BaseConfigParser>()
            .Build();

        var nl = Environment.NewLine;

        const string text = """
                            !! a comment
                            [Tag 2:]
                            Content of Tag 2
                            [Tag3:]
                            !! a comment
                            Content of Tag 3
                            !! a comment

                            [Tag1:]
                            Content
                            of
                            Tag 1
                            """;

        // act
        var result = sut.ParseBaseConfig(text);

        // assert
        var (configVersion, configEntries) = TestHelper.AssertAndUnwrapSuccess(result);
        configVersion.Should().Be(Version.Parse("1.0"));

        configEntries[0].Tag.Should().Be("Tag 2");
        configEntries[0].Content.Should().Be($"Content of Tag 2{nl}");

        configEntries[1].Tag.Should().Be("Tag3");
        configEntries[1].Content.Should().Be($"Content of Tag 3{nl}");

        configEntries[2].Tag.Should().Be("Tag1");
        configEntries[2].Content.Trim().Should().Be($"Content{nl}of{nl}Tag 1");
    }

    [TestCase("No Tags", 1, 1)]
    [TestCase("[Tag 1]Content on same line", 1, 1)]
    public void Invalid_config_fails_with_correct_position(string text, int expectedLine, int expectedColumn)
    {
        // arrange
        var sut = new ItemBuilder<BaseConfigParser>()
            .Build();

        // act
        var result = sut.ParseBaseConfig(text);

        // assert
        result.IsFailure.Should().BeTrue();
        var failure = result.GetFailureUnsafe();
        var (line, column) = failure.Span.Start.LineAndColumn;
        line.Should().Be(expectedLine);
        column.Should().Be(expectedColumn, failure.Message);
    }
}