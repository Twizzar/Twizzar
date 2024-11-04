using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Keywords;
using Twizzar.Design.Ui.Parser;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.Parser;

[TestClass]
public class ComplexParserTests
{
    [TestMethod]
    [DataRow("TestType ")]
    [DataRow("    TestType ")]
    [DataRow("    TestType       ")]
    [DataRow("TypeWithNumbers123       ")]
    [DataRow("ExampleCode.Car ")]
    [DataRow("System.Collections.Generic.IEnumerable<string> ")]
    [DataRow("int[] ")]
    [DataRow("TestType ")]
    [DataRow("TestType ")]
    [DataRow("TestType ")]
    [DataRow("TestType ")]
    [DataRow("TestType<int> ")]
    [DataRow("TestType<int>")]
    [DataRow("TestType ")]
    [DataRow("اختبار ")]
    [DataRow("Контрольная_работа")]
    [DataRow("測試 ")]
    [DataRow("テスト ")]
    public void Parse_valid_link_returns_a_valid_link_token(string type)
    {
        // arrange
        var input = $"{type}";
        var sut = new ComplexParser();

        // act
        var result = sut.Parse(input);

        // assert
        var token = AssertAndUnwrapSuccess(result);
        var linkToken = token.Should().BeAssignableTo<IViLinkToken>().Subject;
        linkToken.ContainingText.Should().Be(input);
        linkToken.Start.Should().Be(0);
        linkToken.Length.Should().Be(input.Length);

        // type token
        linkToken.TypeToken.Should().NotBeNull();
        var maybeTypeToke = linkToken.TypeToken;

        maybeTypeToke.IsSome.Should().BeTrue();
        var typeToken = maybeTypeToke.GetValueUnsafe();
        typeToken.TypeFullNameToken.ContainingText.Should().Be(type.Trim());
        typeToken.Start.Should().Be(0);
        typeToken.Length.Should().Be(typeToken.Length);
    }

    [TestMethod]
    [DataRow("default")]
    [DataRow("  default")]
    [DataRow("  default  ")]
    [DataRow("default   ")]
    [DataRow("   default   ")]
    public void Parse_default_returns_default_token(string input)
    {
        // arrange
        var sut = new ComplexParser();

        // act
        var result = sut.Parse(input);

        // assert
        var defaultToken = AssertAndUnwrapSuccess(result).Should().BeAssignableTo<IViDefaultKeyword>().Subject;
        defaultToken.Start.Should().Be(0);
        defaultToken.Length.Should().Be(input.Length);
        defaultToken.ContainingText.Should().Be(input);
    }

    [TestMethod]
    [DataRow("Test#Type")]
    [DataRow("TestType #Id#")]
    [DataRow("Test#Type")]
    [DataRow("T#estType")]
    [DataRow("T$estType")]
    public void Parse_invalid_link_returns_a_failure(string input)
    {
        // arrange
        var sut = new ComplexParser();

        // act
        var result = sut.Parse(input);

        // assert
        result.IsFailure.Should().BeTrue();
    }

    [TestMethod]
    [DataRow("Car")]
    [DataRow("  Car   ")]
    [DataRow("Some.NameSpace.MyType   ")]
    public void Only_type_results_in_successful_parse(string input)
    {
        // arrange
        var sut = new ComplexParser();

        // act
        var result = sut.Parse(input);

        // assert
        AssertAndUnwrapSuccess(result).Should().BeAssignableTo<IViLinkToken>();
    }

    [TestMethod]
    [DataRow("null")]
    [DataRow("    null")]
    [DataRow("null       ")]
    [DataRow("     null  ")]
    public void Parse_null_returns_success_for_nullable_complex_parser(string input)
    {
        // arrange
        var sut = new NullableComplexParser();

        // act
        var result = sut.Parse(input);

        // assert
        result.IsSuccess.Should().BeTrue();
    }
}