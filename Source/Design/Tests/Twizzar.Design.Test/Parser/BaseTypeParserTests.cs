using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Keywords;
using Twizzar.Design.Ui.Parser;
using Twizzar.TestCommon;

namespace Twizzar.Design.Test.Parser;

[TestClass]
public abstract class BaseTypeParserTests
{
    public abstract BaseTypeParser BaseTypeParser { get; }

    [TestMethod]
    [DataRow("unique")]
    [DataRow(" unique")]
    [DataRow(" unique ")]
    [DataRow("unique ")]
    [DataRow("            unique    ")]
    public void Parse_valid_unique_keyword_returns_a_unique_keyword_token(string input)
    {
        // arrange
        var sut = this.BaseTypeParser;

        // act
        var subject = sut.Parse(input);

        // assert
        var token = TestHelper.AssertAndUnwrapSuccess(subject);

        token.Should().BeAssignableTo<IViKeywordToken>().And
            .BeAssignableTo<IViUniqueKeywordToken>();
        token.ContainingText.Should().Be(input);
        token.Start.Should().Be(0);
        token.Length.Should().Be(input.Length);
    }

    [TestMethod]
    [DataRow("null")]
    [DataRow(" null")]
    [DataRow(" null ")]
    [DataRow("null ")]
    [DataRow("null    ")]
    [DataRow("            null    ")]
    public void Parse_valid_null_keyword_returns_a_null_keyword_token(string input)
    {
        // arrange
        var sut = this.BaseTypeParser;

        // act
        var subject = sut.Parse(input);

        // assert
        var token = TestHelper.AssertAndUnwrapSuccess(subject);

        token.Should().BeAssignableTo<IViKeywordToken>().And
            .BeAssignableTo<IViNullKeywordToken>();
        token.ContainingText.Should().Be(input);
        token.Start.Should().Be(0);
        token.Length.Should().Be(input.Length);
    }

    [TestMethod]
    [DataRow("undefined")]
    [DataRow(" undefined")]
    [DataRow(" undefined ")]
    [DataRow("undefined ")]
    [DataRow("undefined    ")]
    [DataRow("            undefined    ")]
    public void Parse_valid_undefined_keyword_returns_a_undefined_keyword_token(string input)
    {
        // arrange
        var sut = this.BaseTypeParser;

        // act
        var subject = sut.Parse(input);

        // assert
        var token = TestHelper.AssertAndUnwrapSuccess(subject);

        token.Should().BeAssignableTo<IViKeywordToken>().And
            .BeAssignableTo<IViUndefinedKeywordToken>();
        token.ContainingText.Should().Be(input);
        token.Start.Should().Be(0);
        token.Length.Should().Be(input.Length);
    }
}