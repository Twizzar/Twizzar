using System.Threading;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Command.Services
{
    /// <summary>
    /// Listens to a specific type of event.
    /// </summary>
    /// <typeparam name="TEvent">The event type to listen.</typeparam>
    public interface IEventListener<in TEvent>
        where TEvent : IEvent
    {
        /// <summary>
        /// Gets a value indicating whether the listener is listening.
        /// The <see cref="IEventBus"/> will not call <see cref="Handle"/> when this is false.
        /// </summary>
        public bool IsListening { get; }

        /// <summary>
        /// Gets the synchronization context.
        /// When Some the <see cref="IEventBus"/> will invoke the <see cref="Handle"/>
        /// method in the synchronization context.
        /// When None <see cref="Handle"/> will be called on the <see cref="IEventBus"/> thread.
        /// </summary>
        public Maybe<SynchronizationContext> SynchronizationContext { get; }

        /// <summary>
        /// Handle the event.
        /// </summary>
        /// <param name="e">The event.</param>
        public void Handle(TEvent e);
    }
}
