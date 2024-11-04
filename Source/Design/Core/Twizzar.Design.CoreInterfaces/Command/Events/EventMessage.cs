using System;
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.CoreInterfaces.Command.Events
{
    /// <summary>
    /// Event with metadata.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class EventMessage : Entity<EventMessage, Guid>
    {
        #region ctors

        private EventMessage(Guid id, DateTime timeStamp, IEvent @event, Type eventType)
            : base(id)
        {
            this.Id = id;
            this.TimeStamp = timeStamp;
            this.Event = @event;
            this.EventType = eventType;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the id of the message.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the timestamp when the message was created.
        /// </summary>
        public DateTime TimeStamp { get; }

        /// <summary>
        /// Gets the event.
        /// </summary>
        public IEvent Event { get; }

        /// <summary>
        /// Gets the event type.
        /// </summary>
        public Type EventType { get; }

        #endregion

        #region members

        /// <summary>
        /// Create a new event message.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="type">The event type. Needs to be the same type which the <see cref="IEventListener{TEvent}"/> uses.</param>
        /// <returns>A new <see cref="EventMessage"/>.</returns>
        public static EventMessage Create(
            IEvent @event,
            Type type) =>
            new(Guid.NewGuid(), DateTime.UtcNow, @event, type);

        /// <summary>
        /// Create a new event message.
        /// </summary>
        /// <typeparam name="TEvent">The event type. Needs to be the same type which the <see cref="IEventListener{TEvent}"/> uses.</typeparam>
        /// <param name="event">The event.</param>
        /// <returns>A new <see cref="EventMessage"/>.</returns>
        public static EventMessage Create<TEvent>(IEvent @event) =>
            new(Guid.NewGuid(), DateTime.UtcNow, @event, typeof(TEvent));

        /// <inheritdoc />
        protected override bool Equals(Guid a, Guid b) =>
            a.Equals(b);

        #endregion
    }
}