using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Ui.Helpers;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Validator;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree.Keywords;
using Twizzar.Design.Ui.Parser.SyntaxTree.Link;
using Twizzar.Design.Ui.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Validator;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using Twizzar.TestCommon.TypeDescription.Builders;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;
using static Twizzar.Design.TestCommon.DesignTestHelper;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.Validator;

[TestClass]
public class ComplexValidatorTests : BaseValidatorTests
{
    #region members

    [TestMethod]
    public async Task ValidInput_contains_null_and_assignable_types()
    {
        // arrange
        var types = Enumerable.Repeat(0, RandomInt(1, 10))
            .Select(_ => RandomDesignTypeFullName())
            .ToList();

        // act
        var sut = this.CreateSutWithTypes(
            new TypeDescriptionBuilder().Build(),
            types);

        await sut.InitializeAsync();

        // assert
        sut.ValidInput.Should().Contain(KeyWords.Null);
        sut.ValidInput.Should().Contain(types.Select(name => name.FullName));
    }

    [TestMethod]
    public async Task LinkToken_which_is_assignable_returns_a_linkToken()
    {
        // arrange
        var pp = ParserPoint.New(string.Empty);
        var type = RandomDesignTypeFullName();

        var expectedLinkToken = ViLinkToken.Create(
            ViTypeToken.CreateWithoutWhitespaces(new ParserSpan(pp, pp), type.FullName, type.GetTypeFullNameToken()),
            ViLinkNameToken.CreateWithoutWhitespace(new ParserSpan(pp, pp), "#" + RandomString()));

        var linkOnlyToken = ViLinkToken.Create(
            Maybe.None(),
            ViLinkNameToken.CreateWithoutWhitespace(new ParserSpan(pp, pp), "#" + RandomString()));
            
        var sut = this.CreateSutWithTypes(
            new TypeDescriptionBuilder().WithTypeFullName(RandomDesignTypeFullName()).Build(),
            new[] { type });
        await sut.InitializeAsync();

        // act
        var result = await sut.ValidateAsync(Success(expectedLinkToken));
        var result2 = await sut.ValidateAsync(Success(linkOnlyToken));

        // assert
        result.Should().BeAssignableTo<IViLinkToken>();
        result2.Should().Be(linkOnlyToken);
    }

    [TestMethod]
    public async Task LinkToken_which_is_not_assignable_returns_a_invalidToken()
    {
        // arrange
        var pp = ParserPoint.New(string.Empty);
        var type = RandomDesignTypeFullName();

        var linkToken = ViLinkToken.Create(
            ViTypeToken.CreateWithoutWhitespaces(new ParserSpan(pp, pp), type.FullName, type.GetTypeFullNameToken()),
            ViLinkNameToken.CreateWithoutWhitespace(new ParserSpan(pp, pp), "#" + RandomString()));

        var sut = this.CreateSut(new TypeDescriptionBuilder().Build());

        // act
        var result = await sut.ValidateAsync(Success(linkToken));

        // assert
        result.Should().BeAssignableTo<ViInvalidToken>();
    }

    [TestMethod]
    public void DefaultValue_of_property_method_and_field_should_be_undefined()
    {
        // arrange
        var sut  = this.CreateSut(Mock.Of<IPropertyDescription>());
        var sut2 = this.CreateSut(Mock.Of<IFieldDescription>());
        var sut3 = this.CreateSut(Mock.Of<IMethodDescription>());
        var sut4 = this.CreateSut(Mock.Of<IMemberDescription>());

        // act
        var result = sut.DefaultValue;
        var result2 = sut2.DefaultValue;
        var result3 = sut3.DefaultValue;
        var result4 = sut4.DefaultValue;

        // assert
        result.Should().Be(KeyWords.Undefined);
        result2.Should().Be(KeyWords.Undefined);
        result3.Should().Be(KeyWords.Undefined);
        result4.Should().Be(KeyWords.Undefined);
    }

