using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Validator;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using Twizzar.TestCommon;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Test.Validator;

[TestClass]
public partial class EnumValidatorTests
{
    [TestMethod]
    [DataRow(new[] { "a", "b", "c" }, "a")]
    [DataRow(new[] { "a", "b", "c" }, "b")]
    [DataRow(new[] { "a", "b", "c" }, "c")]
    public async Task Valid_enumName_result_in_success(string[] enumValues, string tokenEnumName)
    {
        // arrange
        var typeDescription = Mock.Of<ITypeDescription>(
            description =>
                description.GetEnumNames() == Maybe.Some(enumValues));


        var baseDescription = Mock.Of<IBaseDescription>(
            description =>
                description.GetReturnTypeDescription() == typeDescription &&
                description.TypeFullName == TestHelper.RandomTypeFullName("", -1));
            
        var token = Mock.Of<IViEnumToken>(enumToken => 
            enumToken.EnumName == tokenEnumName &&
            enumToken.EnumType == Maybe.Some(baseDescription.TypeFullName.FullName));

        var sut = new EnumValidator(baseDescription);

        // act
        var result = await sut.ValidateAsync(Result.Success<IViToken, ParseFailure>(token));

        // assert
        result.Should().BeAssignableTo<IViEnumToken>();
    }

    [TestMethod]
    public async Task When_input_token_is_not_assignable_to_IViEnumToke_return_failure()
    {
        // arrange
        var typeDescription = Mock.Of<ITypeDescription>(
            description =>
                description.GetEnumNames() == Maybe.None<string[]>());


        var baseDescription = new EmptyIBaseDescriptionBuilder()
            .With(p => p.GetReturnTypeDescription.Value(typeDescription))
            .Build();

        var sut = new EnumValidator(baseDescription);
        var token = Build.New<IViToken>();

        // act
        var result = await sut.ValidateAsync(Result.Success<IViToken, ParseFailure>(token));

        // assert
        result.Should().BeAssignableTo<IViInvalidToken>();
    }

    [TestMethod]
    public async Task When_input_a_failure_return_failure()
    {
        // arrange
        var typeDescription = Mock.Of<ITypeDescription>(
            description =>
                description.GetEnumNames() == Maybe.None<string[]>());


        var baseDescription = new EmptyIBaseDescriptionBuilder()
            .With(p => p.GetReturnTypeDescription.Value(typeDescription))
            .Build();

        var sut = new EnumValidator(baseDescription);

        // act
        var parsePoint = ParserPoint.New(string.Empty);//"ZeroPoint");
        var parseSpan = new ParserSpan(parsePoint, parsePoint);

        var parseFailure = new ParseFailure(Guid.NewGuid().ToString(), parseSpan);
        var result = await sut.ValidateAsync(
            Result.Failure<IViToken, ParseFailure>(parseFailure));

        // assert
        result.Should().BeAssignableTo<IViInvalidToken>();
    }
}