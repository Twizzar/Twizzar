using System.Windows.Media.Imaging;

namespace Twizzar.Design.Ui.Interfaces.Services
{
    /// <summary>
    /// Resolves a path to an image to an <see cref="BitmapSource"/>.
    /// </summary>
    public interface IImagePathResolver
    {
        /// <summary>
        /// Get the bitmap source from a relative file path.
        /// </summary>
        /// <param name="path">The relative file path.</param>
        /// <returns>A new Bitmap source.</returns>
        BitmapSource GetBitmapSource(string path);
    }
}