using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Shell;

namespace Twizzar.Design.Infrastructure.VisualStudio.Ui.View
{
    /// <summary>
    /// Mapping class to VisualStudio VsFonts class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ViFonts
    {
        #region properties

        /// <summary>
        /// Gets the environment font family key.
        /// </summary>
        public static string FontFamilyKey => VsFonts.EnvironmentFontFamilyKey;

        /// <summary>
        /// Gets the environment font size key.
        /// </summary>
        public static string FontSizeKey => VsFonts.EnvironmentFontSizeKey;

        #endregion
    }
}