using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.CoreInterfaces.Resources;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Link;
using Twizzar.Design.Ui.Validator;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using Twizzar.TestCommon.TypeDescription.Builders;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;
using static Twizzar.Design.TestCommon.DesignTestHelper;

namespace Twizzar.Design.Ui.Tests.Validator;

[TestFixture]
public partial class ComplexValidatorTests
{
    private ComplexValidator _sut;
    private IEnumerable<ITypeFullName> _assignableTypes;
    private ITypeDescription _typeDescription;

    [SetUp]
    public async Task SetUp()
    {
        var assignableTypes = Enumerable.Range(0, 5)
            .Select(i => RandomDesignTypeFullName())

            // last type name will be duplicated therefore a namespace is required
            .Append(RandomDesignTypeFullName(namespaceSegments: 2))
            .ToList();
        assignableTypes.Add(TypeFullName.Create("TestNamespace." + assignableTypes.Last().GetFriendlyCSharpTypeName()));
        this._assignableTypes = assignableTypes;

        this._typeDescription = new TypeDescriptionBuilder()
            .WithTypeFullName(RandomDesignTypeFullName())
            .WithIsClass(true)
            .Build();

        var typeQuery = new ItemBuilder<IAssignableTypesQuery>()
            .With(p => p.GetAssignableTypesAsync.Value(
                Task.FromResult(
                    this._assignableTypes
                        .Select(name => Mock.Of<IBaseDescription>(description =>
                            description.TypeFullName == name && description.IsClass)))))
            .Build();

        //var typeQuery = Build.New<IAssignableTypesQuery>(new EmptyIAssignableTypesQueryConfig()
        //    .Method(EmptyIAssignableTypesQueryConfig.GetAssignableTypesAsync_TaskIEnumerableIBaseDescription)
        //    .Value(Task.FromResult(
        //        this._assignableTypes
        //            .Select(name => Mock.Of<IBaseDescription>(description =>
        //                description.TypeFullName == name && description.IsClass)))));

        this._sut = new ItemBuilder<ComplexValidator>()
            .With(p => p.Ctor.assignableTypesQuery.Value(typeQuery))
            .With(p => p.Ctor.typeDescription.Value(this._typeDescription))
            .Build();

        await this._sut.InitializeAsync();
    }

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<ComplexValidator>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void Prettify_returns_same_token_when_not_IViLinkToken()
    {
        // arrange
        var sut = this._sut;

        // act
        var inputToken = new ItemBuilder<IViToken>().Build();
        var outputToken = sut.Prettify(inputToken);

        // assert
        outputToken.Should().Be(inputToken);
    }

    [Test]
    public void When_TypeToken_is_none_the_typeDescription_type_will_be_set_on_prettify()
    {
        var token = new ItemBuilder<IViLinkToken>().Build();

        this._sut.Prettify(token);

        Mock.Get(token).Verify(linkToken =>
                linkToken.WithTypeToken(It.Is<IViTypeToken>(
                    typeToken => Equals(
                        typeToken.TypeFullNameToken,
                        this._typeDescription.TypeFullName.GetTypeFullNameToken()))),
            Times.Once);
    }

    [Test]
    public async Task Tooltip_and_AdornerText_is_updated_after_validate()
    {
        var typeFullName = this._assignableTypes.First();

        var name = typeFullName.GetFriendlyCSharpTypeName();

        var typeToken = Maybe.Some(
            Mock.Of<IViTypeToken>(
                token =>
                    token.TypeFullNameToken == TypeFullNameParser.FriendlyTypeParser.Parse(name).GetSuccessUnsafe().Value));

        var token = new ItemBuilder<IViLinkToken>()
            .With(p => p.TypeToken.Value(typeToken))
            .Build()
            .ToSuccess<IViToken, ParseFailure>();

        this._sut.Tooltip.Should().Be(null);
        this._sut.AdornerText.Should().Be(null);

        // act
        await this._sut.ValidateAsync(token);

        // assert
        this._sut.Tooltip.Should().NotBe(null);
        this._sut.Tooltip.Should().Be(string.Format(MessagesDesign.ToolTip_WithoutLinkWithNotIsInterfaceType, typeFullName.GetFriendlyCSharpFullName(), string.Empty));
        this._sut.AdornerText.Should().NotBe(null);
        this._sut.AdornerText.Should().Be(MessagesDesign.Adorner_Class);
    }

    [Test]
    public void Single_assignable_type_only_typeName_is_given_is_returned_correctly()
    {
        var typeFullName = this._assignableTypes.First();
        var name = typeFullName.GetFriendlyCSharpTypeName();

        var typeToken = Maybe.Some(
            Mock.Of<IViTypeToken>(
                token =>
                    token.TypeFullNameToken == TypeFullNameParser.FriendlyTypeParser.Parse(name).GetSuccessUnsafe().Value));

        var token = new ItemBuilder<IViLinkToken>()
            .With(p => p.TypeToken.Value(typeToken))
            .Build();

        this._sut.Prettify(token);

        Mock.Get(token).Verify(linkToken =>
                linkToken.WithTypeToken(It.Is<IViTypeToken>(typeToken =>
                    Equals(typeToken.TypeFullNameToken, typeFullName.GetTypeFullNameToken()))),
            Times.Once);
    }

    [Test]
    public void Multiple_assignable_type_fullTypeName_is_given_is_returned_correctly()
    {
        var typeFullName = this._assignableTypes.Last();

        var typeToken = Maybe.Some(
            Mock.Of<IViTypeToken>(
                token =>
                    token.TypeFullNameToken == typeFullName.GetTypeFullNameToken()));

        var token = new ItemBuilder<IViLinkToken>()
            .With(p => p.TypeToken.Value(typeToken))
            .Build();

        this._sut.Prettify(token);

        Mock.Get(token).Verify(linkToken =>
                linkToken.WithTypeToken(It.Is<IViTypeToken>(typeToken =>
                    Equals(typeToken.TypeFullNameToken, typeFullName.GetTypeFullNameToken()))),
            Times.Once);
    }

    [Test]
    public void Multiple_assignable_type_only_typeName_is_given_is_returns_invalid()
    {
        // arrange
        var typeFullName = this._assignableTypes.Last();
        Console.WriteLine(typeFullName);

        var name = typeFullName.GetFriendlyCSharpTypeName();

        var typeFullNameToken = 
            TypeFullNameParser.FriendlyTypeParser.Parse(name).GetSuccessUnsafe().Value;

        var typeToken = Maybe.Some(
            Mock.Of<IViTypeToken>(
                token =>
                    token.TypeFullNameToken == typeFullNameToken));

        var token = Mock.Of<IViLinkToken>(linkToken => linkToken.TypeToken == typeToken);

        // act
        var result = this._sut.Prettify(token);

        Console.WriteLine(result is null);

        // assert
        result.Should().BeAssignableTo<IViInvalidToken>();
    }

    [Test]
    public void Not_assignable_only_returns_typeName()
    {
        var typeFullNameToken = RandomDesignTypeFullName().GetTypeFullNameToken();

        var typeToken = Mock.Of<IViTypeToken>(
            token =>
                token.TypeFullNameToken == typeFullNameToken);

        var token = new ItemBuilder<IViLinkToken>()
            .With(p => p.TypeToken.Value(Maybe.Some(typeToken)))
            .Build();

        this._sut.Prettify(token);

        Mock.Get(typeToken).Verify(x =>
                x.With(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    typeFullNameToken.ToFriendlyCSharpTypeName()),
            Times.Once);
    }
}