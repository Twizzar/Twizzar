using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Interfaces.Validator;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree.Keywords;
using Twizzar.Design.Ui.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Validator;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using Twizzar.TestCommon.TypeDescription.Builders;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.Validator;

[TestClass]
public class NumericValidatorTests : BaseTypeValidatorTests
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
    [DataRow(typeof(double), 'd')]
    [DataRow(typeof(double), 'D')]
    [DataRow(typeof(decimal), 'm')]
    [DataRow(typeof(decimal), 'M')]
    [DataRow(typeof(float), 'f')]
    [DataRow(typeof(float), 'F')]
    public async Task Suffix_validation_returns_input_token_when_valid(Type type, char suffix)
    {
        // arrange
        var token = Success(GetRandomValidNumericToken(suffix));
        var sut = this.CreateSutValidatingFor(type);

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
    [DataRow(typeof(double), 'm')]
    [DataRow(typeof(double), 'F')]
    [DataRow(typeof(decimal), 'f')]
    [DataRow(typeof(decimal), 'D')]
    [DataRow(typeof(float), 'd')]
    [DataRow(typeof(float), 'M')]
    [DataRow(typeof(uint), 'M')]
    public async Task Suffix_validation_returns_invalid_token_when_invalid(Type type, char suffix)
    {
        // arrange
        var token = Success(GetRandomValidNumericToken(suffix));
        var sut = this.CreateSutValidatingFor(type);

        // act
        var output = await sut.ValidateAsync(token);

        // assert
        output.Should().BeAssignableTo<IViInvalidToken>();
    }

    [TestMethod]
    [DataRow(typeof(double), null, null)]
    [DataRow(typeof(double), 'd', 'd')]
    [DataRow(typeof(double), 'D', 'D')]
    [DataRow(typeof(float), null, 'f')]
    [DataRow(typeof(float), 'f', 'f')]
    [DataRow(typeof(float), 'F', 'F')]
    [DataRow(typeof(decimal), null, 'm')]
    [DataRow(typeof(decimal), 'm', 'm')]
    [DataRow(typeof(decimal), 'M', 'M')]
    public void Prettify_adds_suffix_when_not_present_and_mandatory(Type type, char? suffixBeforePrettify, char? suffixAfterPrettify)
    {
        // arrange
        var tokenBeforePrettify = GetRandomValidNumericToken(Maybe.ToMaybe(suffixBeforePrettify));
        var sut = this.CreateSutValidatingFor(type);

        // act
        var outputToken = sut.Prettify(tokenBeforePrettify);

        // assert
        var expectedToken = tokenBeforePrettify.With(tokenBeforePrettify.NumericWithSuffix with { Suffix = Maybe.ToMaybe(suffixAfterPrettify) });
        outputToken.Start.Should().Be(0);
        outputToken.ContainingText.Should().Be(expectedToken.ContainingText);
        outputToken.Length.Should().Be(expectedToken.ContainingText.Length);
        outputToken.Should()
            .BeAssignableTo<IViNumericToken>()
            .Which.NumericWithSuffix.Should().Be(expectedToken.NumericWithSuffix);
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
            Success(GetRandomValidNumericToken(Maybe.None())),
        };

        yield return new object[]
        {
            Success(
                CreateRandomToken(
                    (start, len, content) =>
                        new ViUniqueKeywordToken(start, len, content))),
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
        new NumericValidator(typeDescription);

    public IValidator CreateSutValidatingFor(Type type) =>
        this.CreateSut(new TypeDescriptionBuilder().WithTypeFullName(TypeFullName.CreateFromType(type)).Build());

    public static ViNumericToken GetRandomValidNumericToken(Maybe<char> suffix)
    {
        var randomNumeric = GetRandomNumericWithSuffix(suffix);
        var start = ParserPoint.New(randomNumeric.ToString());
        var span = new ParserSpan(start, randomNumeric.ToString().Length);

        return ViNumericToken.Create(span, randomNumeric);
    }

    #endregion
}