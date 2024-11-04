using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Parser;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.Parser;

[TestClass]
public class BooleanParserTests : BaseTypeParserTests
{
    [TestMethod]
    [DataRow("true", true)]
    [DataRow("   true", true)]
    [DataRow("true    ", true)]
    [DataRow("     true    ", true)]
    [DataRow("false", false)]
    [DataRow("    false", false)]
    [DataRow("false    ", false)]
    [DataRow("    false    ", false)]
    public void Valid_input_gets_parsed_correctly(string input, bool value)
    {
        // arrange
        var sut = new BoolParser();

        // act
        var result = ((BaseParser)sut).Parse(input);

        // assert
        var token = AssertAndUnwrapSuccess(result);
        var boolToken = token.Should().BeAssignableTo<IViBoolToken>().Subject;
        boolToken.ContainingText.Should().Be(input);
        boolToken.Length.Should().Be(input.Length);
        boolToken.Start.Should().Be(0);
        boolToken.Boolean.Should().Be(value);
    }

    [TestMethod]
    [DataRow("\"false\"")]
    [DataRow("\"true\"")]
    [DataRow("'false'")]
    [DataRow("'true'")]
    [DataRow("TRUE")]
    [DataRow("False")]
    [DataRow("1")]
    [DataRow("0")]
    [DataRow("1 false")]
    [DataRow("this is true")]
    public void Invalid_input_returns_failure(string input)
    {
        // arrange
        var sut = new BoolParser();

        // act
        var result = ((BaseParser)sut).Parse(input);

        // assert
        result.IsFailure.Should().BeTrue();
    }

    #region Overrides of BaseTypeParserTests

    /// <inheritdoc />
    public override BaseTypeParser BaseTypeParser => new BoolParser();

    #endregion
}