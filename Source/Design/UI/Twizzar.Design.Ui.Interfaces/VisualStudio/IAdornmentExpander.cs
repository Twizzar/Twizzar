using System;
using System.Windows;

namespace Twizzar.Design.Ui.Interfaces.VisualStudio
{
    /// <summary>
    /// The AdornmentExpander for opening and closing the FixtureItemPanel.
    /// </summary>
    public interface IAdornmentExpander : IDisposable
    {
        #region properties

        /// <summary>
        /// Gets the underlying ui element.
        /// </summary>
        public UIElement UiElement { get; }

        /// <summary>
        /// Gets a value indicating whether the adornment should be expanded.
        /// </summary>
        public bool IsExpanded { get; }

        /// <summary>
        /// Gets the open/close shortcut tooltip for expander button.
        /// </summary>
        public string OpenOrCloseShortCutTooltip { get; }

        #endregion

        #region members

        /// <summary>
        /// Hides the toggle button.
        /// </summary>
        public void HideExpanderButton();

        /// <summary>
        /// Toggles the expander state.
        /// </summary>
        public void ToggleExpander();

        #endregion
    }
}