using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Parser;
using Twizzar.TestCommon;

namespace Twizzar.Design.Test.Parser;

[TestClass]
public class NumericParserTests : BaseTypeParserTests
{
    #region Overrides of BaseTypeParserTests

    /// <inheritdoc />
    public override BaseTypeParser BaseTypeParser => new NumericParser();

    #endregion

    #region members

    [TestMethod]
    [DataRow("5 ", "5")]
    [DataRow("-5 ", "-5")]
    [DataRow(" 30 ", "30")]
    [DataRow(" -30 ", "-30")]
    [DataRow("    400   ", "400")]
    [DataRow("    -400   ", "-400")]
    [DataRow("2   ", "2")]
    [DataRow("-2   ", "-2")]
    [DataRow("10   ", "10")]
    [DataRow("-10   ", "-10")]
    [DataRow("10000   ", "10000")]
    [DataRow("-10000   ", "-10000")]
    [DataRow("0.5   ", "0.5")]
    [DataRow("-0.5   ", "-0.5")]
    [DataRow("   10.6   ", "10.6")]
    [DataRow("   -10.6   ", "-10.6")]
    [DataRow("   -10.6d   ", "-10.6d")]
    [DataRow(" 1.1f ", "1.1f")]
    [DataRow(" 1.1F ", "1.1F")]
    [DataRow(" 1.1d ", "1.1d")]
    [DataRow(" 1.1D ", "1.1D")]
    [DataRow(" 1.1m ", "1.1m")]
    [DataRow(" 1.1M ", "1.1M")]
    public void Parse_valid_numeric_returns_a_numeric_token(string input, string expectedNumericWithSuffix)
    {
        // arrange
        var sut = new NumericParser();

        // act
        var subject = sut.Parse(input);

        // assert
        var token = TestHelper.AssertAndUnwrapSuccess(subject);
        var numericToken = token.Should().BeAssignableTo<IViNumericToken>().Subject;
        numericToken.NumericWithSuffix.ToString().Should().Be(expectedNumericWithSuffix);
        numericToken.Length.Should().Be(input.Length);
        numericToken.Start.Should().Be(0);
        numericToken.ContainingText.Should().Be(input);
    }

    [TestMethod]
    [DataRow("invalid")]
    [DataRow("'i")]
    [DataRow("a")]
    [DataRow("'ab'")]
    [DataRow("inva\"lid")]
    [DataRow("v'v'")]
    [DataRow("\"5\"")]
    [DataRow("'5'")]
    [DataRow(",5")]
    [DataRow("a500")]
    [DataRow("-")]
    [DataRow("5.")]
    [DataRow("5dd")]
    [DataRow("5c")]
    [DataRow("5 c")]
    public void Parse_invalid_numeric_returns_a_failure(string input)
    {
        // arrange
        var sut = new NumericParser();

        // act
        var subject = sut.Parse(input);

        // assert
        subject.IsSuccess.Should().BeFalse();
        var failure = subject.GetFailureUnsafe();

        failure.Span.Start.Position.Should().Be(0);
        failure.Span.Length.Should().Be(input.Length);
    }

    [TestMethod]
    [DataRow("5 notValid")]
    [DataRow("null notValid")]
    [DataRow("unique notValid")]
    [DataRow("10 5 asd")]
    [DataRow("2.5 asd 4")]
    [DataRow("null asd 10.0")]
    [DataRow("unique asd 12")]
    public void Parse_many_tokens_and_one_is_failing_returns_all_tokens_till_failure(string input)
    {
        // arrange
        var sut = new NumericParser();

        // act
        var subject = sut.Parse(input);

        // assert
        subject.IsFailure.Should().BeTrue();
    }

    #endregion
}