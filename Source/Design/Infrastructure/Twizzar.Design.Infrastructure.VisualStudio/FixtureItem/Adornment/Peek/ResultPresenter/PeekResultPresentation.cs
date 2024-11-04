using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.Peek.ResultPresenter
{
    /// <summary>
    /// The peek presentation displays our ui in the peek view.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class PeekResultPresentation : IPeekResultPresentation, IDesiredHeightProvider
    {
        #region static fields and constants

        /// <summary>
        /// The supposed height of the border of the peek view.
        /// It is important to calculate the <see cref="DesiredHeight"/>.
        /// This value has been taken from a sample code in the repo:
        /// https://github.com/github/VisualStudio/tree/master/src/GitHub.InlineReviews.
        /// </summary>
        private const double PeekBorders = 28.0;

        #endregion

        #region fields

        private readonly IUiEventHub _eventHub;
        private IFixtureItemPeekResultContent _fixtureItemPeekResultContent;
        private AdornmentId _adornmentId;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="PeekResultPresentation"/> class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        public PeekResultPresentation(IUiEventHub eventHub)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(eventHub, nameof(eventHub))
                .ThrowWhenNull();

            this._eventHub = eventHub;
            eventHub.Subscribe<AdornmentSizeChangedEvent>(this, this.Handler);
        }

        #endregion

        #region events

        /// <inheritdoc />
        public event EventHandler<EventArgs> DesiredHeightChanged;

#pragma warning disable CS0067

        /// <inheritdoc />
        public event EventHandler IsDirtyChanged;

        /// <inheritdoc />
        public event EventHandler IsReadOnlyChanged;

        /// <inheritdoc />
        public event EventHandler<RecreateContentEventArgs> RecreateContent;
#pragma warning restore

        #endregion

        #region properties

        /// <inheritdoc />
        public double ZoomLevel
        {
            get => this._fixtureItemPeekResultContent.Zoom * 100f;
            set => this._fixtureItemPeekResultContent.Zoom = value / 100;
        }

        /// <inheritdoc />
        public bool IsDirty => false;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public double DesiredHeight =>
            (this._fixtureItemPeekResultContent?.ControlHeight is null)
                ? PeekBorders
                : this.FromDiuToPixel(this._fixtureItemPeekResultContent.ControlHeight + PeekBorders);

        #endregion

        #region members

        /// <inheritdoc />
        public void Dispose()
        {
            this._fixtureItemPeekResultContent.FixtureUserControl.KeyDown -= this.FixtureItemPanelOnKeyDown;
        }

        /// <inheritdoc />
        public bool TryOpen(IPeekResult otherResult) =>
            true;

        /// <inheritdoc />
        public bool TryPrepareToClose() =>
            true;

        /// <inheritdoc />
        public UIElement Create(IPeekSession session, IPeekResultScrollState scrollState)
        {
            this._adornmentId = AdornmentId.Parse(session.RelationshipName)
                .Match(
                    id => id,
                    failure => throw new InternalException(failure.Message));

            this._fixtureItemPeekResultContent =
                session.TextView.Properties.GetProperty<IFixtureItemPeekResultContent>(
                    typeof(IFixtureItemPeekResultContent));

            this._fixtureItemPeekResultContent.FixtureUserControl.KeyDown += this.FixtureItemPanelOnKeyDown;

            this.DesiredHeightChanged?.Invoke(this, EventArgs.Empty);

            return this._fixtureItemPeekResultContent.ScrollViewer;
        }

        /// <inheritdoc />
        public void ScrollIntoView(IPeekResultScrollState scrollState)
        {
            // scrollbar used FixtureItemPeekResultContent
        }

        /// <inheritdoc />
        public IPeekResultScrollState CaptureScrollState() =>
            null;

        /// <inheritdoc />
        public void Close()
        {
            this._eventHub.Publish(new PeekCollapsedEvent(this._adornmentId));
            this._fixtureItemPeekResultContent.FixtureUserControl.KeyDown -= this.FixtureItemPanelOnKeyDown;
            this._eventHub.Unsubscribe<AdornmentSizeChangedEvent>(this, this.Handler);
        }

        /// <inheritdoc />
        public void SetKeyboardFocus()
        {
            if (this._fixtureItemPeekResultContent != null)
            {
                Keyboard.Focus(this._fixtureItemPeekResultContent.FixtureUserControl);
                this._fixtureItemPeekResultContent.FixtureUserControl.Focus();
            }
        }

        /// <inheritdoc />
        public bool CanSave(out string defaultPath)
        {
            defaultPath = string.Empty;
            return false;
        }

        /// <inheritdoc />
        public bool TrySave(bool saveAs) =>
            true;

        private void FixtureItemPanelOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Handler(AdornmentSizeChangedEvent obj)
        {
            this.DesiredHeightChanged?.Invoke(this, EventArgs.Empty);
        }

        private double FromDiuToPixel(double actualHeight)
        {
            var dpi = VisualTreeHelper.GetDpi(
                this._fixtureItemPeekResultContent.FixtureUserControl ?? Application.Current.MainWindow);

            return dpi.DpiScaleY * actualHeight;
        }

        #endregion
    }
}