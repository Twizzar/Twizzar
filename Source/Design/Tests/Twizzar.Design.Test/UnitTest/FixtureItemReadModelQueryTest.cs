using System;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Twizzar.Design.Core.Query.Services;
using Twizzar.Design.CoreInterfaces.Common.FixtureItem.Description.Services;
using Twizzar.Design.CoreInterfaces.Query.Services.ReadModel;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.TestCommon.TypeDescription;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon.Configuration.Builders;
using Twizzar.TestCommon.TypeDescription.Builders;

using TwizzarInternal.Fixture;

using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

using static Twizzar.TestCommon.TestHelper;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;
using static ViCommon.Functional.Monads.ResultMonad.Result;

namespace Twizzar.Design.Test.UnitTest;

[TestClass]
public class FixtureItemReadModelQueryTest
{
    private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();

    [TestMethod]
    public async Task Query_returns_correct_baseType_viewModel()
    {
        // arrange
        var id = RandomNamedFixtureItemId();
        var value = RandomString();

        var description = new TypeDescriptionBuilder()
            .AsBaseType()
            .Build();
        var config = new ConfigurationItemBuilder().AsValueBaseType(value).Build();

        var sut = GetSut(id, description, config);

        // act
        var result = await sut.GetFixtureItem(id);

        // assert
        var model = AssertAndUnwrapSuccess(result);
        var baseTypeModel = model.Should().BeOfType<BaseTypeFixtureItemModel>().Subject;
        baseTypeModel.Id.Should().Be(id);
        var memberConfig = baseTypeModel.Value.Should().BeOfType<ValueMemberConfiguration>().Subject;
        memberConfig.Value.Should().Be(value);
    }

    [TestMethod]
    public void When_no_ConfigMember_with_the_constructor_key_is_declared_on_ConcreteComponent_throw_InternalException()
    {
        // arrange
        var id = RandomNamedFixtureItemId();

        var description = new TypeDescriptionBuilder()
            .AsClass()
            .WithDeclaredConstructors(new MethodDescriptionBuilder().AsConstructor().Build())
            .Build();
        var config = new ConfigurationItemBuilder()
            .WithCtorParameters(new[] { RandomMemberConfig }, false)
            .Build();

        var sut = GetSut(id, description, config);

        // act
        Func<Task> action = async () => await sut.GetFixtureItem(id);

        // assert
        action.Should().Throw<InternalException>();
    }

    [TestMethod]
    public void When_memberConfiguration_with_ctor_key_is_not_CtorMemberConfiguration_throw_InternalException()
    {
        // arrange
        var id = RandomNamedFixtureItemId();

        var description = new TypeDescriptionBuilder()
            .AsClass()
            .WithDeclaredConstructors(new MethodDescriptionBuilder().AsConstructor().Build())
            .Build();


        var config = new ConfigurationItemBuilder()
            .WithCtorParameters(new[] { RandomMemberConfig }, false)
            .WithMemberConfiguration(new UndefinedMemberConfiguration(
                ConfigurationConstants.CtorMemberName,
                TypeFullName.Create(RandomString()),
                Source))
            .Build();

        var sut = GetSut(id, description, config);

        // act
        Func<Task> action = async () => await sut.GetFixtureItem(id);

        // assert
        action.Should().Throw<InternalException>();
    }

