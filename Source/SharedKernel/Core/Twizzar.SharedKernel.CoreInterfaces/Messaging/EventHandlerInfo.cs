using System;
using System.Threading;

namespace Twizzar.SharedKernel.CoreInterfaces.Messaging
{
    /// <summary>
    /// Type to store handler data.
    /// </summary>
    public class EventHandlerInfo
    {
        /// <summary>
        /// Gets or sets the event handler action.
        /// </summary>
        /// <value>The  event handler action.</value>
        public Delegate Action { get; set; }

        /// <summary>
        /// Gets or sets the subscriber of the event.
        /// </summary>
        /// <value>The subscriber.</value>
        public WeakReference Subscriber { get; set; }

        /// <summary>
        /// Gets or sets the type of the event.
        /// </summary>
        /// <value>The type.</value>
        public Type EventType { get; set; }

        /// <summary>
        /// Gets or sets the synchronization context, on which the event handler should get called.
        /// </summary>
        /// <value>The synchronization context.</value>
        public SynchronizationContext SynchronizationContext { get; set; }
    }
}