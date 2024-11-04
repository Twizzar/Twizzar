using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
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
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.Validator;

[TestClass]
public class BoolValidatorTests : BaseTypeValidatorTests
{
    #region members

    [TestMethod]
    [DynamicData(nameof(GetValidData), DynamicDataSourceType.Method)]
    public async Task Validation_returns_input_token_when_valid(object input)
    {
        // arrange
        var token = (IResult<IViToken, ParseFailure>)input;
        var sut = this.CreateSut(new TypeDescriptionBuilder().WithIsNullableBaseType(true).Build());

        // act
        var output = await sut.ValidateAsync(token);

        // assert
        output.Should().Be(token.GetSuccessUnsafe());
    }

    [TestMethod]
    [DynamicData(nameof(GetInvalidData), DynamicDataSourceType.Method)]
    public async Task Validation_returns_invalid_token_when_invalid(object input)
    {
        // arrange
        var token = (IResult<IViToken, ParseFailure>)input;
        var sut = this.CreateSut(new TypeDescriptionBuilder().Build());

        // act
        var output = await sut.ValidateAsync(token);

        // assert
        output.Should().BeAssignableTo<IViInvalidToken>();
    }

    [TestMethod]
    public void InputValues_of_nullable_bool_should_be_true_false_and_null()
    {
        // arrange
        var sut = this.CreateSut(new TypeDescriptionBuilder().WithIsNullableBaseType(true).Build());

        // act
        var validInput = sut.ValidInput.ToList();

        // assert
        validInput.Count.Should().Be(3);
        validInput.Should().Contain("true").And.Contain("false").And.Contain("null");
    }

    [TestMethod]
    public void InputValues_of_bool_should_be_true_false()
    {
        // arrange
        var sut = this.CreateSut(new TypeDescriptionBuilder().AsBaseType().Build());

        // act
        var validInput = sut.ValidInput.ToList();

        // assert
        validInput.Count.Should().Be(2);
        validInput.Should().Contain("true").And.Contain("false");
    }

    [TestMethod]
    public void InputValues_of_property_bool_should_be_true_false()
    {
        // arrange
        var sut = this.CreateSut(new PropertyDescriptionBuilder().WithBaseType(true).Build());

        // act
        var validInput = sut.ValidInput.ToList();

        // assert
        validInput.Count.Should().Be(3);
        validInput.Should().Contain("true").And.Contain("false").And.Contain("undefined");
    }

    [TestMethod]
    public void InputValues_of_property_nullable_bool_should_be_true_false()
    {
        // arrange
        var sut = this.CreateSut(new PropertyDescriptionBuilder().WithNullableBaseType(true).Build());

        // act
        var validInput = sut.ValidInput.ToList();

        // assert
        validInput.Count.Should().Be(4);
        validInput.Should().Contain("true").And.Contain("false").And.Contain("undefined").And.Contain("null");
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

        yield return new object[]
        {
            Success(
                CreateRandomToken(
                    (start, len, content) =>
                        new ViUniqueKeywordToken(start, len, content))),
        };
    }

    public static IEnumerable<object[]> GetValidData()
    {
        yield return new object[]
        {
            Success(GetRandomValidBoolToken()),
        };

        yield return new object[]
        {
            Success(
                CreateRandomToken(
                    (start, len, content) =>
                        new ViNullKeywordToken(start, len, content))),
        };
    }

    /// <inheritdoc />
    public override IValidator CreateSut(IBaseDescription typeDescription) =>
        new BoolValidator(typeDescription);

    private static IViToken GetRandomValidBoolToken() =>
        new ViBoolToken(
            RandomInt(0, 10),
            RandomInt(0),
            RandomString(),
            RandomBool());

    #endregion
}