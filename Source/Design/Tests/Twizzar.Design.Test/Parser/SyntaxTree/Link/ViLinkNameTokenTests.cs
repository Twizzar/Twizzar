using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Parser.SyntaxTree.Link;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using Twizzar.TestCommon;

namespace Twizzar.Design.Test.Parser.SyntaxTree.Link;

[TestClass]
public class ViLinkNameTokenTests
{
    [TestMethod]
    public void CreateWithoutWhitespace_not_throws_on_valid_containingText()
    {
        // arrange
        var pp = ParserPoint.New(string.Empty);

        // act
        Func<ViLinkNameToken> func = () => ViLinkNameToken
            .CreateWithoutWhitespace(new ParserSpan(pp, pp), "#" + TestHelper.RandomString());

        // assert
        func.Should().NotThrow();
        func().Should().NotBeNull();
    }

    [TestMethod]
    public void CreateWithoutWhitespace_throws_when_containingText_does_not_start_with_hashset()
    {
        // arrange
        var pp = ParserPoint.New(string.Empty);

        // act
        Func<ViLinkNameToken> func = () => ViLinkNameToken
            .CreateWithoutWhitespace(new ParserSpan(pp, pp), "S" + TestHelper.RandomString());

        // assert
        var exp = func.Should().Throw<ArgumentException>().Subject.First();
        exp.ParamName.Should().Be("containingText");
    }

    [TestMethod]
    public void CreateWithoutWhitespace_throws_when_containingText_has_whitespaces()
    {
        // arrange
        var pp = ParserPoint.New(string.Empty);

        // act
        Func<ViLinkNameToken> func = () => ViLinkNameToken
            .CreateWithoutWhitespace(new ParserSpan(pp, pp), " " + TestHelper.RandomString());

        // assert
        var exp = func.Should().Throw<ArgumentException>().Subject.First();
        exp.ParamName.Should().Be("containingText");
    }
}