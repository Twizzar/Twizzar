using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.VisualStudio.Enums;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.View
{
    /// <summary>
    /// Host of the definition UI.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class FixtureItemPeekResultContent : ScrollViewer,
        IFixtureItemPeekResultContent,
        IHasEnsureHelper,
        IHasLogger
    {
        #region fields

        private readonly IUiEventHub _eventHub;
        private readonly IViKeyboardTrackingService _viKeyboardTrackingService;
        private readonly FrameworkElement _fixtureControl;

        private AdornmentId _adornmentId;
        private IAdornmentInformation _adornmentInformation;
        private Maybe<Window> _parentWindow = Maybe.None();
        private double _zoom;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemPeekResultContent"/> class.
        /// </summary>
        /// <param name="eventHub">The ui event hub.</param>
        /// <param name="viKeyboardTrackingService">The vs wpf keyboard tracking service.</param>
        /// <param name="viewModel"></param>
        public FixtureItemPeekResultContent(
            IUiEventHub eventHub,
            IViKeyboardTrackingService viKeyboardTrackingService,
            IFixtureItemViewModel viewModel)
        {
            this.EnsureMany()
                .Parameter(eventHub, nameof(eventHub))
                .Parameter(viKeyboardTrackingService, nameof(viKeyboardTrackingService))
                .Parameter(viewModel, nameof(viewModel))
                .ThrowWhenNull();

            this._eventHub = eventHub;
            this._viKeyboardTrackingService = viKeyboardTrackingService;

            // initialize fixture control
            this._fixtureControl = new FixtureControl();
            this._fixtureControl.BeginInit();
            this._fixtureControl.SizeChanged += this.FixtureControlSizeChanged;
            this.DataContext = viewModel;

            // set some ScrollViewer properties
            this.Content = this._fixtureControl;

            // set some ScrollViewer properties
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Top;
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            this._eventHub.Subscribe<AdornmentExpandedOrCollapsedEvent>(this, this.AdornmentExpandCollapse);
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public double Zoom
        {
            get => this._zoom;
            set
            {
                this._zoom = value;
                this.LayoutTransform = new ScaleTransform(value, value);
                this.UpdateLayout();
            }
        }

        /// <inheritdoc />
        public double ControlHeight => this._fixtureControl?.ActualHeight * this.Zoom ?? 0;

        /// <inheritdoc />
        public UIElement ScrollViewer => this;

        /// <inheritdoc />
        public UIElement FixtureUserControl => this._fixtureControl;

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        private IFixtureItemViewModel ViewModel => (IFixtureItemViewModel)this.DataContext;

        #endregion

        #region members

        /// <inheritdoc />
        public async Task InitializeAsync(
            AdornmentId adornmentId,
            IAdornmentInformation adornmentInformation,
            IDocumentWriter documentWriter,
            IStatusPanelViewModel statusPanelViewModel,
            ICompilationTypeQuery compilationTypeQuery,
            CancellationToken cancellationToken)
        {
            this._adornmentId = adornmentId;
            this._adornmentInformation = adornmentInformation;

            await this.ViewModel.InitializeAsync(
                adornmentInformation,
                adornmentId,
                documentWriter,
                statusPanelViewModel,
                compilationTypeQuery,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(IAdornmentInformation adornmentInformation)
        {
            await this.ViewModel.UpdateAsync(adornmentInformation);
        }

        /// <inheritdoc />
        public void MoveFocus(ViEnterFocusPosition direction)
        {
            switch (direction)
            {
                case ViEnterFocusPosition.First:
                    this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    break;
                case ViEnterFocusPosition.Last:
                    this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Last));
                    break;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this._fixtureControl.SizeChanged -= this.FixtureControlSizeChanged;
            this.ViewModel?.Dispose();
            this.EndKeyboardTracking();
        }

        /// <inheritdoc />
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            var window = Window.GetWindow(this);
            this._parentWindow = Maybe.ToMaybe(window);
        }

        /// <inheritdoc />
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            this.EndKeyboardTracking();
        }

        /// <inheritdoc />
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            this._parentWindow.IfSome(
                window =>
                {
                    this._viKeyboardTrackingService.BeginTrackingKeyDown(
                        new WindowInteropHelper(window).Handle);
                });
        }

        private void AdornmentExpandCollapse(AdornmentExpandedOrCollapsedEvent obj)
        {
            if (!obj.IsExpanded)
            {
                this.EndKeyboardTracking();
            }
        }

        private void FixtureControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.HeightChanged && !e.WidthChanged)
            {
                return;
            }

            // call to peek view
            this._eventHub.Publish(new AdornmentSizeChangedEvent(this._adornmentId, this._adornmentInformation));
        }

        private void EndKeyboardTracking()
        {
            this._viKeyboardTrackingService.EndTrackingKeyboard();
        }

        #endregion
    }
}