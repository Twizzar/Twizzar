using System.Threading.Tasks;

namespace Twizzar.Design.CoreInterfaces.Common.VisualStudio;

/// <summary>
/// Service for managing shortcuts.
/// </summary>
public interface IShortcutService
{
    /// <summary>
    /// Sets the default key bindings for twizzar commands.
    /// </summary>
    /// <returns></returns>
    Task SetDefaultKeyBindingsAsync();
}