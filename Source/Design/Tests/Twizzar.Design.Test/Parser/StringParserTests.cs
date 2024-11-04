using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Parser;
using Twizzar.TestCommon;

namespace Twizzar.Design.Test.Parser;

[TestClass]
public class StringParserTests : BaseTypeParserTests
{
    #region properties

    /// <inheritdoc />
    public override BaseTypeParser BaseTypeParser => new StringParser();

    #endregion

    #region members

    [TestMethod]
    [DataRow("\"Valid\" ", "Valid")]
    [DataRow(" \"Valid\" ", "Valid")]
    [DataRow(" \"This is a valid string\" ", "This is a valid string")]
    [DataRow("\"\"", "")]
    [DataRow("\"\\\\\"", "\\\\")]
    [DataRow("\"unique\"", "unique")]
    [DataRow("\"null\"", "null")]
    [DataRow("      \"valid with many Whitespaces\"       ", "valid with many Whitespaces")]
    [DataRow("\"これはテストです \"       ", "これはテストです ")]
    [DataRow("\"這是一個測試\"       ", "這是一個測試")]
    [DataRow("\"هذا اختبار\"       ", "هذا اختبار")]
    [DataRow("\"\\\"\"", "\\\"")]
    [DataRow("\"TheseAreTheSupportedCharacters \\\' \\\\ \\\n \\\r \\\t \\\" \"", "TheseAreTheSupportedCharacters \\\' \\\\ \\\n \\\r \\\t \\\" ")]
    public void Parse_valid_string_returns_a_string_token(string input, string expectedString)
    {
        // arrange
        var sut = new StringParser();

        // act
        var subject = sut.Parse(input);

        // assert
        var token = TestHelper.AssertAndUnwrapSuccess(subject);
        var stringToken = token.Should().BeAssignableTo<IViStringToken>().Subject;
        stringToken.Text.Should().Be(expectedString);
        stringToken.Length.Should().Be(input.Length);
        stringToken.Start.Should().Be(0);
        stringToken.ContainingText.Should().Be(input);
    }

    [TestMethod]
    [DataRow("invalid")]
    [DataRow("\"invalid")]
    [DataRow("inva\"lid")]
    [DataRow("'invalid'")]
    [DataRow("5")]
    public void Parse_invalid_string_returns_a_failure(string input)
    {
        // arrange
        var sut = new StringParser();

        // act
        var subject = sut.Parse(input);

        // assert
        subject.IsSuccess.Should().BeFalse();
        var failure = subject.GetFailureUnsafe();

        failure.Span.Start.Position.Should().Be(0);
        failure.Span.Length.Should().Be(input.Length);
    }

    [TestMethod]
    [DataRow("\"valid\" notValid")]
    [DataRow("null notValid")]
    [DataRow("unique notValid")]
    [DataRow("notValid")]
    [DataRow("\"valid\" \"also valid\" asd")]
    [DataRow("\"valid\" asd \"also valid\"")]
    [DataRow("null asd \"also valid\"")]
    [DataRow("unique asd \"also valid\"")]
    public void Parse_many_tokens_and_one_is_failing_returns_all_tokens_till_failure(string input)
    {
        // arrange
        var sut = new StringParser();

        // act
        var subject = sut.Parse(input);

        // assert
        subject.IsFailure.Should().BeTrue();
    }

    #endregion
}