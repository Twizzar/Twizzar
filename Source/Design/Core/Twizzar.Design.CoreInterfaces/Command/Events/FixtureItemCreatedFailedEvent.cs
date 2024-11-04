using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;

namespace Twizzar.Design.CoreInterfaces.Command.Events
{
    /// <summary>
    /// Event raised when the creation of a fixture item failed.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public record FixtureItemCreatedFailedEvent(FixtureItemId FixtureItemId, string Reason) :
        IEvent<FixtureItemCreatedFailedEvent>,
        IEventFailed,
        IFixtureItemEvent
    {
        #region members

        /// <inheritdoc />
        string IEvent.ToLogString() =>
            $"{nameof(FixtureItemCreatedFailedEvent)} {{ Reason = {this.Reason}, FixtureItemId = {this.FixtureItemId.GetHashCode()} }}";

        #endregion
    }
}