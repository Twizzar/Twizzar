using Twizzar.Design.CoreInterfaces.Command.Services;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Core.Command.FixtureItem.Definition
{
    /// <summary>
    /// Failure for a failed event.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    public class FailedEventFailure<T> : Failure
        where T : IEvent<IEvent>, IEventFailed
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedEventFailure{T}"/> class.
        /// </summary>
        /// <param name="event">The event.</param>
        public FailedEventFailure(T @event)
            : base(@event.Reason)
        {
            this.Event = @event;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the event.
        /// </summary>
        public T Event { get; }

        #endregion
    }
}