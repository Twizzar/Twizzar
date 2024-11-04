using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.TestCreation.Mapping;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Mapping;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio2022.Tests.Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Mapping;

[TestFixture]
public class WildcardPatternMatcherTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<WildcardPatternMatcher>()
            .ShouldThrowArgumentNullException();
    }

    [TestCase("Test", "Test")]
    [TestCase("PrefixTest", "*Test")]
    [TestCase("PrefixTestTest", "*Test")]
    [TestCase("TestPostfix", "Test*")]
    [TestCase("TestTestPostfix", "Test*")]
    [TestCase("Test", "*")]
    [TestCase("InfixTestInfix", "*Test*")]
    [TestCase("TestATest", "Test*Test")]
    public void Valid_pattern_will_match(string input, string pattern)
    {
        // arrange
        var sut = new ItemBuilder<WildcardPatternMatcher>()
            .Build();

        var expectedReplacement = TestHelper.RandomString();

        // act
        var result = sut.Match(input, new []{ new MappingEntry(Maybe.Some(pattern), expectedReplacement) });

        // assert
        var replacement = TestHelper.AssertAndUnwrapSuccess(result);
        replacement.Should().Be(expectedReplacement);
    }

    [TestCase("A", "B")]
    [TestCase("A", "*B")]
    [TestCase("A", "B*")]
    [TestCase("InfixAInfix", "*A")]
    [TestCase("InfixAInfix", "A*")]
    public void Invalid_pattern_will_no_match(string input, string pattern)
    {
        // arrange
        var sut = new ItemBuilder<WildcardPatternMatcher>()
            .Build();

        var expectedReplacement = TestHelper.RandomString();

        // act
        var result = sut.Match(input, new[] { new MappingEntry(Maybe.Some(pattern), expectedReplacement) });

        // assert
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    public void Match_by_the_priority_given()
    {
        // arrange
        var sut = new ItemBuilder<WildcardPatternMatcher>()
            .Build();

        var expectedReplacement = TestHelper.RandomString();

        var patterns = new[] 
        {
            new MappingEntry(Maybe.Some("Test"), "ShouldNotMatch"),
            new MappingEntry(Maybe.Some("*Test"), expectedReplacement),
            new MappingEntry(Maybe.Some("*Test"), "ShouldNotMatch"),
        };

        // act
        var result = sut.Match("SomeTest", patterns);

        // assert
        var replacement = TestHelper.AssertAndUnwrapSuccess(result);
        replacement.Should().Be(expectedReplacement);
    }

    [Test]
    public void When_no_matches_before_take_default_match_when_available()
    {
        // arrange
        var sut = new ItemBuilder<WildcardPatternMatcher>()
            .Build();

        var expectedReplacement = TestHelper.RandomString();

        var patterns = new[]
        {
            new MappingEntry(Maybe.Some("Test"), "ShouldNotMatch"),
            new MappingEntry(Maybe.None(), expectedReplacement),
        };

        // act
        var result = sut.Match("SomeTest", patterns);

        // assert
        var replacement = TestHelper.AssertAndUnwrapSuccess(result);
        replacement.Should().Be(expectedReplacement);
    }

    [TestCase("PrefixTestPostfix", "Prefix*Postfix", "$1", "Test")]
    [TestCase("PrefixTest", "Prefix*", "$1", "Test")]
    [TestCase("TestPostfix", "*Postfix", "$1", "Test")]
    [TestCase("a/b/c", "*/*/*", "$1 $2 $3", "a b c")]
    [TestCase("ATestB", "*Test*", "$3", "$3")]
    public void CorrectReplacementIsInserted(string input, string pattern, string replaceWith, string expectedReplacement)
    {
        // arrange
        var sut = new ItemBuilder<WildcardPatternMatcher>()
            .Build();

        // act
        var result = sut.Match(input, new[] { new MappingEntry(Maybe.Some(pattern), replaceWith) });

        // assert
        var replacement = TestHelper.AssertAndUnwrapSuccess(result);
        replacement.Should().Be(expectedReplacement);
    }
}