using System;
using System.Threading.Tasks;

namespace Twizzar.Design.CoreInterfaces.Common.Util
{
    /// <summary>
    /// Event hub for publishing events and register listeners.
    /// </summary>
    public interface IUiEventHub
    {
        /// <summary>
        /// Subscribes a listener.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="handler">The handler method of the subscriber.</param>
        void Subscribe<T>(object subscriber, Action<T> handler)
            where T : IUiEvent;

        /// <summary>
        /// Subscribes a listener.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="handler">The handler method of the subscriber.</param>
        void Subscribe<T>(object subscriber, Func<T, Task> handler)
            where T : IUiEvent;

        /// <summary>
        /// Unsubscribes the specified listener.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="handler">The handler method of the subscriber.</param>
        void Unsubscribe<T>(object subscriber, Action<T> handler)
            where T : IUiEvent;

        /// <summary>
        /// Publishes the specified UI event.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="uiEvent">The UI event.</param>
        void Publish<T>(T uiEvent)
            where T : IUiEvent;
    }
}