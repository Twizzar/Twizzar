using System;

namespace Twizzar.Design.CoreInterfaces.Common.VisualStudio
{
    /// <summary>
    /// Decorator for IWPFKeyboardTrackingService from visual studio.
    /// </summary>
    public interface IViKeyboardTrackingService
    {
        /// <summary>
        /// Begin tracking key down events for a specific window.
        /// </summary>
        /// <param name="handle">Get the handle with <c>new WindowInteropHelper(window).Handle</c>.</param>
        void BeginTrackingKeyDown(IntPtr handle);

        /// <summary>
        /// End the keyboard tracking.
        /// </summary>
        void EndTrackingKeyboard();
    }
}