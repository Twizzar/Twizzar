using System.Collections.Generic;
using Twizzar.SharedKernel.CoreInterfaces;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Command.Services
{
    /// <summary>
    /// Container which holds: <br/>
    /// - all the <see cref="ICommandHandler{TCommand}"/> <br/>
    /// - all the <see cref="IEventListener{TEvent}"/> <br/>
    /// - The <see cref="IEventStoreToQueryCacheSynchronizer"/>.<br/>
    /// </summary>
    public interface IEventSourcingContainer : IService
    {
        /// <summary>
        /// Get the <see cref="ICommandHandler{TCommand}"/> for a specific command type.
        /// </summary>
        /// <typeparam name="TCommand">The command type.</typeparam>
        /// <returns>Some command handler if registered else None.</returns>
        public Maybe<ICommandHandler<TCommand>> GetCommandHandler<TCommand>()
            where TCommand : ICommand;

        /// <summary>
        /// Gets all the <see cref="IEventListener{TEvent}"/> for a specific event type.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <returns>A sequence of event listeners.</returns>
        public IEnumerable<IEventListener<TEvent>> GetEventListeners<TEvent>()
            where TEvent : IEvent;

        /// <summary>
        /// Return the registered <see cref="IEventStoreToQueryCacheSynchronizer"/>.
        /// </summary>
        /// <returns>Some event query synchronizer if registered else None.</returns>
        public Maybe<IEventStoreToQueryCacheSynchronizer> GetEventQuerySynchronizer();
    }
}
