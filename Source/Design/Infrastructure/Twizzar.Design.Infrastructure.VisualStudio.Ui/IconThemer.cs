using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.PlatformUI;

namespace Twizzar.Design.Infrastructure.VisualStudio.Ui
{
    /// <summary>
    /// Service for change the theme of an icon.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class IconThemer
    {
        private uint _bgColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="IconThemer"/> class.
        /// </summary>
        public IconThemer()
        {
            this.SetBgColor();
        }

        /// <summary>
        /// Change the theme of a writable bitmap.
        /// </summary>
        /// <param name="writeableBitmap">The writable bitmap.</param>
        public void ThemeIcon(WriteableBitmap writeableBitmap)
        {
            this.SetBgColor();
            ThemeIcon(writeableBitmap, this._bgColor);
        }

        private void SetBgColor()
        {
            var key = EnvironmentColors.ToolboxBackgroundBrushKey;
            var color = Color.White;

            try
            {
                color = VSColorTheme.GetThemedColor(key);
            }
            catch (Exception)
            {
                // Ignored
            }

            this._bgColor = ToUint(color);
        }

        private static void ThemeIcon(WriteableBitmap writeableBitmap, uint backgroundColor)
        {
            var bufferSize = writeableBitmap.PixelHeight * writeableBitmap.BackBufferStride;
            var buffer = new byte[bufferSize];
            writeableBitmap.CopyPixels(buffer, writeableBitmap.BackBufferStride, 0);

            try
            {
                writeableBitmap.Lock();

                var success = ImageThemingUtilities.ThemeDIBits(
                    buffer.Length,
                    buffer,
                    writeableBitmap.PixelWidth,
                    writeableBitmap.PixelHeight,
                    true,
                    backgroundColor);

                if (success)
                {
                    var rect = new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight);

                    writeableBitmap.WritePixels(
                        rect,
                        buffer,
                        writeableBitmap.BackBufferStride,
                        0);

                    writeableBitmap.AddDirtyRect(rect);
                }
            }
            catch (Exception)
            {
                // Do nothing
            }
            finally
            {
                writeableBitmap.Unlock();
            }
        }

        private static uint ToUint(System.Drawing.Color c) =>
            (uint)(((c.A << 24) | (c.R << 16) | (c.G << 8) | c.B) & 0xffffffffL);
    }
}