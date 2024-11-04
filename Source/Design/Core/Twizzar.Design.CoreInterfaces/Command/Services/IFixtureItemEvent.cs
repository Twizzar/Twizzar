using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;

namespace Twizzar.Design.CoreInterfaces.Command.Services
{
    /// <summary>
    /// An fixture item event is stored in the event store. And supports undo redo.
    /// </summary>
    /// <seealso cref="IEvent{TSelf}" />
    public interface IFixtureItemEvent : IEvent
    {
        /// <summary>
        /// Gets the fixture item id.
        /// </summary>
        public FixtureItemId FixtureItemId { get; }
    }
}