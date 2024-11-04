using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Ui.Interfaces.Factories;

namespace Twizzar.Design.Infrastructure.VisualStudio.Ui.View.About
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class AboutWindow : Window
    {
        #region fields

        private readonly IViKeyboardTrackingService _viKeyboardTrackingService;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutWindow"/> class.
        /// </summary>
        /// <param name="aboutViewModelFactory">The about view model factory.</param>
        /// <param name="viKeyboardTrackingService"></param>
        public AboutWindow(
            IAboutViewModelFactory aboutViewModelFactory,
            IViKeyboardTrackingService viKeyboardTrackingService)
        {
            this._viKeyboardTrackingService = viKeyboardTrackingService;
            this.InitializeComponent();
            this.DataContext = aboutViewModelFactory.CreateAboutViewModel();
        }

        #endregion

        #region members

        /// <inheritdoc />
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            e.Cancel = true;
            this.Hide();
        }

        #region Overrides of UIElement

        /// <inheritdoc />
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            this._viKeyboardTrackingService.BeginTrackingKeyDown(new WindowInteropHelper(this).Handle);
            base.OnGotKeyboardFocus(e);
        }

        /// <inheritdoc />
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            this._viKeyboardTrackingService.EndTrackingKeyboard();
            base.OnLostKeyboardFocus(e);
        }

        #endregion

        /// <inheritdoc />
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            this._viKeyboardTrackingService.BeginTrackingKeyDown(new WindowInteropHelper(this).Handle);
            base.OnGotFocus(e);
        }

        /// <inheritdoc />
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            this._viKeyboardTrackingService.EndTrackingKeyboard();
            base.OnLostFocus(e);
        }

        #endregion
    }
}