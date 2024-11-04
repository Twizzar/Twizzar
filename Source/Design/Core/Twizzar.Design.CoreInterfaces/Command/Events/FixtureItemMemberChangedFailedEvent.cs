using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Command.Events
{
    /// <summary>
    /// Event raised when the member change failed.
    /// </summary>
    /// <param name="Reason">A text explaining the reason.</param>
    /// <param name="FixtureItemId">The fixture id.</param>
    /// <param name="MemberConfiguration">The member configuration.</param>
    /// <param name="RequestedMemberConfiguration">
    /// Gets the requested member configuration by command to change member configuration this is the same as <see cref="MemberConfiguration"/>,
    /// except the <see cref="MemberConfiguration"/> is a parameter description. This will be a method member config.
    /// The requested member configuration.</param>
    [ExcludeFromCodeCoverage]
    public record FixtureItemMemberChangedFailedEvent(
        FixtureItemId FixtureItemId,
        IMemberConfiguration MemberConfiguration,
        string Reason,
        Maybe<IMemberConfiguration> RequestedMemberConfiguration = default) :
        IEvent<FixtureItemMemberChangedFailedEvent>,
        IFixtureItemEvent,
        IEventFailed
    {
        #region members

        /// <inheritdoc />
        string IEvent.ToLogString() =>
            $"{nameof(FixtureItemMemberChangedFailedEvent)} {{ " +
            $"Reason = {this.Reason}, " +
            $"FixtureItemId = {this.FixtureItemId.GetHashCode()}, " +
            $"MemberConfiguration = {this.MemberConfiguration.GetType().Name}, " +
            $"RequestedMemberConfiguration = {this.RequestedMemberConfiguration.GetType().Name} }}";

        #endregion
    }
}