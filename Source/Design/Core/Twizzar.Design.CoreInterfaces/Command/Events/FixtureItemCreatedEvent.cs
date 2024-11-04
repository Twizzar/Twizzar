using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;

namespace Twizzar.Design.CoreInterfaces.Command.Events
{
    /// <summary>
    /// Event which occurs when a new Fixture Item is created.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public record FixtureItemCreatedEvent(FixtureItemId FixtureItemId) : IEvent<FixtureItemCreatedEvent>,
        IFixtureItemEvent
    {
        #region members

        /// <inheritdoc />
        string IEvent.ToLogString() =>
            $"{nameof(FixtureItemCreatedEvent)} {{ " +
            $"FixtureItemId = {this.FixtureItemId.GetHashCode()} }}";

        #endregion
    }
}