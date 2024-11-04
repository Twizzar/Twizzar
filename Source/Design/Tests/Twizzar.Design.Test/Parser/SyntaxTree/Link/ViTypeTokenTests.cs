using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Ui.Parser.SyntaxTree.Link;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using Twizzar.TestCommon;

namespace Twizzar.Design.Test.Parser.SyntaxTree.Link;

[TestClass()]
public class ViTypeTokenTests
{
    [TestMethod]
    public void CreateWithoutWhitespace_not_throws_on_valid_containingText()
    {
        // arrange
        var pp = ParserPoint.New(string.Empty);

        // act
        Func<ViTypeToken> func = () => ViTypeToken
            .CreateWithoutWhitespaces(new ParserSpan(pp, pp), TestHelper.RandomString(), Mock.Of<ITypeFullNameToken>());

        // assert
        func.Should().NotThrow();
    }

    [TestMethod]
    public void CreateWithoutWhitespace_throws_when_containingText_has_whitespaces()
    {
        // arrange
        var pp = ParserPoint.New(string.Empty);

        // act
        Func<ViTypeToken> func = () => ViTypeToken
            .CreateWithoutWhitespaces(new ParserSpan(pp, pp), " " + TestHelper.RandomString(), Mock.Of<ITypeFullNameToken>());

        // assert
        var exp = func.Should().Throw<ArgumentException>().Subject.First();
        exp.ParamName.Should().Be("containingText");
    }
}