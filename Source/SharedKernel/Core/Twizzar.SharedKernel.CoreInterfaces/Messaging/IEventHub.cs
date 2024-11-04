using System;

namespace Twizzar.SharedKernel.CoreInterfaces.Messaging
{
    /// <summary>
    /// Manage the publication and subscription of event.
    /// Represents the event aggregator pattern.
    /// </summary>
    public interface IEventHub
    {
        /// <summary>
        /// Publishes the specified event data to all subscribers.
        /// </summary>
        /// <typeparam name="T">Type of the event data. All subscribers of this type gets triggered.</typeparam>
        /// <param name="eventData">The event data to publish.</param>
        void Publish<T>(T eventData);

        /// <summary>
        /// Subscribe to a specified event type.
        /// </summary>
        /// <typeparam name="T">The event type, on which you want to register.</typeparam>
        /// <param name="subscriber">The subscriber, usually the calling instance ("this").</param>
        /// <param name="handler">Delegate to handle the event.</param>
        /// <param name="useSynchronizationContext">if set to <c>true</c> and if there is a current synchronization context, the handler gets called via the synchronization context of the subscribe time.</param>
        void Subscribe<T>(object subscriber, Action<T> handler, bool useSynchronizationContext = false);

        /// <summary>
        /// Unsubscribe to all events of the specified subscriber.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        void Unsubscribe(object subscriber);

        /// <summary>
        /// Unsubscribe the specified handler.
        /// </summary>
        /// <typeparam name="T">The event type, on which you want to unregister.</typeparam>
        /// <param name="subscriber">The subscriber, which is registered.</param>
        /// <param name="handler">The handler to unsubscribe.</param>
        void Unsubscribe<T>(object subscriber, Action<T> handler = null);

        /// <summary>
        /// Checks if there are handlers register for the specified subscriber.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <returns><c>true</c> if there are handlers registered, <c>false</c> otherwise.</returns>
        bool Exists(object subscriber);

        /// <summary>
        /// Checks if there are handlers register for the specified event type and subscriber.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <returns><c>true</c> if there are handlers registered, <c>false</c> otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "T is the type of the event")]
        bool Exists<T>(object subscriber);

        /// <summary>
        /// Checks if there are handlers register for the specified event type, subscriber and handler delegate.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="handler">The handler.</param>
        /// <returns><c>true</c> if there are handlers registered, <c>false</c> otherwise.</returns>
        bool Exists<T>(object subscriber, Action<T> handler);
    }
}