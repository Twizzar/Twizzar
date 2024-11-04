using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;

namespace Twizzar.Design.CoreInterfaces.Command.Events;

/// <summary>
/// Event when the analytics are disabled or enabled.
/// </summary>
/// <param name="Enabled"></param>
[ExcludeFromCodeCoverage]
public record AnalyticsEnabledOrDisabledEvent(bool Enabled) : IEvent<AnalyticsEnabledOrDisabledEvent>
{
    #region members

    /// <inheritdoc />
    string IEvent.ToLogString() => this.ToString();

    #endregion
}