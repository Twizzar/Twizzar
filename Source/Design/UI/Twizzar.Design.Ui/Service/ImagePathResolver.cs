using System;
using System.Windows.Media.Imaging;
using Twizzar.Design.Ui.Interfaces.Services;

namespace Twizzar.Design.Ui.Service
{
    /// <summary>
    /// Resolve the path for images in the folder View/Images/ .
    /// </summary>
    public class ImagePathResolver : IImagePathResolver
    {
        #region Implementation of IImagePathResolver

        /// <inheritdoc />
        public BitmapSource GetBitmapSource(string path) =>
            new BitmapImage(
                new Uri($"pack://application:,,,/Twizzar.Design.Ui;component/View/Images/{path}"));

        #endregion
    }
}