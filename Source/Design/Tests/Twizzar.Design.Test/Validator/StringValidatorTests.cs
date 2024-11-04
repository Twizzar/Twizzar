using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Helpers;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Validator;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree.Keywords;
using Twizzar.Design.Ui.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Validator;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using Twizzar.TestCommon.TypeDescription.Builders;
using ViCommon.Functional.Monads.ResultMonad;
using static Twizzar.Design.TestCommon.DesignTestHelper;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.Validator;

[TestClass]
public class StringValidatorTests : BaseTypeValidatorTests
{
    #region members

    [TestMethod]
    [DynamicData(nameof(GetValidData), DynamicDataSourceType.Method)]
    public async Task Validation_returns_input_token_when_valid(object input)
    {
        // arrange
        var token = (IResult<IViToken, ParseFailure>)input;
        var sut = new StringValidator(new TypeDescriptionBuilder().Build());

        // act
        var output = await sut.ValidateAsync(token);

        // assert
        output.Should().Be(token.GetSuccessUnsafe());
    }

    [TestMethod]
    public void ValidInput_contains_null_and_unique()
    {
        // arrange
        var sut = new StringValidator(new TypeDescriptionBuilder().WithTypeFullName(RandomDesignTypeFullName()).Build());

        // assert
        sut.ValidInput.Should().Contain(KeyWords.Null);
        sut.ValidInput.Should().Contain(KeyWords.Unique);
    }

    [TestMethod]
    [DynamicData(nameof(GetInvalidData), DynamicDataSourceType.Method)]
    public async Task Validation_returns_invalid_token_when_invalid(object input)
    {
        // arrange
        var token = (IResult<IViToken, ParseFailure>)input;
        var sut = new StringValidator(new TypeDescriptionBuilder().WithIsNullableBaseType(true).Build());

        // act
        var output = await sut.ValidateAsync(token);

        // assert
        output.Should().BeAssignableTo<IViInvalidToken>();
    }

    [TestMethod]
    public void ValidInput_contains_unique()
    {
        // arrange
        var sut = this.CreateSut(new TypeDescriptionBuilder().WithTypeFullName(RandomDesignTypeFullName()).WithIsNullableBaseType(false).Build());

        // assert
        sut.ValidInput.Should().Contain(KeyWords.Unique);
        sut.ValidInput.Should().Contain(KeyWords.Null);
    }

    public static IEnumerable<object[]> GetInvalidData()
    {
        yield return new object[]
        {
            Result.Success<IViToken, ParseFailure>(
                new ViCtorToken(0, 10, RandomString())),
        };

        yield return new object[]
        {
            Result.Success<IViToken, ParseFailure>(
                new ViInvalidToken(0, 10, RandomString(), RandomString())),
        };

        yield return new object[]
        {
            Result.Failure<IViToken, ParseFailure>(
                new ParseFailure(
                    RandomString(), new ParserSpan(ParserPoint.New(RandomString()), 10))),
        };
    }

    public static IEnumerable<object[]> GetValidData()
    {
        yield return new object[]
        {
            Result.Success<IViToken, ParseFailure>(
                ViStringToken.CreateWithoutWhitespaces(0, 10, RandomString())),
        };

        yield return new object[]
        {
            Result.Success<IViToken, ParseFailure>(
                new ViUniqueKeywordToken(0, 10, RandomString())),
        };

        yield return new object[]
        {
            Result.Success<IViToken, ParseFailure>(
                new ViNullKeywordToken(0, 10, RandomString())),
        };
    }

    /// <inheritdoc />
    public override IValidator CreateSut(IBaseDescription typeDescription) =>
        new StringValidator(typeDescription);

    #endregion
}