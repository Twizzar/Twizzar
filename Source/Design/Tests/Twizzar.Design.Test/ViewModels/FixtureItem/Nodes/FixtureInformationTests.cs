using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Design.Shared.CoreInterfaces.FixtureItem.Description;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Ui.Helpers;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Enums;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.FixtureInformations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.TestCommon;
using Twizzar.TestCommon.Builder;
using Twizzar.TestCommon.TypeDescription.Builders;
using TwizzarInternal.Fixture;
using static Twizzar.Design.TestCommon.DesignTestHelper;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.ViewModels.FixtureItem.Nodes;

[TestClass]
public class FixtureInformationTests
{
    #region static fields and constants

    private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();

    #endregion

    #region properties

    private static IMemberConfiguration RandomMemberConfig =>
        new ItemBuilder<IMemberConfiguration>()
            .With(p => p.Name.Unique())
            .Build();

    #endregion

    #region members

    [TestMethod]
    public void FixtureInformationTest()
    {
        // arrange
        var fixtureDescription = new TypeDescriptionBuilder().Build();
        var memberConfig = new Mock<IMemberConfiguration>();

        // act
        var sut = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            fixtureDescription,
            memberConfig.Object);

        // assert
        sut.CanBeExpanded.Should().BeFalse();
    }

    [TestMethod]
    public void LinkMemberConfiguration_can_be_expanded()
    {
        // arrange
        var fixtureDescription = Build.New<ITypeDescription>();
        var memberConfig = new LinkMemberConfiguration(RandomString(), RandomNamedFixtureItemId(), Source);

        // act
        var sut = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            fixtureDescription,
            memberConfig);

        sut.CanBeExpanded.Should().BeTrue();
    }

    [TestMethod]
    public void UndefinedMemberConfiguration_can_be_expanded()
    {
        // arrange
        var fixtureDescription = new TypeDescriptionBuilder().Build();

        var memberConfig =
            new UndefinedMemberConfiguration(RandomString(), TypeFullName.Create(RandomString()), Source);

        // act
        var sut = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            fixtureDescription,
            memberConfig);

        // assert
        sut.CanBeExpanded.Should().BeTrue();
    }

    [TestMethod]
    public void BaseType_can_not_be_expanded()
    {
        // arrange
        var fixtureDescription = new TypeDescriptionBuilder().AsBaseType().Build();

        var memberConfig =
            new UndefinedMemberConfiguration(RandomString(), TypeFullName.Create(RandomString()), Source);

        // act
        var sut = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            fixtureDescription,
            memberConfig);

        // assert
        sut.CanBeExpanded.Should().BeFalse();
    }

    [TestMethod]
    public void TypeFullName_is_the_type_full_name_of_the_fixtureDescription()
    {
        // arrange
        var typeName = RandomString();

        var fixtureDescription = new TypeDescriptionBuilder()
            .WithTypeFullName(new TypeFullNameBuilder(typeName).Build())
            .Build();

        var memberConfig = new Mock<IMemberConfiguration>().Object;

        // act
        var sut = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            fixtureDescription,
            memberConfig);

        // assert
        sut.TypeFullName.Should().Be(TypeFullName.Create(typeName));
    }

    [TestMethod]
    public void Id_is_provided_id()
    {
        // arrange
        var fixtureDescription = new TypeDescriptionBuilder().Build();
        var memberConfig = new Mock<IMemberConfiguration>().Object;
        var id = RandomNamedFixtureItemId();

        // act
        var sut = new FixtureItemInformation(id, RandomString(), fixtureDescription, memberConfig);

        // assert
        sut.Id.Should().Be(id);
    }

    [TestMethod]
    public void FixtureDescription_is_provided_FixtureDescription()
    {
        // arrange
        var fixtureDescription = new TypeDescriptionBuilder().Build();
        var memberConfig = new Mock<IMemberConfiguration>().Object;
        var id = RandomNamedFixtureItemId();

        // act
        var sut = new FixtureItemInformation(id, RandomString(), fixtureDescription, memberConfig);

        // assert
        sut.FixtureDescription.Should().Be(fixtureDescription);
    }

    [TestMethod]
    public void MemberConfiguration_is_provided_MemberConfiguration()
    {
        // arrange
        var fixtureDescription = new TypeDescriptionBuilder().Build();
        var memberConfig = new Mock<IMemberConfiguration>().Object;
        var id = RandomNamedFixtureItemId();

        // act
        var sut = new FixtureItemInformation(id, RandomString(), fixtureDescription, memberConfig);

        // assert
        sut.MemberConfiguration.Should().Be(memberConfig);
    }

    [TestMethod]
    [DynamicData(nameof(GeKindData), DynamicDataSourceType.Method)]
    public void Test_kind(object fixtureDescription, MemberKind memberKind)
    {
        // arrange
        var memberConfig = Mock.Of<IMemberConfiguration>();
        var id = RandomNamedFixtureItemId();

        // act
        var sut = new FixtureItemInformation(id,
            RandomString(),
            (IBaseDescription)fixtureDescription,
            memberConfig);

        // assert
        sut.Kind.Should().Be(memberKind);
    }

    [TestMethod]
    [DynamicData(nameof(GetModiferData), DynamicDataSourceType.Method)]
    public void Test_modifier(object accessModifier, MemberModifier memberModifier)
    {
        // arrange
        var memberConfig = new Mock<IMemberConfiguration>().Object;

        var fixtureDescription =
            new TypeDescriptionBuilder().WithAccessModifier((AccessModifier)accessModifier).Build();

        var id = RandomNamedFixtureItemId();

        // act
        var sut = new FixtureItemInformation(id, RandomString(), fixtureDescription, memberConfig);

        // assert
        sut.Modifier.Should().Be(memberModifier);
    }

    [TestMethod]
    [DynamicData(nameof(GetDisplayValueData), DynamicDataSourceType.Method)]
    public void Test_DisplayValue(object configuration, string expectedDisplayValue)
    {
        // arrange
        var id = RandomNamedFixtureItemId();

        var sut = new FixtureItemInformation(
            id,
            RandomString(),
            Mock.Of<ITypeDescription>(),
            (IMemberConfiguration)configuration);

        // act
        var displayValue = sut.DisplayValue;

        // assert
        displayValue.Should().Be(expectedDisplayValue);
    }

    [TestMethod]
    public void DisplayValue_of_ctor_is_correctly_formatted()
    {
        // arrange
        var parameterTypes = Enumerable.Range(0, RandomInt(1, 10))
            .Select(i => RandomDesignTypeFullName($"T{i}"))
            .ToArray();

        var parameterNames = string.Join(",", parameterTypes.Select(name => name.GetFriendlyCSharpTypeName()));
        var expectedDisplayValue = $"({parameterNames})";

        var methodDescription = new MethodDescriptionBuilder()
            .WithDeclaredParameter(
                parameterTypes.Select(s => new ParameterDescriptionBuilder().WithType(s).Build()))
            .Build<IDesignMethodDescription>(mock =>
                mock.Setup(description => description.FriendlyParameterTypes).Returns(parameterNames));

        var id = RandomNamedFixtureItemId();

        var sut = new FixtureItemInformation(
            id,
            RandomString(),
            methodDescription,
            new CtorMemberConfiguration(
                ImmutableArray<IMemberConfiguration>.Empty
                    .Add(RandomMemberConfig),
                ImmutableArray<ITypeFullName>.Empty,
                Source));

        // act
        var displayValue = sut.DisplayValue;

        // assert
        displayValue.Should().Be(expectedDisplayValue);
    }

    [TestMethod]
    [DynamicData(nameof(GetDisplayValueDataForValueConfig), DynamicDataSourceType.Method)]
    public void DisplayValue_of_string_and_char_are_correctly_formatted(
        string typeName,
        object value,
        string expectedDisplayValue)
    {
        // arrange
        var description = new TypeDescriptionBuilder()
            .AsBaseType()
            .WithTypeFullName(new TypeFullNameBuilder(typeName).Build())
            .Build();

        var configuration = new ValueMemberConfiguration(RandomString(), value, Source);

        var sut = new FixtureItemInformation(RandomNamedFixtureItemId(),
            RandomString(),
            description,
            configuration);

        // act
        var displayValue = sut.DisplayValue;

        // assert
        displayValue.Should().Be(expectedDisplayValue);
    }

    public static IEnumerable<object[]> GeKindData()
    {
        yield return new object[]
        {
            Mock.Of<IFieldDescription>(description => description.Name == ""),
            MemberKind.Field,
        };

        yield return new object[]
        {
            Mock.Of<IMethodDescription>(description =>
                description.Name == "" &&
                description.TypeFullName == new TypeFullNameBuilder(typeof(int)).Build()),
            MemberKind.Method,
        };

        yield return new object[]
        {
            Mock.Of<IPropertyDescription>(description => description.Name == ""),
            MemberKind.Property,
        };

        yield return new object[]
        {
            Mock.Of<IParameterDescription>(description => description.Name == ""),
            MemberKind.Field,
        };

        yield return new object[]
        {
            Mock.Of<ITypeDescription>(),
            MemberKind.Field,
        };
    }

    public static IEnumerable<object[]> GetModiferData()
    {
        yield return new object[]
        {
            AccessModifier.CreatePublic(),
            MemberModifier.Public,
        };

        yield return new object[]
        {
            AccessModifier.CreatePrivate(),
            MemberModifier.Private,
        };

        yield return new object[]
        {
            AccessModifier.CreateProtected(),
            MemberModifier.Protected,
        };

        yield return new object[]
        {
            new AccessModifier(false, false, false, true),
            MemberModifier.Internal,
        };
    }

    public static IEnumerable<object[]> GetDisplayValueData()
    {
        var namelessLink = RandomNamelessFixtureItemId();
        namelessLink = namelessLink.WithType(RandomDesignTypeFullName());

        yield return new object[]
        {
            new LinkMemberConfiguration(RandomString(), namelessLink, Source),
            $"{namelessLink.TypeFullName}",
        };

        yield return new object[]
        {
            new UndefinedMemberConfiguration(RandomString(), RandomTypeFullName(), Source),
            KeyWords.Undefined,
        };

        yield return new object[]
        {
            new UniqueValueMemberConfiguration(RandomString(), Source),
            KeyWords.Unique,
        };

        yield return new object[]
        {
            new NullValueMemberConfiguration(RandomString(), Source),
            KeyWords.Null,
        };

        yield return new object[]
        {
            new Mock<IMemberConfiguration>().Object,
            KeyWords.Undefined,
        };
    }

    public static IEnumerable<object[]> GetDisplayValueDataForValueConfig()
    {
        yield return new object[]
        {
            typeof(string).FullName,
            "Test",
            "\"Test\"",
        };

        yield return new object[]
        {
            typeof(string).FullName,
            " Test ",
            "\" Test \"",
        };

        yield return new object[]
        {
            typeof(char).FullName,
            'c',
            "'c'",
        };
    }

    #endregion
}