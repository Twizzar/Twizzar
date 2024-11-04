using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Parser;
using Twizzar.TestCommon;

namespace Twizzar.Design.Test.Parser;

[TestClass]
public class CharParserTests : BaseTypeParserTests
{
    #region properties

    /// <inheritdoc />
    public override BaseTypeParser BaseTypeParser => new CharParser();

    #endregion

    #region members

    [TestMethod]
    [DataRow("'c' ", 'c')]
    [DataRow(" 'a' ", 'a')]
    [DataRow("    'b'   ", 'b')]
    [DataRow("'c'   ", 'c')]
    [DataRow("'d'   ", 'd')]
    [DataRow("'\"'   ", '\"')]
    [DataRow("'\\''   ", '\\')]
    public void Parse_valid_char_returns_a_char_token(string input, char expectedCharacter)
    {
        // arrange
        var sut = new CharParser();

        // act
        var subject = sut.Parse(input);

        // assert
        var token = TestHelper.AssertAndUnwrapSuccess(subject);
        var charToken = token.Should().BeAssignableTo<IViCharToken>().Subject;
        charToken.Character.Should().Be(expectedCharacter);
        charToken.Length.Should().Be(input.Length);
        charToken.Start.Should().Be(0);
        charToken.ContainingText.Should().Be(input);
    }

    [TestMethod]
    [DataRow("invalid")]
    [DataRow("'i")]
    [DataRow("a")]
    [DataRow("5")]
    [DataRow("'ab'")]
    [DataRow("inva\"lid")]
    [DataRow("v'v'")]
    public void Parse_invalid_char_returns_a_failure(string input)
    {
        // arrange
        var sut = new CharParser();

        // act
        var subject = sut.Parse(input);

        // assert
        subject.IsSuccess.Should().BeFalse();
        var failure = subject.GetFailureUnsafe();

        failure.Span.Start.Position.Should().Be(0);
        failure.Span.Length.Should().Be(input.Length);
    }

    [TestMethod]
    [DataRow("'c' notValid")]
    [DataRow("null notValid")]
    [DataRow("unique notValid")]
    [DataRow("c")]
    [DataRow("'c' 'c' asd")]
    [DataRow("'c' asd 'c'")]
    [DataRow("null asd 'c'")]
    [DataRow("unique asd 'c'")]
    public void Parse_many_tokens_and_one_is_failing_returns_all_tokens_till_failure(string input)
    {
        // arrange
        var sut = new CharParser();

        // act
        var subject = sut.Parse(input);

        // assert
        subject.IsFailure.Should().BeTrue();
    }

    #endregion
}