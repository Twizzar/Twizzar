using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.CoreInterfaces.Command.Services
{
    /// <summary>
    /// Service for register <see cref="IEventListener{TEvent}"/> which get called by the <see cref="IEventBus"/>.
    /// </summary>
    /// <seealso cref="IService" />
    public interface IEventSourcingRegisterService : IService
    {
        /// <summary>
        /// Registers a new listener.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="listener">The listener.</param>
        public void RegisterListener<TEvent>(IEventListener<TEvent> listener)
            where TEvent : IEvent;
    }
}
