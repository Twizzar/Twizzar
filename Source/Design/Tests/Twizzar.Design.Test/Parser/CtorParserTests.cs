using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser;
using Twizzar.TestCommon;

namespace Twizzar.Design.Test.Parser;

[TestClass]
public class CtorParserTests
{
    #region members

    [TestMethod]
    public void When_parsing_any_text_returns_one_ctor_token()
    {
        // arrange
        var input = TestHelper.RandomString();
        var sut = new CtorParser();

        // act
        var result = sut.Parse(input);

        // assert
        var token = TestHelper.AssertAndUnwrapSuccess(result);
        var ctorToken = token.Should().BeAssignableTo<IViCtorToken>().Subject;
        ctorToken.Start.Should().Be(0);
        ctorToken.Length.Should().Be(input.Length);
        ctorToken.ContainingText.Should().Be(input);
    }

    #endregion
}