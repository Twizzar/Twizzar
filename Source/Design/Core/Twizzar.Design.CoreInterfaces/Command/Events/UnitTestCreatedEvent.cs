using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;

namespace Twizzar.Design.CoreInterfaces.Command.Events;

/// <summary>
/// Fired when a unit test was created successful.
/// </summary>
[ExcludeFromCodeCoverage]
public record UnitTestCreatedEvent : IEvent<UnitTestCreatedEvent>
{
    #region members

    /// <inheritdoc />
    public string ToLogString() => this.ToString();

    #endregion
}