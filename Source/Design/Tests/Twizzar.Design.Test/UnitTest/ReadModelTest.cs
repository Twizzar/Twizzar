using System.Collections.Immutable;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Design.CoreInterfaces.Query.Services.ReadModel;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using TwizzarInternal.Fixture;
using static Twizzar.TestCommon.TestHelper;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;

namespace Twizzar.Design.Test.UnitTest;

[TestClass]
public class ReadModelTest
{
    private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();

    [TestMethod]
    public void BaseTypeFixtureItemModel_with_returns_new_instance_with_changed_parameter()
    {
        // arrange
        var id = RandomNamedFixtureItemId();
        var value = new UniqueValueMemberConfiguration(RandomString(), Source);

        var otherId = RandomNamedFixtureItemId();
        var otherValue = new ValueMemberConfiguration(RandomString(), RandomString(), Source);

        var fixtureConfigs = ImmutableDictionary<string, IFixtureConfiguration>.Empty;

        var typeDescription = new Mock<ITypeDescription>();

        var sut = new BaseTypeFixtureItemModel(
            id,
            fixtureConfigs,
            value,
            typeDescription.Object);

        // act
        var differentId = sut.With(id: otherId);
        var differentValue = sut.With(value: otherValue);
        var differentIdAndValue = sut.With(id: otherId, value: otherValue);

        // assert
        differentId.Id.Should().Be(otherId);
        differentId.FixtureConfigurations.Should().BeEquivalentTo(fixtureConfigs);
        differentId.Value.Should().Be(value);

        differentValue.Id.Should().Be(id);
        differentValue.FixtureConfigurations.Should().BeEquivalentTo(fixtureConfigs);
        differentValue.Value.Should().Be(otherValue);

        differentIdAndValue.Id.Should().Be(otherId);
        differentIdAndValue.FixtureConfigurations.Should().BeEquivalentTo(fixtureConfigs);
        differentIdAndValue.Value.Should().Be(otherValue);
    }

    [TestMethod]
    public void FixtureItemMemberModel_with_returns_new_instance_with_changed_parameter()
    {
        // arrange
        var config = GetRandomMemberConfiguration();
        var otherConfig = GetRandomMemberConfiguration();

        var description = GetRandomMemberDescription();
        var otherDescription = GetRandomMemberDescription();

        var sut = new FixtureItemMemberModel(config, description);

        // act
        var differentConfig = sut.With(Some(otherConfig));
        var differentDescription = sut.With(description: Some(otherDescription));

        // assert
        differentConfig.Configuration.Should().Be(otherConfig);
        differentConfig.Description.Should().Be(description);

        differentDescription.Configuration.Should().Be(config);
        differentDescription.Description.Should().Be(otherDescription);
    }

    [TestMethod]
    public void FixtureItemParameterModel_with_returns_new_instance_with_changed_parameter()
    {
        // arrange
        var config = GetRandomMemberConfiguration();
        var otherConfig = GetRandomMemberConfiguration();

        var description = GetRandomParameterDescription();
        var otherDescription = GetRandomParameterDescription();

        var sut = new FixtureItemParameterModel(config, description);

        // act
        var differentConfig = sut.With(Some(otherConfig));
        var differentDescription = sut.With(description: Some(otherDescription));

        // assert
        differentConfig.Configuration.Should().Be(otherConfig);
        differentConfig.Description.Should().Be(description);

        differentDescription.Configuration.Should().Be(config);
        differentDescription.Description.Should().Be(otherDescription);
    }

    [TestMethod]
    public void ObjectFixtureItemModel_with_returns_new_instance_with_changed_parameter()
    {
        // arrange
        var id = RandomNamedFixtureItemId();
        var otherId = RandomNamedFixtureItemId();

        var fixtureConfigs = ImmutableDictionary<string, IFixtureConfiguration>.Empty;

        var usedCtor = None<FixtureItemConstructorModel>();
        var otherUsedCtor = Some(GetRandomFixtureItemConstructorModel());

        var properties = ImmutableDictionary<string, FixtureItemMemberModel>.Empty;
        var fields = ImmutableDictionary<string, FixtureItemMemberModel>.Empty;
        var methods = ImmutableDictionary<string, FixtureItemMemberModel>.Empty;
        var otherProperties = ImmutableDictionary<string, FixtureItemMemberModel>.Empty
            .Add(RandomString(), null);

        var declaredConstructors = ImmutableArray<IMethodDescription>.Empty;
        var otherDeclaredConstructors = ImmutableArray<IMethodDescription>.Empty
            .Add(GetRandomMethodDescription());

        var typeDescription = new Mock<ITypeDescription>();

        var sut = new ObjectFixtureItemModel(id, fixtureConfigs, usedCtor, properties, fields, methods, declaredConstructors, typeDescription.Object);

        // act
        var differentId = sut with { Id = otherId };
        var differentProperty = sut with {Properties = otherProperties };
        var differentCtors = sut with { DeclaredConstructors = otherDeclaredConstructors };
        var differentUsedCtors = sut with { UsedConstructor = otherUsedCtor };

        // assert
        differentId.Id.Should().Be(otherId);
        differentId.Properties.Should().BeEquivalentTo(properties);
        differentId.DeclaredConstructors.Should().BeEquivalentTo(declaredConstructors);

        differentProperty.Properties.Should().BeEquivalentTo(otherProperties);

        differentCtors.DeclaredConstructors.Should().BeEquivalentTo(otherDeclaredConstructors);

        differentUsedCtors.UsedConstructor.Should().Be(otherUsedCtor);
    }

    private static IMemberConfiguration GetRandomMemberConfiguration() =>
        new ItemBuilder<IMemberConfiguration>()
            .With(p => p.Name.Unique())
            .Build();

    private static IMemberDescription GetRandomMemberDescription() =>
        new Mock<IMemberDescription>().Object;

    private static IParameterDescription GetRandomParameterDescription() =>
        new Mock<IParameterDescription>().Object;

    private static IMethodDescription GetRandomMethodDescription() =>
        new Mock<IMethodDescription>().Object;

    private static FixtureItemConstructorModel GetRandomFixtureItemConstructorModel() =>
        new(
            ImmutableArray<FixtureItemParameterModel>.Empty,
            GetRandomMethodDescription(),
            new CtorMemberConfiguration(
                ImmutableArray<IMemberConfiguration>.Empty
                    .Add(GetRandomMemberConfiguration()), 
                ImmutableArray<ITypeFullName>.Empty,
                Source));

}