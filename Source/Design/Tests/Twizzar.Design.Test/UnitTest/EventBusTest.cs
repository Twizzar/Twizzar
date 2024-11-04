using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Design.Core.Command.Services;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using ViCommon.Functional.Monads.MaybeMonad;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.UnitTest;

[TestClass]
public class EventBusTest
{
    [TestMethod]
    public async Task EventBus_calls_listener_and_synchronizer()
    {
        // arrange
        var testEvent = new TestEvent() { FixtureItemId = RandomNamedFixtureItemId() };

        var eventStore = new Mock<IEventStore>();
        var container = new Mock<IEventSourcingContainer>();
        var eventListener = new Mock<IEventListener<ITestEvent>>();
        var eventSynchronizer = new Mock<IEventStoreToQueryCacheSynchronizer>();

        eventListener.Setup(listener => listener.IsListening)
            .Returns(true);

        container.Setup(
                sourcingContainer => 
                    sourcingContainer.GetEventListeners<ITestEvent>())
            .Returns(new[] { eventListener.Object });

        container.Setup(
                sourcingContainer => 
                    sourcingContainer.GetEventQuerySynchronizer())
            .Returns(Maybe.Some(eventSynchronizer.Object));

        var sut = new EventBus(eventStore.Object, container.Object);

        // act
        await sut.PublishAsync(testEvent);

        eventStore.Verify(eStore => eStore.Store(
                It.Is<EventMessage>(message => message.Event == testEvent)), 
            Times.Once());

        eventListener.Verify(listener => listener.Handle(testEvent), Times.Once);
        eventSynchronizer.Verify(
            synchronizer => 
                synchronizer.Synchronize(
                    It.Is<EventMessage>(message => message.Event == testEvent)),
            Times.Once);
    }

    [TestMethod]
    public async Task EventBus_calls_only_listeners_which_are_listening()
    {
        // arrange
        var testEvent = new TestEvent() { FixtureItemId = RandomNamedFixtureItemId()};

        var eventStore = new Mock<IEventStore>();
        var container = new Mock<IEventSourcingContainer>();
        var eventSynchronizer = new Mock<IEventStoreToQueryCacheSynchronizer>();

        var isListening = new [] {true, false, true};

        var listeners =
            isListening
                .Select(
                    b =>
                    {
                        var mock = new Mock<IEventListener<ITestEvent>>();
                        mock.Setup(listener => listener.IsListening)
                            .Returns(b);
                        return mock;
                    }).ToList();

        container.Setup(
                sourcingContainer =>
                    sourcingContainer.GetEventListeners<ITestEvent>())
            .Returns(listeners.Select(mock => mock.Object));

        container.Setup(
                sourcingContainer =>
                    sourcingContainer.GetEventQuerySynchronizer())
            .Returns(Maybe.Some(eventSynchronizer.Object));

        var sut = new EventBus(eventStore.Object, container.Object);

        // act
        await sut.PublishAsync(testEvent);

        eventStore.Verify(eStore => eStore.Store(
                It.Is<EventMessage>(message => message.Event == testEvent)),
            Times.Once);

        eventSynchronizer.Verify(
            synchronizer =>
                synchronizer.Synchronize(
                    It.Is<EventMessage>(message => message.Event == testEvent)),
            Times.Once);

        foreach (var listener in listeners)
        {
            var times = listener.Object.IsListening ? Times.Once() : Times.Never();
            listener.Verify(l => l.Handle(testEvent), times);
        }
    }
}