    [TestMethod]
    public void DefaultValue_of_type_and_parameter_should_be_typename()
    {
        // arrange
        var t1 = new TypeDescriptionBuilder().WithTypeFullName(RandomDesignTypeFullName()).Build();
        var t2 = new TypeDescriptionBuilder().WithTypeFullName(RandomDesignTypeFullName()).Build();

        var sut = this.CreateSut(t1);
        var sut2 = this.CreateSut(t2);

        // act
        var result = sut.DefaultValue;
        var result2 = sut2.DefaultValue;

        // assert
        result.Should().Be(t1.TypeFullName.GetFriendlyCSharpTypeName());
        result2.Should().Be(t2.TypeFullName.GetFriendlyCSharpTypeName());
    }

    [TestMethod]
    public async Task NullToken_returns_a_nullToken()
    {
        // arrange
        var nullToken = new ViNullKeywordToken(0, 10, RandomString());

        var sut = this.CreateSut(new TypeDescriptionBuilder().Build());

        // act
        var result = await sut.ValidateAsync(Success(nullToken));

        // assert
        result.Should().Be(nullToken);
    }

    [TestMethod]
    [DynamicData(nameof(GetInvalidData), DynamicDataSourceType.Method)]
    public async Task Test_invalid_validation(object input)
    {
        // arrange
        var sut = this.CreateSut(new TypeDescriptionBuilder().Build());

        // act
        var tokens = await sut.ValidateAsync((IResult<IViToken, ParseFailure>)input);

        // assert
        tokens.Should().BeAssignableTo<IViInvalidToken>();
    }

    public static IEnumerable<object[]> GetInvalidData()
    {
        yield return new object[]
        {
            Success(
                new ViCtorToken(0, 10, RandomString())),
        };

        yield return new object[]
        {
            Success(
                new ViInvalidToken(0, 10, RandomString(), RandomString())),
        };

        yield return new object[]
        {
            Success(
                ViStringToken.CreateWithoutWhitespaces(0, 10, RandomString())),
        };

        yield return new object[]
        {
            Result.Failure<IViToken, ParseFailure>(
                new ParseFailure(
                    RandomString(), new ParserSpan(ParserPoint.New(RandomString()), 10))),
        };
    }

    private ComplexValidator CreateSutWithTypes(ITypeDescription typeDescription, IEnumerable<ITypeFullName> types)
    {
        return new ComplexValidator(
            typeDescription,
            Mock.Of<IFixtureItemInformation>(info => info.Id == RandomNamelessFixtureItemId()),
            this.CreateAssignableTypesQuery(types));
    }

    /// <inheritdoc />
    public override IValidator CreateSut(IBaseDescription typeDescription) =>
        new ComplexValidator(
            typeDescription,
            Mock.Of<IFixtureItemInformation>(info => 
                info.Id == RandomNamelessFixtureItemId() &&
                info.FixtureDescription == new TypeDescriptionBuilder().Build()),
            this.CreateEmptyAssignableTypesQuery());

    private IAssignableTypesQuery CreateAssignableTypesQuery(IEnumerable<ITypeFullName> types)
    {
        var mock = new Mock<IAssignableTypesQuery>();

        mock.Setup(query =>
                query.IsAssignableTo(It.IsAny<IBaseDescription>(), It.IsAny<ITypeFullName>(), It.IsAny<Maybe<string>>()))
            .Returns((IBaseDescription baseDesc, TypeFullName typeFullName, Maybe<string> project) =>
                Task.FromResult(
                    types.Contains(typeFullName)
                        ? Maybe.Some(Mock.Of<IBaseDescription>(description => description.TypeFullName == typeFullName))
                        : Maybe.None()));

        mock.Setup(query => query.GetAssignableTypesAsync(
                It.IsAny<IBaseDescription>()))
            .Returns(Task.FromResult(types.Select(name => Mock.Of<IBaseDescription>(description => description.TypeFullName == name))));

        return mock.Object;
    }

    private IAssignableTypesQuery CreateEmptyAssignableTypesQuery()
    {
        return this.CreateAssignableTypesQuery(Enumerable.Empty<TypeFullName>());
    }

    #endregion
}