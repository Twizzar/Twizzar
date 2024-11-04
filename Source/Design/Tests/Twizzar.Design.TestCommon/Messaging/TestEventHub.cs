using System;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.SharedKernel.Core.Messaging;

namespace Twizzar.Design.TestCommon.Messaging
{
    public class TestEventHub : EventHub, IUiEventHub
    {
        /// <inheritdoc />
        public void Subscribe<T>(object subscriber, Action<T> handler) where T : IUiEvent
        {
            this.Subscribe(subscriber, handler, true);
        }

        /// <inheritdoc />
        public void Subscribe<T>(object subscriber, Func<T, Task> handler) where T : IUiEvent
        {
            this.Subscribe(subscriber, new Action<T>(e => handler(e)), true);
        }

        /// <inheritdoc />
        public new void Unsubscribe<T>(object subscriber, Action<T> handler) where T : IUiEvent
        {
            base.Unsubscribe(subscriber, handler);
        }

        /// <inheritdoc />
        public new void Publish<T>(T uiEvent) where T : IUiEvent
        {
            base.Publish(uiEvent);
        }
    }
}