using System.Collections.Generic;
using System.Linq;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Core.Command.Services
{
    /// <inheritdoc cref="IEventStream"/>
    public class EventStream : IEventStream
    {
        #region fields

        private readonly object _lockObject = new();

        private readonly SortedDictionary<EventStreamId, EventMessage> _internalStream =
            new(EventStreamId.OrderComparer);

        private uint _eventCount;

        #endregion

        #region members

        /// <inheritdoc />
        public void Add(EventMessage eventMessage)
        {
            lock (this._lockObject)
            {
                this._internalStream.Add(
                    new EventStreamId(eventMessage.TimeStamp, this._eventCount++),
                    eventMessage);
            }
        }

        /// <inheritdoc />
        public bool Contains(EventMessage eventMessage)
        {
            lock (this._lockObject)
            {
                return this._internalStream
                    .ContainsValue(eventMessage);
            }
        }

        /// <inheritdoc />
        public Maybe<EventMessage> Last<TEvent>()
            where TEvent : IEvent
        {
            lock (this._lockObject)
            {
                return this._internalStream.Values
                    .LastOrNone(e => e.Event is TEvent);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            lock (this._lockObject)
            {
                this._internalStream.Clear();
            }
        }

        /// <inheritdoc />
        public IEnumerable<TEvent> FindAll<TEvent>()
            where TEvent : IEvent
        {
            lock (this._lockObject)
            {
                return
                    from message in this._internalStream.Values
                    where message.Event is TEvent
                    select (TEvent)message.Event;
            }
        }

        #endregion
    }
}