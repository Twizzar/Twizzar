using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Media.Imaging;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Infrastructure.VisualStudio.Ui;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.Design.Ui.Interfaces.Services;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Ui.Service
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class ImageProvider : IImageProvider
    {
        #region fields

        private static readonly string[] NotThemableNames =
        {
            "StatusCriticalError",
            "StatusInvalid",
            "StatusOK",
            "StatusWarning",
            "StatusInformation",
        };

        private readonly IconThemer _iconThemer;
        private readonly IImagePathResolver _imagePathResolver;

        private readonly HashSet<WeakReference<WriteableBitmap>> _wBitmaps =
            new();

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageProvider"/> class.
        /// </summary>
        /// <param name="eventHub">The event ui hub.</param>
        /// <param name="imagePathResolver">The image path resolver.</param>
        public ImageProvider(
            IUiEventHub eventHub,
            IImagePathResolver imagePathResolver)
        {
            this._imagePathResolver = imagePathResolver;
            this._iconThemer = new IconThemer();
            eventHub.Subscribe<VsThemeChangedEvent>(this, this.VsThemeChanged);
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public BitmapSource GetBitmapSource(string relativePath)
        {
            this.EnsureParameter(relativePath, nameof(relativePath)).ThrowWhenNull();

            var bitmapSource = this._imagePathResolver.GetBitmapSource(relativePath);

            if (!IsThemeable(relativePath))
            {
                return bitmapSource;
            }

            var wBitmap = new WriteableBitmap(bitmapSource);
            this._iconThemer.ThemeIcon(wBitmap);

            this._wBitmaps.RemoveWhere(reference => !reference.TryGetTarget(out _));
            this._wBitmaps.Add(new WeakReference<WriteableBitmap>(wBitmap));
            return wBitmap;
        }

        private static bool IsThemeable(string relativePath) =>
            !NotThemableNames.Any(relativePath.Contains);

        private void VsThemeChanged(VsThemeChangedEvent obj)
        {
            this._wBitmaps.ExceptWith(this.ApplyChanges());
        }

        private IEnumerable<WeakReference<WriteableBitmap>> ApplyChanges()
        {
            foreach (var weakReference in this._wBitmaps)
            {
                if (weakReference.TryGetTarget(out var wBitmap))
                {
                    this._iconThemer.ThemeIcon(wBitmap);
                }
                else
                {
                    yield return weakReference;
                }
            }
        }

        #endregion
    }
}