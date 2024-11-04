using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Core.Command.Services
{
    /// <summary>
    /// Abstract class for aggregates which publishes events.
    /// </summary>
    public abstract class EventPublisher : IService
    {
        private readonly IEventBus _eventBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisher"/> class.
        /// </summary>
        /// <param name="eventBus">The <see cref="IEventBus"/>.</param>
        protected EventPublisher(IEventBus eventBus)
        {
            this._eventBus = this.EnsureCtorParameterIsNotNull(eventBus, nameof(eventBus));
        }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        /// <summary>
        /// Publish an event.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="e">The event to publish.</param>
        /// <returns>A task.</returns>
        protected async Task PublishAsync<TEvent>(IEvent<TEvent> e)
            where TEvent : IEvent
        {
            this.EnsureParameter(e, nameof(e)).ThrowWhenNull();

            await this._eventBus.PublishAsync(e);
        }
    }
}
