using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Parser.SyntaxTree.Literals;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.Parser.SyntaxTree;

[TestClass()]
public class ViCharTokenTests
{
    [TestMethod]
    public void CreateWithoutWhitespaces_does_not_throw_on_valid_span()
    {
        // arrange
        var start = RandomInt(0);
        var containingText = RandomString().Substring(0, 3);

        // act
        Func<ViCharToken> func = () => ViCharToken.CreateWithoutWhitespaces(start, containingText);
            
        // assert
        var token = func.Should().NotThrow().Subject;
        token.Character.Should().Be(containingText[1]);
        token.Length.Should().Be(3);
        token.Start.Should().Be(start);
    }

    [TestMethod]
    public void CreateWithoutWhitespaces_throws_when_containingText_has_not_the_length_of_three()
    {
        // arrange
        var start = RandomInt(0);
        var containingText = RandomString();

        var length = RandomInt(0, 10) < 3
            ? RandomInt(0, 3)
            : RandomInt(4, containingText.Length);

        containingText = containingText.Substring(0, length);

        // act
        Func<ViCharToken> func = () => ViCharToken.CreateWithoutWhitespaces(start, containingText);

        // assert
        var exception = func.Should().Throw<ArgumentException>().Subject.First();
        exception.ParamName.Should().Be("containingText");
    }

    [TestMethod]
    public void Create_does_not_throw_when_span_has_length_of_three()
    {
        // arrange
        var expectedChar = RandomChar();

        var start = ParserPoint.New($"'{expectedChar}'");
        var end = start;

        while (end.HasNext)
        {
            end = end.Next();
        }

        // act
        Func<ViCharToken> func = () => ViCharToken.Create(start, end);

        // assert
        var token = func.Should().NotThrow().Subject;
        token.Character.Should().Be(expectedChar);
        token.Length.Should().Be(3);
        token.Start.Should().Be(start.Position);
    }

    [TestMethod]
    public void Create_throws_when_span_has_not_the_length_of_three()
    {
        // arrange
        RandomInt(0);
        var length = RandomInt(0, 10) < 3
            ? RandomInt(0, 3)
            : RandomInt(4, 1000);

        var start = ParserPoint.New(Enumerable.Repeat(RandomChar(), length).AsString());
        var end = start;

        while (end.HasNext)
        {
            end = end.Next();
        }

        // act
        Func<ViCharToken> func = () => ViCharToken.Create(start, end);

        // assert
        var exception = func.Should().Throw<ArgumentException>().Subject.First();
        exception.ParamName.Should().Be("start");
    }
}