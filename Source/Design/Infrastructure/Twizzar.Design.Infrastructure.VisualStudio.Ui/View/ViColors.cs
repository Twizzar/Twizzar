using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;

namespace Twizzar.Design.Infrastructure.VisualStudio.Ui.View
{
    /// <summary>
    /// Mapping class to VisualStudio ViColors class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ViColors
    {
        #region properties

        /// <summary>
        /// Gets the background color key.
        /// </summary>
        public static ThemeResourceKey BackgroundColor { get; } = EnvironmentColors.ToolWindowBackgroundColorKey;

        /// <summary>
        /// Gets the background brush key.
        /// </summary>
        public static ThemeResourceKey BackgroundBrush { get; } = EnvironmentColors.ToolWindowBackgroundBrushKey;

        /// <summary>
        /// Gets the 2nd background brush.
        /// </summary>
        public static ThemeResourceKey BackgroundSecondaryBrush { get; } = EnvironmentColors.ToolWindowContentGridBrushKey;

        /// <summary>
        /// Gets the text brush key.
        /// </summary>
        public static ThemeResourceKey TextBrush { get; } = EnvironmentColors.ToolWindowTextBrushKey;

        /// <summary>
        /// Gets the 2nd text brush.
        /// </summary>
        public static ThemeResourceKey TextSecondaryBrush { get; } = EnvironmentColors.CommandBarDragHandleBrushKey;

        /// <summary>
        /// Gets the 3rd text brush.
        /// </summary>
        public static ThemeResourceKey TextTertiaryBrush { get; } = EnvironmentColors.ClassDesignerLassoBrushKey;

        /// <summary>
        /// Gets the caret brush key.
        /// </summary>
        public static ThemeResourceKey CaretBrush { get; } = TextBrush;

        /// <summary>
        /// Gets the inactive brush key.
        /// </summary>
        public static ThemeResourceKey InactiveTextBrush { get; } = EnvironmentColors.ComboBoxItemTextInactiveBrushKey;

        /// <summary>
        /// Gets the border brush key.
        /// </summary>
        public static ThemeResourceKey BorderBrush { get; } = EnvironmentColors.ToolWindowBorderBrushKey;

        /// <summary>
        /// Gets the glyph color key.
        /// </summary>
        public static ThemeResourceKey GlyphBrushKey { get; } = TreeViewColors.GlyphBrushKey;

        /// <summary>
        /// Gets the selected item active glyph color key.
        /// </summary>
        public static ThemeResourceKey GlyphExpandedBrush { get; } = TreeViewColors.SelectedItemActiveGlyphBrushKey;

        /// <summary>
        /// Gets the glyph mouse over color key.
        /// </summary>
        public static ThemeResourceKey GlyphMouseOverBrush { get; } = TreeViewColors.GlyphMouseOverBrushKey;

        /// <summary>
        /// Gets the glyph selected item inactive color key.
        /// </summary>
        public static ThemeResourceKey GlyphCollapsedBrush { get; } = TreeViewColors.SelectedItemInactiveGlyphBrushKey;

        #endregion
    }
}