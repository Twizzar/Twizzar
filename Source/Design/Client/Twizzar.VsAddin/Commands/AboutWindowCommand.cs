using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualStudio.Shell;

using Twizzar.Design.Infrastructure.VisualStudio.Ui.View.About;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

using Task = System.Threading.Tasks.Task;

namespace Twizzar.VsAddin.Commands
{
    /// <summary>
    /// Command handler.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class AboutWindowCommand
    {
        #region static fields and constants

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new("aa52ddc3-4927-4f8d-b8d8-2909474f65bc");

        #endregion

        #region fields

        private readonly AboutWindow _aboutWindow;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutWindowCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file).
        /// </summary>
        /// <param name="commandService">Command service to add command to, not null.</param>
        /// <param name="aboutWindow"></param>
        private AboutWindowCommand(OleMenuCommandService commandService, AboutWindow aboutWindow)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(commandService, nameof(commandService))
                .Parameter(aboutWindow, nameof(aboutWindow))
                .ThrowWhenNull();

            this._aboutWindow = aboutWindow;

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AboutWindowCommand Instance { get; private set; }

        #endregion

        #region members

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="aboutWindow">The about window.</param>
        /// <returns>A task.</returns>
        public static async Task InitializeAsync(AsyncPackage package, AboutWindow aboutWindow)
        {
            if (Instance != null)
            {
                var exp = new InternalException($"{nameof(AboutWindowCommand)} is already Initialized");
                ViLog.Log(CallerContext.Create(), exp);
                throw exp;
            }

            // Switch to the main thread - the call to AddCommand in InfoCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new AboutWindowCommand(commandService, aboutWindow);
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
            ThreadHelper.ThrowIfNotOnUIThread();

            this._aboutWindow.Show();
        }

        #endregion
    }
}