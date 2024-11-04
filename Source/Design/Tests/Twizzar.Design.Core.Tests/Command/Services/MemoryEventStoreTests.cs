using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Design.Core.Command.Services;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using ViCommon.Functional.Monads.MaybeMonad;
using Assert = NUnit.Framework.Assert;

namespace Twizzar.Design.Core.Tests.Command.Services
{
    [TestFixture]
    public class MemoryEventStoreTests
    {
        private Mock<IEventStreamCollection> _streamCollection;
        private IEventStore _eventStore;

        [SetUp]
        public void Setup()
        {
            this._streamCollection = new Mock<IEventStreamCollection>();
            this._eventStore = new MemoryEventStore(this._streamCollection.Object);
        }

        [Test]
        public void Add_EventMessage_should_pass_to_streamCollection()
        {
            // arrange
            var @event = new TestEvent
            {
                FixtureItemId = FixtureItemId.CreateNameless(
                    TypeFullName.Create("FullName")),
            };
            var message = EventMessage.Create(@event, @event.GetType());
            
            // act
            this._eventStore.Store(message).Wait();

            // assert
            this._streamCollection.Verify(s => s.Add(message), Times.Once);
        }

        [Test]
        public void ClearAll_should_pass_to_stream_collection()
        {
            const string rootItemId = "rootItemId";

            this._eventStore.ClearAll(rootItemId).Wait();

            this._streamCollection.Verify(s => s.ClearStream(rootItemId), Times.Once);
        }

        [Test]
        public void FindLast_should_return_las_event_of_type_from_stream()
        {
            // arrange
            const string rootItemId = "rootItemId";
            var fixtureItemId = FixtureItemId.CreateNameless(TypeFullName.Create(""))
                .WithRootItemPath(rootItemId);

            var @event = new FixtureItemEvent() { FixtureItemId = fixtureItemId };
            var eventStream = new Mock<IEventStream>();
            var maybeEventStream = Maybe.Some(eventStream.Object);

            eventStream.Setup(s => s.FindAll<IFixtureItemEvent>()).Returns(new [] { @event });

            this._streamCollection.Setup(s => s.GetStream(rootItemId)).Returns(maybeEventStream);

            // act
            var result = this._eventStore.FindLast<FixtureItemEvent>(fixtureItemId).Result;

            // assert
            Assert.IsTrue(result.IsSome);
            result.Do(
                e => Assert.AreEqual(e, @event),
                Assert.Fail);
        }

        [Test]
        public void FindAll_for_FixtureItemId_should_throw_InternalException_when_RootItemId_is_none()
        {
            var fixtureItemId = FixtureItemId.CreateNameless(TypeFullName.Create(""));
            
            Assert.ThrowsAsync<InternalException>(() => this._eventStore.FindAll(fixtureItemId));
        }

        [Test]
        public void FindAll_for_FixtureItemId_should_return_pass_to_stream_and_return_new_empty()
        {
            // arrange
            const string rootItemId = "rootItemId";
            var fixtureItemId = FixtureItemId.CreateNameless(TypeFullName.Create(""))
                .WithRootItemPath(rootItemId);

            // act
            this._eventStore.FindAll(fixtureItemId);

            // assert
            this._streamCollection.Verify(s => s.GetStream(rootItemId), Times.Once);
        }

        [Test]
        public void FindAll_for_FixtureItemId_should_return_pass_to_stream_and_return_result_from_stream()
        {
            // arrange
            const string rootItemId = "rootItemId";
            var fixtureItemId = FixtureItemId.CreateNameless(TypeFullName.Create(""))
                .WithRootItemPath(rootItemId);

            var stream = new Mock<IEventStream>();
            var maybeStream = Maybe.Some(stream.Object);
            stream.Setup(s => s.FindAll<IFixtureItemEvent>())
                .Returns(Mock.Of<IEnumerable<IFixtureItemEvent>>());

            this._streamCollection.Setup(s => s.GetStream(rootItemId))
                .Returns(maybeStream);

            // act
            this._eventStore.FindAll(fixtureItemId);

            // assert
            this._streamCollection.Verify(s => s.GetStream(rootItemId), Times.Once);
        }

        [Test]
        public async Task FindLast_FakeEvent_returns_the_last_sent_event_of_this_type()
        {
            // arrange
            var expectedEvent = new FakeEvent();
            var eventMessage = EventMessage.Create(expectedEvent, expectedEvent.GetType());
            var stream = new Mock<IEventStream>();
            stream.Setup(s => s.Last<FakeEvent>()).Returns(eventMessage);
            var maybeStream = Maybe.Some(stream.Object);
            this._streamCollection.Setup(s => s.GetStream())
                .Returns(maybeStream);

            // act
            var result = await this._eventStore.FindLast<FakeEvent>();

            // assert
            result.IsSome.Should().BeTrue();
            result.Do(
                s => s.Should().Be(expectedEvent),
                Assert.Fail);
        }

        [Test]
        public async Task FindAll_FakeEvent_returns_all_sent_event_of_this_type()
        {
            // arrange
            var expectedEvents = new []
            {
                new FakeEvent(),new FakeEvent(), new FakeEvent()
            };
            var stream = new Mock<IEventStream>();
            stream.Setup(s => s.FindAll<FakeEvent>()).Returns(expectedEvents);
            var maybeStream = Maybe.Some(stream.Object);
            this._streamCollection.Setup(s => s.GetStream())
                .Returns(maybeStream);

            // act
            var result = (await this._eventStore.FindAll<FakeEvent>()).ToList();

            // assert
            foreach (var expectedEvent in expectedEvents)
                result.Should().Contain(expectedEvent);
        }
    }
}
