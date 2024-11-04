using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.Ui.Interfaces.Controller;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.FixtureInformations;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;
using static Twizzar.TestCommon.TestHelper;


namespace Twizzar.Design.Test.ViewModels.FixtureItem.Nodes;

[TestClass]
public class CtorFixtureItemNodeReceiverTests
{
    private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();

    [TestMethod]
    public void CtorFixtureItemNodeReceiverTest()
    {
        // assert
        Verify.Ctor<CtorFixtureItemNodeReceiver>()
            .IgnoreParameter("synchronizationContext", SynchronizationContext.Current)
            .IgnoreParameter("isListening", false)
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public void Throws_InternalException_when_the_memberConfig_is_not_a_CtorMemberConfig()
    {
        // arrange
        var memberName = RandomString();

        var oldMemberConfiguration = Mock.Of<IMemberConfiguration>(
            configuration =>
                configuration.Name == memberName);

        var newMemberConfiguration = Mock.Of<IMemberConfiguration>(
            configuration =>
                configuration.Name == memberName);

        var information = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            new ItemBuilder<ITypeDescription>().Build(),
            oldMemberConfiguration);

        var valueController = new ItemBuilder<IFixtureItemNodeValueController>().Build();

        var fixtureItemNode = Mock.Of<IFixtureItemNode>(
            node =>
                node.FixtureItemInformation == information &&
                node.NodeValueController == valueController);

        var sut = new CtorFixtureItemNodeReceiver(fixtureItemNode, true, SynchronizationContext.Current);

        // act
        Action action = () => sut.Handle(
            new FixtureItemMemberChangedEvent(
                information.Id,
                newMemberConfiguration));

        // assert
        action.Should().Throw<InternalException>();
    }

    [TestMethod]
    public void Children_with_parameterDescription_are_getting_updated()
    {
        var paramName = RandomString();

        // arrange
        var oldMemberConfiguration = new CtorMemberConfiguration(
            ImmutableArray<IMemberConfiguration>.Empty
                .Add(Mock.Of<IMemberConfiguration>(configuration => configuration.Name == paramName)),
            ImmutableArray<ITypeFullName>.Empty,
            Source);

        var newMemberConfiguration = Mock.Of<IMemberConfiguration>(
            configuration =>
                configuration.Name == oldMemberConfiguration.Name);

        var information = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            Mock.Of<IParameterDescription>(description => description.Name == paramName),
            oldMemberConfiguration);

        var valueController = Mock.Of<IFixtureItemNodeValueController>(controller =>
            controller.IsCommitDirty == false);

        var parameterChildren = Enumerable.Range(0, RandomInt(1, 5))
            .Select(
                i =>
                {
                    var child = new Mock<IFixtureItemNode>();
                    child
                        .Setup(node => node.FixtureItemInformation)
                        .Returns(information);

                    return child;
                })
            .ToList();

        var otherChildren = Enumerable.Range(0, RandomInt(1, 5))
            .Select(
                i =>
                {
                    var child = new Mock<IFixtureItemNode>();
                    child
                        .Setup(node => node.FixtureItemInformation)
                        .Returns(new FixtureItemInformation(
                            RandomNamedFixtureItemId(),
                            RandomString(),
                            Build.New<IMemberDescription>(),
                            Build.New<IMemberConfiguration>()));

                    return child;
                })
            .ToList();

        var fixtureItemNode = Mock.Of<IFixtureItemNode>(
            node =>
                node.FixtureItemInformation == information &&
                node.NodeValueController == valueController &&
                node.Children == parameterChildren.Concat(otherChildren).Select(mock => mock.Object));

        var sut = new CtorFixtureItemNodeReceiver(fixtureItemNode, true, SynchronizationContext.Current);

        // act
        sut.Handle(
            new FixtureItemMemberChangedEvent(
                information.Id,
                newMemberConfiguration));

        // assert
        parameterChildren.ForEach(
            mock => 
                mock.Verify(node => node.RefreshFixtureInformation(It.IsAny<IFixtureItemInformation>()), Times.Once()));

        otherChildren.ForEach(
            mock =>
                mock.Verify(node => node.RefreshFixtureInformation(It.IsAny<IFixtureItemInformation>()), Times.Never));
    }
}