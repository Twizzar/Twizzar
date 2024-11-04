using System.Threading.Tasks;
using System.Windows.Media;

using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.Interfaces.VisualStudio
{
    /// <summary>
    /// Visual studio color service.
    /// </summary>
    public interface IVsColorService
    {
        /// <summary>
        /// Determines the brush from given option key.
        /// Tools -> Options -> Environment -> Fonts and Colors.
        /// </summary>
        /// <param name="vsOptionBrushKey"></param>
        /// <returns>Task of maybe brush. None if key was not found.</returns>
        public Task<Maybe<Brush>> GetVsOptionBrushAsync(string vsOptionBrushKey);

        /// <summary>
        /// Resolves mapped fonts.
        /// </summary>
        /// <param name="vsFont"></param>
        /// <returns>The font key as string.</returns>
        public string ResolveVsFont(VsFonts vsFont);

        /// <summary>
        /// Resolves mapped vs colors.
        /// </summary>
        /// <param name="vsColor"></param>
        /// <returns>The vs color as ThemeResourceKey.</returns>
        public object ResolveVsColor(VsColors vsColor);
    }
}
