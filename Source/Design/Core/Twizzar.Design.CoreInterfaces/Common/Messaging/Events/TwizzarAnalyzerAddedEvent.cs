using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Common.Util;

namespace Twizzar.Design.CoreInterfaces.Common.Messaging.Events;

/// <summary>
/// Event fired when a twizzar analyzer is added.
/// </summary>
/// <param name="Version">The version of the analyzer.</param>
[ExcludeFromCodeCoverage]
public record TwizzarAnalyzerAddedEvent(System.Version Version) : IUiEvent
{
}