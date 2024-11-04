using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status
{
    /// <summary>
    /// View Model for a status icon.
    /// </summary>
    public interface IStatusIconViewModel
    {
        #region properties

        /// <summary>
        /// Gets the command when the icon is clicked.
        /// </summary>
        ICommand ClickCommand { get; }

        /// <summary>
        /// Gets the tooltip text.
        /// </summary>
        string ToolTip { get; }

        /// <summary>
        /// Gets the icon image.
        /// </summary>
        BitmapSource Image { get; }

        #endregion
    }
}