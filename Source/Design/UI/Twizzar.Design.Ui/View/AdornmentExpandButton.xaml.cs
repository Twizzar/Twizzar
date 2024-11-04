using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;

using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Resources;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Design.Ui.View
{
    /// <summary>
    /// Interaction logic for AdornmentExpandButton.xaml.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed partial class AdornmentExpandButton : IAdornmentExpander
    {
        #region fields

        private readonly AdornmentId _adornmentId;
        private readonly IUiEventHub _eventHub;
        private readonly IVsCommandQuery _vsCommandQuery;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdornmentExpandButton"/> class.
        /// </summary>
        /// <param name="adornmentId">The adornment id.</param>
        /// <param name="uiEventHub">The ui event hub.</param>
        /// <param name="statusPanelViewModel">The status panel view model.</param>
        /// <param name="vsCommandQuery">The vs command query.</param>
        public AdornmentExpandButton(
            AdornmentId adornmentId,
            IUiEventHub uiEventHub,
            IStatusPanelViewModel statusPanelViewModel,
            IVsCommandQuery vsCommandQuery)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(adornmentId, nameof(adornmentId))
                .Parameter(uiEventHub, nameof(uiEventHub))
                .Parameter(statusPanelViewModel, nameof(statusPanelViewModel))
                .Parameter(vsCommandQuery, nameof(vsCommandQuery))
                .ThrowWhenNull();

            this._adornmentId = adornmentId;
            this._eventHub = uiEventHub;
            this._vsCommandQuery = vsCommandQuery;

            this.Initialize();

            this.InitializeComponent();
            this.InternalToggleButton.Checked += this.InternalToggleButtonOnChecked;
            this.InternalToggleButton.Unchecked += this.InternalToggleButtonOnUnchecked;
            this.IsExpanded = false;

            this._eventHub.Subscribe<PeekCollapsedEvent>(this, this.PeekCollapsed);
            this._eventHub.Subscribe<LockAdornmentExpanderEvent>(this, this.Lock);
            this._eventHub.Subscribe<ReleaseAdornmentExpanderEvent>(this, this.Release);
            this.FixtureItemNodeStatus.DataContext = statusPanelViewModel;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public UIElement UiElement => this;

        /// <inheritdoc />
        public bool IsExpanded { get; private set; }

        /// <inheritdoc />
        public string OpenOrCloseShortCutTooltip { get; private set; }

        #endregion

        #region members

        /// <inheritdoc />
        public void HideExpanderButton()
        {
            this.InternalToggleButton.Visibility = Visibility.Collapsed;
        }

        /// <inheritdoc />
        public void ToggleExpander()
        {
            this.IsExpanded = !this.IsExpanded;
            this.InternalToggleButton.IsChecked = this.IsExpanded;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.InternalToggleButton.Checked -= this.InternalToggleButtonOnChecked;
            this.InternalToggleButton.Unchecked -= this.InternalToggleButtonOnUnchecked;
        }

        [SuppressMessage(
            "Major Bug",
            "S3168:\"async\" methods should not return \"void\"",
            Justification = "Ui async void ok.")]
        private async void Initialize()
        {
            try
            {
                this.OpenOrCloseShortCutTooltip =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        MessagesDesign.Expander_ToolTip,
                        await this._vsCommandQuery.GetShortCutOfOpenCloseCommandAsync());
            }
            catch (Exception e)
            {
                this.Log(e);
            }
        }

        private void Lock(LockAdornmentExpanderEvent e)
        {
            if (e.AdornmentId == this._adornmentId)
            {
                this.InternalToggleButton.IsChecked = false;
                this.InternalToggleButton.Visibility = Visibility.Collapsed;
            }
        }

        private void Release(ReleaseAdornmentExpanderEvent e)
        {
            if (e.AdornmentId == this._adornmentId)
            {
                this.InternalToggleButton.Visibility = Visibility.Visible;
            }
        }

        private void PeekCollapsed(PeekCollapsedEvent e)
        {
            if (e.AdornmentId == this._adornmentId)
            {
                this.IsExpanded = false;
                this.InternalToggleButton.IsChecked = false;
            }
        }

        private void InternalToggleButtonOnUnchecked(object sender, RoutedEventArgs e)
        {
            if (this.IsExpanded)
            {
                this.IsExpanded = false;
                this._eventHub.Publish(new AdornmentExpandedOrCollapsedEvent(this._adornmentId, false, this));
            }
        }

        private void InternalToggleButtonOnChecked(object sender, RoutedEventArgs e)
        {
            if (!this.IsExpanded)
            {
                this.IsExpanded = true;
                this._eventHub.Publish(new AdornmentExpandedOrCollapsedEvent(this._adornmentId, true, this));
            }
        }

        #endregion
    }
}