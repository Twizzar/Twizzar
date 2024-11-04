using System;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.SharedKernel.Core.Messaging;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.NLog.Interfaces;

namespace Twizzar.Design.Ui.Messaging
{
    /// <inheritdoc cref="IUiEventHub" />
    public class UiEventHub : EventHub, IUiEventHub, IHasLogger
    {
        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public void Subscribe<T>(object subscriber, Action<T> handler)
            where T : IUiEvent
        {
            this.Subscribe(subscriber, handler, true);
        }

        /// <inheritdoc />
        public void Subscribe<T>(object subscriber, Func<T, Task> handler)
            where T : IUiEvent
        {
            this.Subscribe(subscriber, new Action<T>(e => handler(e).Forget()), true);
        }

        /// <inheritdoc />
        public new void Unsubscribe<T>(object subscriber, Action<T> handler)
            where T : IUiEvent
        {
            base.Unsubscribe(subscriber, handler);
        }

        /// <inheritdoc />
        public new void Publish<T>(T uiEvent)
            where T : IUiEvent
        {
            try
            {
                base.Publish(uiEvent);
            }
            catch (Exception e)
            {
                this.Logger?.Log(e);
            }
        }

        #endregion
    }
}