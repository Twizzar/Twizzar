using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Twizzar.Design.CoreInterfaces.TestCreation;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.VsAddin.Commands;

/// <summary>
/// Abstract class for context menu entries.
/// </summary>
[ExcludeFromCodeCoverage]
public abstract class ContextMenuCommand
{
    #region fields

    private readonly AsyncPackage _package;
    private readonly DTE _dte;
    private readonly ILocationService _locationService;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="ContextMenuCommand"/> class.
    /// </summary>
    /// <param name="package"></param>
    /// <param name="commandService"></param>
    /// <param name="dte"></param>
    /// <param name="locationService"></param>
    protected ContextMenuCommand(
        AsyncPackage package,
        OleMenuCommandService commandService,
        DTE dte,
        ILocationService locationService)
    {
        EnsureHelper.GetDefault.Many()
            .Parameter(commandService, nameof(commandService))
            .ThrowWhenNull();

        this._package = package;
        this._dte = dte;
        this._locationService = locationService;

        var menuCommandId = new CommandID(this.CommandSet, this.CommandId);
        var menuItem = new OleMenuCommand(this.Execute, menuCommandId);
        menuItem.BeforeQueryStatus += this.MenuItemOnBeforeQueryStatus;
        commandService.AddCommand(menuItem);
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets the command ID.
    /// </summary>
    protected abstract int CommandId { get; }

    /// <summary>
    /// Gets the command menu group (command set GUID).
    /// </summary>
    protected abstract Guid CommandSet { get; }

    #endregion

    #region members

    /// <summary>
    /// This function is the callback used to execute the command when the menu item is clicked.
    /// See the constructor to see how the menu item is associated with this function using
    /// OleMenuCommandService service and MenuCommand class.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event args.</param>
    protected abstract void Execute(object sender, EventArgs e);

    /// <summary>
    /// Get the current position of the caret and the active document.
    /// </summary>
    /// <returns></returns>
    protected Maybe<(string FileName, int CaretPosition)> GetCurrentPosition()
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        var document = this._dte.ActiveDocument;

        if (document?.Selection is not TextSelection)
        {
            return Maybe.None();
        }

        var textView = this.GetTextView();
        var caretPosition = textView.Caret.Position.BufferPosition;
        return (document.FullName, caretPosition.Position);
    }

    private void MenuItemOnBeforeQueryStatus(object sender, EventArgs e)
    {
        this.MenuItemOnBeforeQueryStatusAsync(sender, e).FireAndForget();
    }

    private async Task MenuItemOnBeforeQueryStatusAsync(object sender, EventArgs e)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        if (sender is not OleMenuCommand myCommand)
        {
            return;
        }

        await this.GetCurrentPosition()
            .IfSomeAsync(async tuple =>
            {
                myCommand.Enabled =
                    await this._locationService.CheckIfValidLocationAsync(tuple.FileName, tuple.CaretPosition);
            });
    }

    private IWpfTextView GetTextView()
    {
        this._package.GetService<SVsTextManager, IVsTextManager>().GetActiveView(1, null, out var textView);
        return this.GetEditorAdaptersFactoryService().GetWpfTextView(textView);
    }

    private IVsEditorAdaptersFactoryService GetEditorAdaptersFactoryService() =>
        this._package.GetService<SComponentModel, IComponentModel>()
            .GetService<IVsEditorAdaptersFactoryService>();

    #endregion
}