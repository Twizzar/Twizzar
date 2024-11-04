using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Language.Intellisense;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;

namespace Twizzar.Design.Infrastructure.VisualStudio.Services
{
    /// <inheritdoc cref="IViKeyboardTrackingService" />
    [ExcludeFromCodeCoverage]
    public class ViKeyboardTrackingService : IViKeyboardTrackingService
    {
        #region fields

        private readonly IWpfKeyboardTrackingService _wpfKeyboardTrackingService;
        private readonly object _lockObject = new();

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViKeyboardTrackingService"/> class.
        /// </summary>
        /// <param name="wpfKeyboardTrackingService"></param>
        public ViKeyboardTrackingService(IWpfKeyboardTrackingService wpfKeyboardTrackingService)
        {
            this._wpfKeyboardTrackingService = wpfKeyboardTrackingService;
        }

        #endregion

        #region members

        /// <inheritdoc />
        public void BeginTrackingKeyDown(IntPtr handle)
        {
            const uint keyDown = 0x0100U; // see https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-keydown

            lock (this._lockObject)
            {
                this._wpfKeyboardTrackingService.BeginTrackingKeyboard(
                    handle,
                    new[] { keyDown });
            }
        }

        /// <inheritdoc />
        public void EndTrackingKeyboard()
        {
            lock (this._lockObject)
            {
                this._wpfKeyboardTrackingService.EndTrackingKeyboard();
            }
        }

        #endregion
    }
}