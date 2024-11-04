using System.Windows.Media.Imaging;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Ui.Interfaces.Services
{
    /// <summary>
    /// Service for creating <see cref="BitmapSource"/> from a relative path to an image.
    /// The image will also be themed and when the theme changes these service will update all images known to it.
    /// </summary>
    public interface IImageProvider : IService
    {
        #region members

        /// <summary>
        /// Get the themed bitmap source of an icon.
        /// </summary>
        /// <param name="relativePath">The relative path to the icon without <c>View/Images</c>.</param>
        /// <returns>A observed writable bitmap which will be altered on vs theme changed.</returns>
        BitmapSource GetBitmapSource(string relativePath);

        #endregion
    }
}