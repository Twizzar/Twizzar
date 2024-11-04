using System.Collections.Generic;
using Twizzar.Design.CoreInterfaces.Command.Events;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Command.Services
{
    /// <summary>
    /// Stream of event messages in chronological order.
    /// </summary>
    public interface IEventStream
    {
        /// <summary>
        /// Adds the given message to the stream.
        /// </summary>
        /// <param name="eventMessage">Event message to be added.</param>
        void Add(EventMessage eventMessage);

        /// <summary>
        /// Checks if the stream contains the given event message.
        /// </summary>
        /// <param name="eventMessage">The event message to check for.</param>
        /// <returns>true if event message is in the stream otherwise false.</returns>
        bool Contains(EventMessage eventMessage);

        /// <summary>
        /// Gets the last event message.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <returns>None if no entry exists for TEvent otherwise the chronological last item.</returns>
        Maybe<EventMessage> Last<TEvent>()
            where TEvent : IEvent;

        /// <summary>
        /// Clears the stream and removes all stored event messages.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets all events available in the stream.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <returns>Events in chronological order.</returns>
        IEnumerable<TEvent> FindAll<TEvent>()
            where TEvent : IEvent;
    }
}