    [TestMethod]
    public async Task Query_returns_correct_mock_viewModel()
    {
        // arrange
        var id = RandomNamedFixtureItemId();
        var value = RandomString();
        var propName = RandomString();
        var propMemberConfig = new ValueMemberConfiguration(propName, value, Source);
        var propDescription = new DesignPropertyDescriptionBuilder()
            .WithName(propName)
            .WithInterface(true)
            .Build();

        var propName2 = RandomString();
        var propMemberConfig2 = new ValueMemberConfiguration(propName2, RandomString(), Source);
        var propDescription2 = new DesignPropertyDescriptionBuilder()
            .WithName(propName2)
            .WithInterface(false)
            .WithCanWrite(true)
            .WithIsAutoProperty(true)
            .Build();

        var fieldName = RandomString();
        var fieldMemberConfig = new ValueMemberConfiguration(fieldName, RandomString(), Source);
        var fieldDescription = new DesignFieldDescriptionBuilder()
            .WithName(fieldName)
            .WithBackingfield(false)
            .Build();

        var fieldName2 = RandomString();
        var fieldMemberConfig2 = new ValueMemberConfiguration(fieldName2, RandomString(), Source);
        var fieldDescription2 = new DesignFieldDescriptionBuilder()
            .WithName(fieldName2)
            .WithBackingfield(true)
            .WithNoneWritableBackingfield()
            .Build();

        var description = new TypeDescriptionBuilder()
            .AsInterface()
            .WithDeclaredProperties(propDescription, propDescription2)
            .WithDeclaredFields(fieldDescription, fieldDescription2)
            .WithIsClass(true)
            .Build();

        var config = new ConfigurationItemBuilder()
            .WithPropertyMember(new IMemberConfiguration[]
            {
                propMemberConfig,
                propMemberConfig2,
                fieldMemberConfig,
                fieldMemberConfig2
            })
            .WithCtorParameters(new IMemberConfiguration[] { })
            .Build();

        var sut = GetSut(id, description, config);

        // act
        var result = await sut.GetFixtureItem(id);

        // assert
        var model = AssertAndUnwrapSuccess(result);
        var oModel = model.Should().BeOfType<ObjectFixtureItemModel>().Subject;
        oModel.Id.Should().Be(id);
        oModel.DeclaredConstructors.Should().HaveCount(0);
        oModel.Properties.Should().HaveCount(2);
        oModel.Fields.Should().HaveCount(2);
        oModel.Properties.Keys.Should().Contain(propName);
        oModel.Properties[propName].Configuration.Should().Be(propMemberConfig);
        oModel.Properties[propName].Description.Should().Be(propDescription);
    }

    [TestMethod]
    public async Task Query_returns_correct_class_viewModel()
    {
        // arrange
        var id = RandomNamedFixtureItemId();
        var value = RandomString();

        var ctorsParams = new[] { RandomString(), RandomString() };

        var description = new TypeDescriptionBuilder()
            .AsClass()
            .WithDeclaredConstructorsParams(ctorsParams)
            .Build();
        var config = new ConfigurationItemBuilder()
            .WithPropertyMember(new IMemberConfiguration[] { })
            .WithCtorParameters(new IMemberConfiguration[] {
                new ValueMemberConfiguration(ctorsParams[0], value, Source),
                new UniqueValueMemberConfiguration(ctorsParams[1], Source),
            })
            .Build();

        var sut = GetSut(id, description, config, Some(description.GetDeclaredConstructors()[0]));

        // act
        var result = await sut.GetFixtureItem(id);

        // assert
        var model = AssertAndUnwrapSuccess(result);
        var oModel = model.Should().BeOfType<ObjectFixtureItemModel>().Subject;
        oModel.Id.Should().Be(id);
        oModel.DeclaredConstructors.Should().HaveCount(1);
        oModel.Properties.Should().HaveCount(0);
        oModel.UsedConstructor.IsSome.Should().BeTrue();
        var ctorModel = oModel.UsedConstructor.GetValueUnsafe();
        ctorModel.Parameters.Should().HaveCount(2);
        ctorModel.Parameters[0].Description.Name.Should().Be(ctorsParams[0]);
        ctorModel.Parameters[1].Description.Name.Should().Be(ctorsParams[1]);
    }

    private static IMemberConfiguration RandomMemberConfig =>
        new ItemBuilder<IMemberConfiguration>()
            .With(p => p.Name.Unique())
            .Build();

    private static FixtureItemReadModelQuery GetSut(
        FixtureItemId id,
        ITypeDescription description,
        IConfigurationItem config,
        Maybe<IMethodDescription> methodDescription = default)
    {
        var descriptionQuery = new Mock<ITypeDescriptionQuery>();
        descriptionQuery.Setup(query => query.GetTypeDescriptionAsync(id.TypeFullName, id.RootItemPath))
            .Returns(
                SuccessAsync<ITypeDescription, Failure>(description));

        var configQuery = new Mock<IConfigurationItemQuery>();
        configQuery.Setup(query => query.GetConfigurationItem(id, description))
            .Returns(Task.FromResult(config));

        var ctorSelector = new Mock<ICtorSelector>();
        methodDescription.IfSome(d =>
            ctorSelector
                .Setup(selector => selector.FindCtor(config, description))
                .Returns(Some(d)));

        return new FixtureItemReadModelQuery(descriptionQuery.Object, configQuery.Object, ctorSelector.Object);
    }
}