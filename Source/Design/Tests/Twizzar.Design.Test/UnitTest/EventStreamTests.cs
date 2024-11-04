using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Core.Command.Services;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Test.UnitTest;

[TestClass]
public class EventStreamTests
{
    private IEventStream _eventStream;

    [TestInitialize]
    public void Setup()
    {
        this._eventStream = new EventStream();
    }

    [TestMethod]
    public void Add_EventMessage_should_store_and_contains_should_be_true()
    {
        // arrange
        var @event = new SomeEvent();
        var eventMessage = EventMessage.Create<SomeEvent>(@event);

        // act
        this._eventStream.Add(eventMessage);

        // assert
        this._eventStream.Contains(eventMessage).Should().BeTrue();
    }

    [TestMethod]
    public void Contains_EventMessage_should_be_false_when_stream_is_empty()
    {
        // arrange
        var @event = new SomeEvent();
        var eventMessage = EventMessage.Create<SomeEvent>(@event);

        // assert
        this._eventStream.Contains(eventMessage).Should().BeFalse();
    }

    [TestMethod]
    public void Last_should_return_newest_event_from_stream()
    {
        // arrange
        var eventMessages = new List<EventMessage>();

        for (var i = 0; i < 10; i++)
        {
            var @event = new SomeEvent();
            var eventMessage = EventMessage.Create<SomeEvent>(@event);
            eventMessages.Add(eventMessage);
        }

        var last = Maybe.Some(eventMessages.Last());

        eventMessages
            .ToList()
            .ForEach(m => this._eventStream.Add(m));

        // act
        var lastFromStream = this._eventStream.Last<SomeEvent>();

        // assert
        lastFromStream.Should().BeEquivalentTo(last);
    }

    [TestMethod]
    public void Clear_should_remove_all_events_from_stream()
    {
        // arrange
        var eventMessages = new List<EventMessage>();

        for (var i = 0; i < 10; i++)
        {
            var @event = new SomeEvent();
            var eventMessage = EventMessage.Create<SomeEvent>(@event);
            eventMessages.Add(eventMessage);
            this._eventStream.Add(eventMessage);
        }

        // act
        this._eventStream.Clear();

        // assert
        this._eventStream.FindAll<SomeEvent>().Should().BeEmpty();
    }

    [TestMethod]
    public void FindAll_should_return_events_of_type_in_ascending_order()
    {
        // arrange
        var eventMessages = new List<EventMessage>();

        for (var i = 0; i < 10; i++)
        {
            var @event = new SomeEvent();
            var eventMessage = EventMessage.Create<SomeEvent>(@event);
            eventMessages.Add(eventMessage);
        }

        eventMessages
            .OrderByDescending(m => m.TimeStamp)
            .ToList()
            .ForEach(m => this._eventStream.Add(m));

        // act
        var events = this._eventStream.FindAll<SomeEvent>().ToList();

        // assert
        for (var i = 0; i < events.Count; i++)
        {
            eventMessages[i].Event.Should().BeSameAs(events[i]);
        }
    }
}

public class SomeEvent : IEvent
{

    /// <inheritdoc />
    string IEvent.ToLogString() => this.ToString();
}