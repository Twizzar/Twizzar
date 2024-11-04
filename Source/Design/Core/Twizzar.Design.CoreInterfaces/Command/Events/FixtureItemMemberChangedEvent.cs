using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;

namespace Twizzar.Design.CoreInterfaces.Command.Events
{
    /// <summary>
    /// Event which occurs when a member configuration of a fixture item changes.
    /// </summary>
    /// <param name="FixtureItemId">The id.</param>
    /// <param name="MemberConfiguration">The new member configuration.</param>
    /// <param name="IsFromInitialization">A value indicating whether this event was fired
    /// while the event store was initialized and does not need to be written back to the config.</param>
    [ExcludeFromCodeCoverage]
    public record FixtureItemMemberChangedEvent(
            FixtureItemId FixtureItemId,
            IMemberConfiguration MemberConfiguration,
            bool IsFromInitialization = false)
        : IEvent<FixtureItemMemberChangedEvent>, IFixtureItemEvent
    {
        #region members

        /// <inheritdoc />
        string IEvent.ToLogString() =>
            $"{nameof(FixtureItemMemberChangedEvent)} {{ " +
            $"FixtureItemId = {this.FixtureItemId.GetHashCode()}, " +
            $"MemberConfiguration = {this.GetMemberConfigLogString()}, " +
            $"IsFromInitialization = {this.IsFromInitialization}}}";

        private string GetMemberConfigLogString() => $"typeof({this.MemberConfiguration.GetType().Name})";

        #endregion
    }
}