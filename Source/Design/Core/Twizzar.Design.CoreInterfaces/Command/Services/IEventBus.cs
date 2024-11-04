using System.Threading.Tasks;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.CoreInterfaces.Command.Services
{
    /// <summary>
    /// The event bus service stores the event in the <see cref="IEventStore"/>
    /// and delegates the event to the <see cref="IEventStoreToQueryCacheSynchronizer"/>
    /// and all <see cref="IEventListener{TEvent}"/> of the type TEvent.
    /// </summary>
    public interface IEventBus : IService
    {
        /// <summary>
        /// Publish the event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="e">The event.</param>
        /// <returns>A task.</returns>
        public Task PublishAsync<TEvent>(IEvent<TEvent> e)
            where TEvent : IEvent;
    }
}
