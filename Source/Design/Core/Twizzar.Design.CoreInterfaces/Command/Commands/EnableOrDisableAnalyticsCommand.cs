using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;

namespace Twizzar.Design.CoreInterfaces.Command.Commands;

/// <summary>
/// Command to enable or disable analytics.
/// </summary>
/// <param name="Enabled"></param>
[ExcludeFromCodeCoverage]
public record EnableOrDisableAnalyticsCommand(bool Enabled) : ICommand<EnableOrDisableAnalyticsCommand>;