using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.FixtureInformations;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.TestCommon;
using ViCommon.Functional.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.ViewModels.FixtureItem.Nodes;

[TestClass]
public class ParameterNodeSenderTests
{
    private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();

    [TestMethod]
    public async Task Update_config_without_a_parent_throws_InternalException()
    {
        // arrange
        var sut = new ParameterNodeSender(Build.New<ICommandBus>(), Build.New<ISystemDefaultService>());

        // act
        Func<Task> func = () => sut.UpdateMemberConfigAsync(
            Build.New<IFixtureItemNode>(),
            Build.New<IMemberConfiguration>(),
            Build.New<IFixtureItemInformation>(),
            Maybe.None());

        // assert
        var exception = await func.Should()
            .ThrowAsync<InternalException>("Parameter should have always an parent.")
            .Map(assertions => assertions.Subject.First());

        exception.Message.Should().Be("The parameter node should have a parent.");
    }

    [TestMethod]
    public async Task Update_config_but_the_description_is_not_a_parameterDescription_throws_InternalException()
    {
        // arrange
        var sut = new ParameterNodeSender(Build.New<ICommandBus>(), Build.New<ISystemDefaultService>());

        // act
        Func<Task> func = () => sut.UpdateMemberConfigAsync(
            Build.New<IFixtureItemNode>(),
            Build.New<IMemberConfiguration>(),
            Mock.Of<IFixtureItemInformation>(
                information => information.FixtureDescription == Build.New<IBaseDescription>()),
            Maybe.Some(Build.New<IFixtureItemNode>()));

        // assert
        var exception = await func.Should()
            .ThrowAsync<InternalException>()
            .Map(assertions => assertions.Subject.First());

        exception.Message.Should().StartWith("The fixture description of the parameter should be");
    }

    [TestMethod]
    public async Task Update_config_but_the_parent_config_is_not_a_CtorMemberConfiguration_throws_InternalException()
    {
        // arrange
        var sut = new ParameterNodeSender(Build.New<ICommandBus>(), Build.New<ISystemDefaultService>());
        var parentFixtureInformation = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            Build.New<ITypeDescription>(),
            Build.New<IMemberConfiguration>());

        var parameterDescription = Build.New<IParameterDescription>();

        // act
        Func<Task> func = () => sut.UpdateMemberConfigAsync(
            Build.New<IFixtureItemNode>(),
            Build.New<IMemberConfiguration>(),
            Mock.Of<IFixtureItemInformation>(
                information => information.FixtureDescription == parameterDescription),
            Maybe.Some(Mock.Of<IFixtureItemNode>(
                node => node.FixtureItemInformation == parentFixtureInformation)));

        // assert
        var exception = await func.Should()
            .ThrowAsync<InternalException>()
            .Map(assertions => assertions.Subject.First());

        exception.Message.Should().StartWith($"The parent member configuration of a Parameter should be {nameof(CtorMemberConfiguration)} but is ");
    }

    [TestMethod]
    public async Task Update_config_sends_update_to_parent()
    {
        // arrange
        var sut = new ParameterNodeSender(
            Build.New<ICommandBus>(), 
            Build.New<ISystemDefaultService>());

        var paramName = RandomString();
        var ctorConfiguration = new CtorMemberConfiguration(
            ImmutableArray<IMemberConfiguration>.Empty
                .Add(new UniqueValueMemberConfiguration(paramName, Source)),
            ImmutableArray<ITypeFullName>.Empty,
            new FromUserInterface());
        var parameterConfig = Mock.Of<IMemberConfiguration>();
            
        var parentFixtureInformation = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            Mock.Of<IMemberDescription>(description => description.Name == ""),
            ctorConfiguration);

        var parent = new Mock<IFixtureItemNode>();
        parent
            .Setup(node => node.FixtureItemInformation)
            .Returns(parentFixtureInformation);

        // act
        await sut.UpdateMemberConfigAsync(
            Build.New<IFixtureItemNode>(),
            parameterConfig,
            Mock.Of<IFixtureItemInformation>(
                information => information.FixtureDescription == Mock.Of<IParameterDescription>(desc => desc.Name == paramName)),
            Maybe.Some(parent.Object));

        // assert
        var expectedConfig = ctorConfiguration.WithParameter(paramName, parameterConfig);

        parent.Verify(
            node =>
                node.CommitMemberConfig(
                    It.Is<CtorMemberConfiguration>(configuration =>
                        configuration.Name.Equals(expectedConfig.Name)
                        && configuration.ConstructorSignature.Equals(expectedConfig.ConstructorSignature) &&
                        configuration.ConstructorParameters.Count == expectedConfig.ConstructorParameters.Count)),
            Times.Once);
    }
}