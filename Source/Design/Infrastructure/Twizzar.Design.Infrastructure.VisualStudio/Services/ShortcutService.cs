using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio.Services;

/// <summary>
/// Service for managing shortcuts.
/// </summary>
public class ShortcutService : IShortcutService
{
    private readonly DTE _dte;

    private readonly Dictionary<string, string> _commandShortcuts = new()
    {
        { "TWIZZAR.About", "Global::Ctrl+Alt+N, Ctrl+Alt+A" },
        { "TWIZZAR.OpenOrClose", "Global::Ctrl+Alt+N, Ctrl+Alt+V" },
        { "TWIZZAR.CreateUnitTest", "Global::Ctrl+Alt+N, Ctrl+Alt+N" },
        { "TWIZZAR.UnitTestCodeNavigation", "Global::Ctrl+Alt+N, Ctrl+Alt+G" },
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="ShortcutService"/> class.
    /// </summary>
    /// <param name="dte">VS automation object model.</param>
    public ShortcutService(DTE dte)
    {
        EnsureHelper.GetDefault
            .Many()
            .Parameter(dte, nameof(dte))
            .ThrowWhenNull();

        this._dte = dte;
    }

    #region Implementation of IShortcutService

    /// <inheritdoc />
    public async Task SetDefaultKeyBindingsAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var availableTasks = this._dte.Commands;

        foreach (Command command in availableTasks)
        {
            if (command is null)
            {
                continue;
            }

            if (this.TrySetKeyBinding(command))
            {
                continue;
            }

            this.ResetKeyBinding(command);
        }
    }

    private bool TrySetKeyBinding(Command command)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        if (!this._commandShortcuts.ContainsKey(command.Name))
        {
            return false;
        }

        command.Bindings = new object[] { this._commandShortcuts[command.Name] };
        return true;
    }

    private void ResetKeyBinding(Command command)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        if (command.Bindings is not Array currentBindings)
        {
            return;
        }

        var newBindings = new List<object>();
        var hasChanges = false;

        foreach (var binding in currentBindings)
        {
            if (this._commandShortcuts.ContainsValue(binding.ToString()))
            {
                hasChanges = true;
                continue;
            }

            newBindings.Add(binding);
        }

        if (hasChanges)
        {
            command.Bindings = newBindings.ToArray();
        }
    }

    #endregion
}