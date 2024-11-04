using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Community.VisualStudio.Toolkit;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.CoreInterfaces.Resources;
using Task = System.Threading.Tasks.Task;

namespace Twizzar.Design.Infrastructure.VisualStudio.Services.Notification
{
    /// <inheritdoc cref="IViNotificationService" />
    [ExcludeFromCodeCoverage]
    public class VsNotificationService : IViNotificationService
    {
        #region static fields and constants

        private const int MaxInfoBarMessageCount = 2;
        private const string OutputWindowName = "TWIZZAR";

        #endregion

        #region fields

        private readonly AsyncLazy<OutputWindowPane> _outputWindowPane;
        private readonly ConcurrentQueue<InfoBar> _infoBarMessages = new();

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="VsNotificationService"/> class.
        /// </summary>
        public VsNotificationService()
        {
            this._outputWindowPane =
                new AsyncLazy<OutputWindowPane>(
                    async () => await VS.Windows.CreateOutputWindowPaneAsync(OutputWindowName),
                    ThreadHelper.JoinableTaskFactory);
        }

        #endregion

        #region members

        /// <inheritdoc/>
        public async Task SendToInfoBarAsync(string message)
        {
            var msg =
                string.Format(CultureInfo.InvariantCulture, MessagesDesign.VsNotificationService_SendToInfoBarAsync, message);

            var model = new InfoBarModel(
                new[]
                {
                    new InfoBarTextSpan(msg),
                },
                KnownMonikers.EventError,
                true);

            var activeDocumentView = await VS.Documents.GetActiveDocumentViewAsync();

            if (activeDocumentView?.WindowFrame != null)
            {
                var infoBar = await VS.InfoBar.CreateAsync(activeDocumentView.WindowFrame, model);

                if (infoBar != null && await infoBar.TryShowInfoBarUIAsync())
                {
                    this._infoBarMessages.Enqueue(infoBar);
                }
            }

            this.CloseOldMessages();
        }

        /// <inheritdoc/>
        public async Task SendToOutputAsync(string message)
        {
            var pane = await this._outputWindowPane.GetValueAsync();
            await pane.WriteLineAsync(message);
        }

        private void CloseOldMessages()
        {
            while (this._infoBarMessages.Count > MaxInfoBarMessageCount)
            {
                if (this._infoBarMessages.TryDequeue(out var bar))
                {
                    bar.Close();
                }
            }
        }

        #endregion
    }
}