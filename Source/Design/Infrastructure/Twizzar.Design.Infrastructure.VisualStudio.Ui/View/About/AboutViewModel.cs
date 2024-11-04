using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Media;

using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Threading;

using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Ui.Interfaces.ViewModels;

using Twizzar.SharedKernel.NLog.Logging;

using ICommand = System.Windows.Input.ICommand;

#pragma warning disable VSTHRD012 // Provide JoinableTaskFactory where allowed

namespace Twizzar.Design.Infrastructure.VisualStudio.Ui.View.About
{
    /// <inheritdoc cref="IAboutViewModel"/>
    [ExcludeFromCodeCoverage]
    public class AboutViewModel : ViewModelBase, IAboutViewModel
    {
        #region static fields and constants

        private static readonly Message NoneMessage = new(string.Empty, Brushes.Transparent);

        #endregion

        #region fields

        private readonly ICommandBus _commandBus;
        private bool _enableAnalytics;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutViewModel"/> class.
        /// </summary>
        /// <param name="addinVersionQuery">The addin version query.</param>
        /// <param name="commandBus"></param>
        /// <param name="settingsQuery"></param>
        public AboutViewModel(
            IAddinVersionQuery addinVersionQuery,
            ICommandBus commandBus,
            ISettingsQuery settingsQuery)
        {
            this.AddinVersion = addinVersionQuery.GetVsAddinVersion();
            this.DllVersion = addinVersionQuery.GetDllVersion();
            this.ProductVersion = addinVersionQuery.GetProductVersion();

            this.SetDefaultShortcutsCommand = new DelegateCommand(this.SetDefaultShortcuts);
            this._commandBus = commandBus;

            this._enableAnalytics = settingsQuery.GetAnalyticsEnabled();
            this.OnPropertyChanged(nameof(this.EnableAnalytics));
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the set default shortcuts command.
        /// </summary>
        public ICommand SetDefaultShortcutsCommand { get; }

        /// <inheritdoc />
        public string AddinVersion { get; }

        /// <inheritdoc />
        public string DllVersion { get; }

        /// <inheritdoc />
        public string ProductVersion { get; }

        /// <summary>
        /// Gets or sets a value indicating whether analyitisc is enabled.
        /// </summary>
        public bool EnableAnalytics
        {
            get => this._enableAnalytics;
            set
            {
                if (this._enableAnalytics == value)
                {
                    return;
                }

                this.SendAnalyticsChangedCommandAsync(value).Forget();

                this._enableAnalytics = value;
                this.OnPropertyChanged();
            }
        }

        #endregion

        #region members

        private async Task SendAnalyticsChangedCommandAsync(bool enabled)
        {
            try
            {
                await this._commandBus.SendAsync(new EnableOrDisableAnalyticsCommand(enabled));
            }
            catch (Exception ex)
            {
                this.Log(ex);
            }
        }

        private void SetDefaultShortcuts(object obj)
        {
            this.SetDefaultShortcutsAsync(null).Forget();
        }

        private async Task SetDefaultShortcutsAsync(object obj)
        {
            try
            {
                await this._commandBus.SendAsync(new SetDefaultShortcutsCommand());
            }
            catch (Exception ex)
            {
                this.Log(ex);
            }
        }

        #endregion

        #region Nested type: Message

        /// <summary>
        /// Message displayed in the vie.
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Brush"></param>
        public record Message(string Text, Brush Brush);

        #endregion
    }
}