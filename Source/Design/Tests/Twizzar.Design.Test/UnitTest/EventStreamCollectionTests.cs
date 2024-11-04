using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Core.Command.Services;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Test.UnitTest;

[TestClass]
public class EventStreamCollectionTests
{
    private IEventStreamCollection _eventStreamCollection;

    [TestInitialize]
    public void Setup()
    {
        this._eventStreamCollection = new EventStreamCollection();
    }

    [TestMethod]
    public void Add_general_message_should_add_message_to_general_stream()
    {
        var generalEvent = new GeneralEvent();
        var eventMessage = EventMessage.Create(generalEvent, generalEvent.GetType());
            
        this._eventStreamCollection.Add(eventMessage);

        this._eventStreamCollection
            .GetStream()
            .AsMaybeValue()
            .Should()
            .BeAssignableTo<SomeValue<IEventStream>>()
            .Subject
            .Value
            .Contains(eventMessage)
            .Should()
            .BeTrue();
    }

    [TestMethod]
    public void Add_fixtureItem_message_should_throw_InternalException_when_RootItemId_is_not_set()
    {
        var fixtureItemId = FixtureItemId.CreateNamed("", TypeFullName.Create(""));
        var fixtureEvent = new FixtureItemEvent {FixtureItemId = fixtureItemId};
        var eventMessage = EventMessage.Create(fixtureEvent, fixtureEvent.GetType());

        Assert.ThrowsException<InternalException>(() => this._eventStreamCollection.Add(eventMessage));
    }

    [TestMethod]
    public void Add_fixtureItem_message_should_store_message()
    {
        const string rootItemId = "rootItemId";
        var fixtureItemId = FixtureItemId.CreateNamed("", TypeFullName.Create(""))
            .WithRootItemPath(rootItemId);

        var fixtureEvent = new FixtureItemEvent {FixtureItemId = fixtureItemId};
        var eventMessage = EventMessage.Create(fixtureEvent, fixtureEvent.GetType());

        this._eventStreamCollection.Add(eventMessage);

        this._eventStreamCollection
            .GetStream(rootItemId)
            .AsMaybeValue()
            .Should()
            .BeAssignableTo<SomeValue<IEventStream>>()
            .Subject
            .Value
            .Contains(eventMessage)
            .Should()
            .BeTrue();
    }

    [TestMethod]
    public void Clear_should_remove_stream()
    {
        const string rootItemId = "rootItemId";
        var fixtureItemId = FixtureItemId.CreateNamed("", TypeFullName.Create(""))
            .WithRootItemPath(rootItemId);

        var fixtureEvent = new FixtureItemEvent {FixtureItemId = fixtureItemId};
        var eventMessage = EventMessage.Create(fixtureEvent, fixtureEvent.GetType());
        this._eventStreamCollection.Add(eventMessage);

        this._eventStreamCollection.ClearStream(rootItemId);

        this._eventStreamCollection.GetStream(rootItemId).IsNone.Should().BeTrue();
    }


    private class GeneralEvent : IEvent<GeneralEvent>
    {
        #region Implementation of IEvent

        /// <inheritdoc />
        string IEvent.ToLogString() => this.ToString();

        #endregion
    }

    private class FixtureItemEvent : IEvent<FixtureItemEvent>, IFixtureItemEvent
    {

        /// <inheritdoc />
        public FixtureItemId FixtureItemId { get; set; }


        /// <inheritdoc />
        string IEvent.ToLogString() => this.ToString();
    }
}