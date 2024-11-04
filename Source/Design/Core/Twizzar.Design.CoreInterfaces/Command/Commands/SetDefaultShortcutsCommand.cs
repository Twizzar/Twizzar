using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;

namespace Twizzar.Design.CoreInterfaces.Command.Commands;

/// <summary>
/// Command to set the default key bindings for twizzar shortcuts.
/// </summary>
[ExcludeFromCodeCoverage]
public record SetDefaultShortcutsCommand() : ICommand<SetDefaultShortcutsCommand>;