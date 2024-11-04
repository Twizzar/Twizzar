using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;

namespace Twizzar.Design.CoreInterfaces.Command.Events;

/// <summary>
/// Fired when a unit test &lt;-&gt; Code navigation done successfully.
/// </summary>
[ExcludeFromCodeCoverage]
public record UnitTestNavigatedEvent : IEvent<UnitTestNavigatedEvent>
{
    #region members

    /// <inheritdoc />
    public string ToLogString() => this.ToString();

    #endregion
}