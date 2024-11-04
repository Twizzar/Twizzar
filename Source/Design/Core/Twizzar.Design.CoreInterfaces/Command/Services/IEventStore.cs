using System.Collections.Generic;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Command.Services
{
    /// <summary>
    /// Stores all the passed events. And provides some query functions.
    /// </summary>
    public interface IEventStore : IService
    {
        /// <summary>
        /// Store a event message.
        /// </summary>
        /// <param name="e">The event message to store.</param>
        /// <returns>A task.</returns>
        Task Store(EventMessage e);

        /// <summary>
        /// Find the last store event of a specific event type and a specific id.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="id">The id.</param>
        /// <returns>If at least one exists Some event else None.</returns>
        Task<Maybe<TEvent>> FindLast<TEvent>(FixtureItemId id)
            where TEvent : IEvent;

        /// <summary>
        /// Find the last store event of a specific event type and a specific root item path.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="rootItemPath"></param>
        /// <returns>If at least one exists Some event else None.</returns>
        Task<Maybe<TEvent>> FindLast<TEvent>(string rootItemPath)
            where TEvent : IEvent;

        /// <summary>
        /// Find the last store event of a specific event type and a specific root item path.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <returns>If at least one exists Some event else None.</returns>
        Task<Maybe<TEvent>> FindLast<TEvent>()
            where TEvent : IEvent;

        /// <summary>
        /// Return all events of a specific id.
        /// </summary>
        /// <param name="id">The fixture id.</param>
        /// <returns>All events.</returns>
        Task<IEnumerable<IFixtureItemEvent>> FindAll(FixtureItemId id);

        /// <summary>
        /// Return all events for a root item.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="rootItemId">The root item id.</param>
        /// <returns>All events.</returns>
        Task<IEnumerable<TEvent>> FindAll<TEvent>(string rootItemId)
            where TEvent : IEvent;

        /// <summary>
        /// Returns all events that are not assigned any FixtureItem.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <returns>All general events.</returns>
        Task<IEnumerable<TEvent>> FindAll<TEvent>()
            where TEvent : IEvent;

        /// <summary>
        /// Clears the fixture items of a project from the event store.
        /// </summary>
        /// <param name="rootItemId">The root item id.</param>
        /// <returns>A task.</returns>
        Task ClearAll(string rootItemId);
    }
}
