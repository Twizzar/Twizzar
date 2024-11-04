using Twizzar.Design.CoreInterfaces.Command.Events;

namespace Twizzar.Design.CoreInterfaces.Command.Services
{
    /// <summary>
    /// This service will receive the events before all <see cref="IEventListener{TEvent}"/>.
    /// The <see cref="IEventBus"/> waits till the <see cref="Synchronize"/> is finished.
    /// This ensures the <see cref="IEventStoreToQueryCacheSynchronizer"/> is up to date when
    /// a <see cref="IEventListener{TEvent}"/> is executing a query.
    /// </summary>
    public interface IEventStoreToQueryCacheSynchronizer
    {
        /// <summary>
        /// Synchronise the public structure to match the new event.
        /// </summary>
        /// <param name="message">The event message.</param>
        void Synchronize(EventMessage message);
    }
}
