using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Twizzar.Design.Ui.Interfaces.Enums;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Validator;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree.Keywords;
using Twizzar.Design.Ui.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Validator;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.Functional.Monads.MaybeMonad;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.Validator;

[TestClass]
public class ValidTokenToItemValueSegmentsConverterTest
{
    private readonly IValidTokenToItemValueSegmentsConverter sut =
        new ValidTokenToItemValueSegmentsConverter();

    private static readonly ParserPoint pp = ParserPoint.New(string.Empty);

    [TestMethod]
    [DynamicData(nameof(ViTokensData))]
    public void ToItemValueSegments_with_ViToken_returns_correct_ItemValueSegment(object obj, SegmentFormat format)
    {
        // act
        var token = (IViToken)obj;
        var result = this.sut.ToItemValueSegments(token).ToList();

        // assert
        result.Count.Should().Be(1);
        result.First().Content.Should().Be(token.ContainingText);
        result.First().Format.Should().Be(format);
    }

    [TestMethod]
    public void ToItemValueSegments_invalid_ViToken_returns_correct_ItemValueSegment()
    {
        // arrange
        var token = new Mock<IViToken>();
        var content = RandomString();
        token.Setup(t => t.ContainingText).Returns(content);

        // act
        var result = this.sut.ToItemValueSegments(token.Object).ToList();

        // assert
        result.Count.Should().Be(1);
        result.First().Content.Should().Be(content);
        result.First().Format.Should().Be(SegmentFormat.None);
    }

    public static IEnumerable<object[]> ViTokensData
    {
        get
        {
            yield return new object[] { new ViDefaultKeywordToken(0, 10, RandomString()), SegmentFormat.Keyword };
            yield return new object[] { new ViCtorToken(0, 1, RandomString()),SegmentFormat.SelectedCtor };
            yield return new object[] { new ViInvalidToken(0, 1, RandomString(), RandomString()), SegmentFormat.None };
            yield return new object[] { new ViBoolToken(0, 1,RandomString(), RandomBool()), SegmentFormat.Boolean };
            yield return new object[] { ViCharToken.CreateWithoutWhitespaces(0, $"\'{RandomChar()}\'"), SegmentFormat.Letter };
            yield return new object[] { NumericValidatorTests.GetRandomValidNumericToken(Maybe.None()), SegmentFormat.Number };
            yield return new object[] { new ViEnumToken(0, 1, RandomString(), RandomString(), RandomString(), RandomString()), SegmentFormat.Keyword };
        }
    }
}