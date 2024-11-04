using System.Diagnostics.CodeAnalysis;

using Twizzar.Design.CoreInterfaces.Command.Services;

namespace Twizzar.Design.CoreInterfaces.Command.Events;

/// <summary>
/// Fired when the unit test navigation failed.
/// </summary>
/// <param name="Reason"></param>
[ExcludeFromCodeCoverage]
public record UnitTestNavigationFailedEvent(string Reason) : IEvent<UnitTestNavigationFailedEvent>, IEventFailed
{
    #region members

    /// <inheritdoc />
    public string ToLogString() => this.ToString();

    #endregion
}