using System.Threading.Tasks;

namespace Twizzar.Design.CoreInterfaces.Common.VisualStudio
{
    /// <summary>
    /// Vi notification service for notifying the user.
    /// </summary>
    public interface IViNotificationService
    {
        /// <summary>
        /// Send a message to the info bar.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>A task.</returns>
        Task SendToInfoBarAsync(string message);

        /// <summary>
        /// Send a message to the output console.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>A task.</returns>
        Task SendToOutputAsync(string message);
    }
}