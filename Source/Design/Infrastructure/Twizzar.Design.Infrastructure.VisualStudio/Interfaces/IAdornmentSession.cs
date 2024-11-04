using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Ui.Interfaces.VisualStudio;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces
{
    /// <summary>
    /// Represents an adornment session. The session is opened when the peek view is created and closed when the peek view will be closed.
    /// </summary>
    public interface IAdornmentSession : IDisposable
    {
        #region members

        /// <summary>
        /// Start a new session.
        /// </summary>
        /// <param name="textView"></param>
        /// <param name="snapshotSpan"></param>
        /// <param name="fixtureItemPeekResultContent"></param>
        /// <param name="documentWriter"></param>
        /// <param name="peekBroker"></param>
        /// <returns>A task.</returns>
        Task StartAsync(
            ITextView textView,
            SnapshotSpan snapshotSpan,
            IFixtureItemPeekResultContent fixtureItemPeekResultContent,
            IDocumentWriter documentWriter,
            IPeekBroker peekBroker);

        /// <summary>
        /// Close the session.
        /// </summary>
        /// <returns>A task.</returns>
        Task CloseAsync();

        #endregion
    }
}