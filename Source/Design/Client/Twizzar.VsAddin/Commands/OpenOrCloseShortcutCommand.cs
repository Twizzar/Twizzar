using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualStudio.Shell;

using Twizzar.Design.CoreInterfaces.Common.Messaging.Events;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

using Task = System.Threading.Tasks.Task;

namespace Twizzar.VsAddin.Commands
{
    /// <summary>
    /// Command handler for open or close vs shortcut.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class OpenOrCloseShortcutCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 256;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new("24a33dbb-e573-47fe-8108-8752fc738cf4");

        private readonly IUiEventHub _eventHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenOrCloseShortcutCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file).
        /// </summary>
        /// <param name="commandService">Command service to add command to, not null.</param>
        /// <param name="eventHub"></param>
        private OpenOrCloseShortcutCommand(OleMenuCommandService commandService, IUiEventHub eventHub)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(commandService, nameof(commandService))
                .Parameter(eventHub, nameof(eventHub))
                .ThrowWhenNull();

            this._eventHub = eventHub;

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static OpenOrCloseShortcutCommand Instance { get; private set; }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="eventHub">The UI event hub.</param>
        /// <returns>A task.</returns>
        public static async Task InitializeAsync(AsyncPackage package, IUiEventHub eventHub)
        {
            if (Instance != null)
            {
                var exp = new InternalException($"{nameof(OpenOrCloseShortcutCommand)} is already Initialized");
                ViLog.Log(CallerContext.Create(), exp);
                throw exp;
            }

            // Switch to the main thread - the call to AddCommand in Command1's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new OpenOrCloseShortcutCommand(commandService, eventHub);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            this._eventHub.Publish<VsOpenOrCloseShortcutPressedEvent>(new VsOpenOrCloseShortcutPressedEvent());
        }
    }
}
