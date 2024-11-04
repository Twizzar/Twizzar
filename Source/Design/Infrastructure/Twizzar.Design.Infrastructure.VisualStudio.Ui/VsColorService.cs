using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Twizzar.Design.Infrastructure.VisualStudio.Ui.View;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using ViCommon.Functional.Monads.MaybeMonad;
using Brush = System.Windows.Media.Brush;
using VsColors = Twizzar.Design.Ui.Interfaces.VisualStudio.VsColors;
using VsFonts = Twizzar.Design.Ui.Interfaces.VisualStudio.VsFonts;

namespace Twizzar.Design.Infrastructure.VisualStudio.Ui
{
    /// <summary>
    /// Vs Color service.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class VsColorService : IVsColorService
    {
        #region Implementation of IVsColorService

        /// <inheritdoc />
        public async Task<Maybe<Brush>> GetVsOptionBrushAsync(string vsOptionBrushKey)
        {
            // Switch to the main thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var storage = Package.GetGlobalService(typeof(SVsFontAndColorStorage)) as IVsFontAndColorStorage;

            try
            {
                var category = Microsoft.VisualStudio.Editor.DefGuidList.guidTextEditorFontCategory;

                var success = storage?.OpenCategory(ref category, (uint)(__FCSTORAGEFLAGS.FCSF_READONLY | __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS | __FCSTORAGEFLAGS.FCSF_NOAUTOCOLORS));

                if (success == 0)
                {
                    var colors = new ColorableItemInfo[1];
                    var hresult = storage.GetItem(vsOptionBrushKey, colors);

                    if (hresult == 0)
                    {
                        var color = ColorTranslator.FromOle(Convert.ToInt32(colors[0].crForeground));
                        var c = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
                        return new SolidColorBrush { Color = c };
                    }
                }
            }
            finally
            {
                storage?.CloseCategory();
            }

            return Maybe.None();
        }

        /// <inheritdoc />
        public string ResolveVsFont(Design.Ui.Interfaces.VisualStudio.VsFonts vsFont) =>
            vsFont switch
            {
                VsFonts.FontFamilyKey => ViFonts.FontFamilyKey,
                VsFonts.FontSizeKey => ViFonts.FontSizeKey,
                _ => throw new ArgumentOutOfRangeException(nameof(vsFont), vsFont, null),
            };

        /// <inheritdoc />
        public object ResolveVsColor(Design.Ui.Interfaces.VisualStudio.VsColors vsColor) =>
            vsColor switch
            {
                VsColors.BackgroundColor => ViColors.BackgroundColor,
                VsColors.BackgroundBrush => ViColors.BackgroundBrush,
                VsColors.BackgroundSecondaryBrush => ViColors.BackgroundSecondaryBrush,
                VsColors.TextBrush => ViColors.TextBrush,
                VsColors.TextSecondaryBrush => ViColors.TextSecondaryBrush,
                VsColors.TextTertiaryBrush => ViColors.TextTertiaryBrush,
                VsColors.CaretBrush => ViColors.CaretBrush,
                VsColors.InactiveTextBrush => ViColors.InactiveTextBrush,
                VsColors.BorderBrush => ViColors.BorderBrush,
                VsColors.GlyphBrushKey => ViColors.GlyphBrushKey,
                VsColors.GlyphExpandedBrush => ViColors.GlyphExpandedBrush,
                VsColors.GlyphMouseOverBrush => ViColors.GlyphMouseOverBrush,
                VsColors.GlyphCollapsedBrush => ViColors.GlyphCollapsedBrush,
                _ => throw new ArgumentOutOfRangeException(nameof(vsColor), vsColor, null),
            };

        #endregion
    }
}
