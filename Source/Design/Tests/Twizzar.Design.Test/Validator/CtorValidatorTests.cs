using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree.Keywords;
using Twizzar.Design.Ui.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Validator;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.Functional.Monads.ResultMonad;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.Validator;

[TestClass]
public class CtorValidatorTests
{
    #region properties

    public static ParseFailure RandomFailure =>
        new(
            RandomString(), new ParserSpan(ParserPoint.New(RandomString()), RandomInt(0, 10)));

    #endregion

    #region members

    [TestMethod]
    public async Task Test_valid_validation()
    {
        // arrange
        var content = RandomString();

        var sut = new CtorValidator(new Mock<IMethodDescription>().Object);

        // act
        var token = await sut.ValidateAsync(Success(new ViCtorToken(0, 10, content)));

        // assert
        var ctorToken = token.Should().BeAssignableTo<IViCtorToken>().Subject;
        ctorToken.ContainingText.Should().Be(content);
        ctorToken.Length.Should().Be(10);
        ctorToken.Start.Should().Be(0);
    }

    [TestMethod]
    [DynamicData(nameof(GetInvalidData), DynamicDataSourceType.Method)]
    public async Task Test_invalid_validation(object input)
    {
        // arrange
        var sut = new CtorValidator(new Mock<IMethodDescription>().Object);

        // act
        var token = await sut.ValidateAsync((IResult<IViToken, ParseFailure>)input);

        // assert
        token.Should().BeAssignableTo<IViInvalidToken>();
    }

    public static IEnumerable<object[]> GetInvalidData()
    {
        yield return new object[]
        {
            Success(
                CreateRandomToken(ViStringToken.CreateWithoutWhitespaces)),
        };

        yield return new object[]
        {
            Success(
                CreateRandomToken(
                    (start, end, content) =>
                        new ViNullKeywordToken(start, end, content))),
        };

        yield return new object[]
        {
            Failure(RandomFailure),
        };
    }

    public static IResult<IViToken, ParseFailure> Success(IViToken token) =>
        Result.Success<IViToken, ParseFailure>(token);

    public static IResult<IViToken, ParseFailure> Failure(ParseFailure parseFailure) =>
        Result.Failure<IViToken, ParseFailure>(parseFailure);

    public static IViToken CreateRandomToken(Func<int, int, string, IViToken> creator) =>
        creator(RandomInt(0, 5), RandomInt(0, 5), RandomString());

    #endregion
}