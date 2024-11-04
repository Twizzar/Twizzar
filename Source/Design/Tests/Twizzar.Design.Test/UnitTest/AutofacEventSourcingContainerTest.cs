using Autofac;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.Infrastructure.Command.Services;

namespace Twizzar.Design.Test.UnitTest;

[TestClass]
[TestCategory("Obsolete")]
public class AutofacEventSourcingContainerTest
{
    [TestMethod]
    public void Can_resolve_CommandHandlers()
    {
        var handler = new Mock<ICommandHandler<TestCommand>>();
        var builder = new ContainerBuilder();
        builder.RegisterInstance(handler.Object)
            .As<ICommandHandler<TestCommand>>()
            .SingleInstance();

        var sut = new AutofacEventSourcingContainer(builder.Build());

        var result = sut.GetCommandHandler<TestCommand>();

        result.IsSome.Should().BeTrue();
        result.GetValueUnsafe().Should().BeAssignableTo<ICommandHandler<TestCommand>>();
    }

    [TestMethod]
    public void Can_resolve_EventListeners()
    {
        var listener = new Mock<IEventListener<TestEvent>>();
        var builder = new ContainerBuilder();
        builder.RegisterInstance(listener.Object)
            .As<IEventListener<TestEvent>>()
            .SingleInstance();

        var sut = new AutofacEventSourcingContainer(builder.Build());

        var listeners = sut.GetEventListeners<TestEvent>();

        listeners.Should().HaveCount(1);
        listeners.Should().AllBeAssignableTo<IEventListener<TestEvent>>();
    }

    [TestMethod]
    public void Can_resolve_EventListeners_when_registered()
    {
        var listener = new Mock<IEventListener<TestEvent>>();
        var builder = new ContainerBuilder();

        var sut = new AutofacEventSourcingContainer(builder.Build());
        sut.RegisterListener(listener.Object);

        var listeners = sut.GetEventListeners<TestEvent>();

        listeners.Should().HaveCount(1);
        listeners.Should().AllBeAssignableTo<IEventListener<TestEvent>>();
    }
}