using System;
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
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.ViewModels.FixtureItem.Nodes;

[TestClass]
public class FixtureItemNodeReceiverTests
{
    [TestMethod]
    public void All_ctor_parameter_throw_argumentNullException_when_null()
    {
        //assert
        Verify.Ctor<FixtureItemNodeReceiver>()
            .IgnoreParameter("synchronizationContext", SynchronizationContext.Current)
            .IgnoreParameter("isListening", false)
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public void FixtureInformationChanged_is_not_called_when_the_fixtureItemId_is_different()
    {
        // arrange
        var memberName = RandomString();

        var memberConfiguration = Mock.Of<IMemberConfiguration>(
            configuration =>
                configuration.Name == memberName);

        var information = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            Build.New<ITypeDescription>(),
            memberConfiguration);

        var fixtureItemNode = Mock.Of<IFixtureItemNode>(
            node =>
                node.FixtureItemInformation == information);

        var sut = new FixtureItemNodeReceiver(fixtureItemNode, false, SynchronizationContext.Current);

        int callCount = 0;
        sut.FixtureInformationChanged += _ => callCount++;

        // act
        sut.Handle(new FixtureItemMemberChangedEvent(RandomNamedFixtureItemId(), memberConfiguration));

        // assert
        callCount.Should().Be(0);
    }

    [TestMethod]
    public void FixtureInformationChanged_is_not_called_when_the_memberName_is_different()
    {
        // arrange
        var memberName = RandomString();

        var memberConfiguration = Mock.Of<IMemberConfiguration>(
            configuration =>
                configuration.Name == memberName);

        var information = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            Build.New<ITypeDescription>(),
            memberConfiguration);

        var fixtureItemNode = Mock.Of<IFixtureItemNode>(
            node =>
                node.FixtureItemInformation == information);

        var sut = new FixtureItemNodeReceiver(fixtureItemNode, false, SynchronizationContext.Current);

        int callCount = 0;
        sut.FixtureInformationChanged += _ => callCount++;

        // act
        sut.Handle(
            new FixtureItemMemberChangedEvent(
                information.Id,
                Mock.Of<IMemberConfiguration>(
                    configuration =>
                        configuration.Name == RandomString(""))));

        // assert
        callCount.Should().Be(0);
    }

    [TestMethod]
    public void FixtureInformationChanged_is_not_called_when_the_memberConfigs_are_the_same()
    {
        // arrange
        var memberName = RandomString();

        var memberConfiguration = Mock.Of<IMemberConfiguration>(
            configuration =>
                configuration.Name == memberName);

        var information = new FixtureItemInformation(
            RandomNamedFixtureItemId(),
            RandomString(),
            Build.New<ITypeDescription>(),
            memberConfiguration);

        var valueController = Mock.Of<IFixtureItemNodeValueController>(controller =>
            controller.IsCommitDirty == false);

        var fixtureItemNode = Mock.Of<IFixtureItemNode>(
            node =>
                node.FixtureItemInformation == information &&
                node.NodeValueController == valueController);

        var sut = new FixtureItemNodeReceiver(fixtureItemNode, false, SynchronizationContext.Current);

        int callCount = 0;
        sut.FixtureInformationChanged += _ => callCount++;

        // act
        sut.Handle(
            new FixtureItemMemberChangedEvent(
                information.Id,
                memberConfiguration));

        // assert
        callCount.Should().Be(0);
    }

    [TestMethod]
    public void FixtureInformationChanged_new_fixtureInformation_is_correctly_updated()
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
            Build.New<ITypeDescription>(),
            oldMemberConfiguration);

        var expectedInformation = information.With(newMemberConfiguration);

        var valueController = Mock.Of<IFixtureItemNodeValueController>(controller =>
            controller.IsCommitDirty == false);

        var fixtureItemNode = Mock.Of<IFixtureItemNode>(
            node =>
                node.FixtureItemInformation == information &&
                node.NodeValueController == valueController);

        var sut = new FixtureItemNodeReceiver(fixtureItemNode, true, SynchronizationContext.Current);

        IFixtureItemInformation outputItemInformation = null;
        sut.FixtureInformationChanged += info => outputItemInformation = info;

        // act
        sut.Handle(
            new FixtureItemMemberChangedEvent(
                information.Id,
                newMemberConfiguration));

        // assert
        outputItemInformation.Should().Be(expectedInformation);
    }

    [TestMethod]
    public void On_Handle_FixtureItemMemberChangedFailedEvent_the_node_gets_called()
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
            Build.New<ITypeDescription>(),
            oldMemberConfiguration);

        var valueController = Mock.Of<IFixtureItemNodeValueController>(controller =>
            controller.IsCommitDirty == false);

        var fixtureItemNode = Mock.Of<IFixtureItemNode>(
            node =>
                node.FixtureItemInformation == information &&
                node.NodeValueController == valueController);

        var e = new FixtureItemMemberChangedFailedEvent(
            information.Id,
            newMemberConfiguration,
            "Test reason");

        var sut = new FixtureItemNodeReceiver(fixtureItemNode, true, SynchronizationContext.Current);

        // act
        sut.Handle(e);
        sut.Handle(new FixtureItemMemberChangedFailedEvent(
            RandomNamedFixtureItemId(),
            newMemberConfiguration,
            RandomString()));
        sut.Handle(new FixtureItemMemberChangedFailedEvent(
            information.Id,
            Build.New<IMemberConfiguration>(),
            RandomString()));

        // assert
        Mock.Get(fixtureItemNode).Verify(node => node.DisplayOnMemberChangedFailed(e), Times.Once);
    }

    [TestMethod]
    public void IsListening_is_false_on_disposed()
    {
        // arrange
        var receiver = Build.New<FixtureItemNodeReceiver>();

        // act
        receiver.Dispose();

        // assert
        receiver.IsListening.Should().BeFalse();
    }

    [TestMethod]
    public void When_disposed_throw_exception_on_dispose_call()
    {
        // arrange
        var receiver = Build.New<FixtureItemNodeReceiver>();

        // act
        receiver.Dispose();
        Action action = () => receiver.Dispose();

        // assert
        action.Should().Throw<ObjectDisposedException>();
    }
}