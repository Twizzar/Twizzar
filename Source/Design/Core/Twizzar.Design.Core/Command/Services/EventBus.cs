using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces.Util;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Core.Command.Services
{
    /// <summary>
    /// Implementation of <see cref="IEventBus"/>.
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly IEventStore _eventStore;
        private readonly IEventSourcingContainer _container;
        private readonly SemaphoreSlim _semaphore = new(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBus"/> class.
        /// </summary>
        /// <param name="eventStore">The event store service.</param>
        /// <param name="container">The container service.</param>
        public EventBus(IEventStore eventStore, IEventSourcingContainer container)
        {
            this._eventStore = this.EnsureCtorParameterIsNotNull(eventStore, nameof(eventStore));
            this._container = this.EnsureCtorParameterIsNotNull(container, nameof(container));
        }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public async Task PublishAsync<TEvent>(IEvent<TEvent> e)
            where TEvent : IEvent
        {
            this.EnsureParameter(e, nameof(e)).ThrowWhenNull();

            try
            {
                await this._semaphore.WaitAsync();

                ViMonitor.TrackEvent($"{typeof(TEvent).Name} triggered", new Dictionary<string, string>() { { "event", e.ToLogString() } });
                var message = EventMessage.Create(e, typeof(TEvent));
                await this._eventStore.Store(message);

                if (e is IFixtureItemEvent)
                {
                    this._container.GetEventQuerySynchronizer()
                        .IfSome(synchronizer => synchronizer.Synchronize(message));
                }

                foreach (var listener in this._container.GetEventListeners<TEvent>()
                    .Where(listener => listener.IsListening))
                {
                    try
                    {
                        var synchronizationContext = listener.SynchronizationContext;
                        switch (synchronizationContext.AsMaybeValue())
                        {
                            case SomeValue<SynchronizationContext> c:
                                c.Value.Post(state => listener.Handle((TEvent)e), null);
                                break;
                            default:
                                listener.Handle((TEvent)e);
                                break;
                        }
                    }
                    catch (Exception exp)
                    {
                        this.Logger?.Log(exp);
                    }
                }
            }
            finally
            {
                this._semaphore.Release();
            }
        }
        #endregion
    }
}