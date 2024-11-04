using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;
namespace Twizzar.Design.Ui.Tests.ViewModels.FixtureItem.Nodes;

[TestFixture]
public partial class MethodNodeSenderTests
{
    private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<MethodNodeSender>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public async Task UpdateMemberConfigAsync_ChangeMemberConfigurationCommand_is_send()
    {
        // arrange
        var commandBus = new Mock<ICommandBus>();
        var sut = new ItemBuilder<MethodNodeSender>()
            .With(p => p.Ctor.commandBus.Value(commandBus.Object))
            .Build();
        var id = TestHelper.RandomNamedFixtureItemId();
        var methodName = TestHelper.RandomString();

        var methodConfig = MethodConfiguration.Create(
            methodName,
            methodName,
            Source,
            new UniqueValueMemberConfiguration(methodName, Source), 
            typeof(int),
            nameof(Int32));

        var current = Build.New<IFixtureItemNode>();

        var information = new ItemBuilder<IFixtureItemInformation>()
            .With(p => p.Id.Value(id))
            .With(p => p.MemberConfiguration.Value(methodConfig))
            .With(p => p.TypeFullName.Value(TypeFullName.Create("System.Int32")))
            .Build();

        // act
        await sut.UpdateMemberConfigAsync(
            current,
            Build.New<IMemberConfiguration>(),
            information,
            Maybe.None());

        // assert
        commandBus.Verify(bus => bus.SendAsync(It.IsAny<ChangeMemberConfigurationCommand>()), Times.Once);
    }

    [Test]
    public async Task UpdateMemberConfigAsync_Sends_no_comments_when_current_is_dirty()
    {
        var sut = new EmptyMethodNodeSenderBuilder().Build(out var context);

        var config = Build.New<IMemberConfiguration>();
        var current = Build.New<IFixtureItemNode>();
        var information =
            Mock.Of<IFixtureItemInformation>(itemInformation => itemInformation.MemberConfiguration == config);

        await sut.UpdateMemberConfigAsync(current, config, information, Maybe.None());

        context.Verify(p => p.Ctor.commandBus.SendAsyncTCommand)
            .Called(0);
    }

    [Test]
    public void UpdateMemberConfigAsync_when_not_MethodConfiguration_throw_InternalException()
    {
        var sut = new MethodNodeSenderWithStubLoggerBuilder().Build();

        var config = Build.New<IMemberConfiguration>();
        var current = Build.New<IFixtureItemNode>();
        var information =
            Mock.Of<IFixtureItemInformation>(itemInformation => 
                itemInformation.MemberConfiguration == Build.New<IMemberConfiguration>());

        Func<Task> action = ()=> sut.UpdateMemberConfigAsync(current, config, information, Maybe.None());
        action.Should().Throw<InternalException>();
    }

    [Test]
    public void UpdateMemberConfigAsync_FixtureInformation_TypeFullName_is_not_in_MethodReturnConfigurations_throw_InternalException()
    {
        var sut = new MethodNodeSenderWithStubLoggerBuilder().Build();
        var methodName = TestHelper.RandomString();

        var methodConfig = MethodConfiguration.Create(
            methodName,
            methodName,
            Source,
            new UniqueValueMemberConfiguration(methodName, Source),
            typeof(string),
            nameof(Int32));

        var current = Build.New<IFixtureItemNode>();//"IFixtureItemNode22a2");
        var information =
            Mock.Of<IFixtureItemInformation>(itemInformation => 
                itemInformation.MemberConfiguration == Build.New<IMemberConfiguration>());

        var action = ()=> sut.UpdateMemberConfigAsync(current, methodConfig, information, Maybe.None());

        action.Should().Throw<InternalException>();
    }
}