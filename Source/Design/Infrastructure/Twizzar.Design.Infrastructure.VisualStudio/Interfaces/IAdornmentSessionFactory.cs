using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment;
using Twizzar.Design.Ui.Interfaces.VisualStudio;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces
{
    /// <summary>
    /// Factory for creating and starting a <see cref="IAdornmentSession"/>.
    /// </summary>
    public interface IAdornmentSessionFactory
    {
        /// <summary>
        /// Create and start the session.
        /// </summary>
        /// <param name="viAdornment"></param>
        /// <param name="textView"></param>
        /// <param name="snapshotSpan"></param>
        /// <param name="fixtureItemPeekResultContent"></param>
        /// <param name="documentWriter"></param>
        /// <param name="peekBroker"></param>
        /// <returns>A new instance of <see cref="AdornmentSession"/>.</returns>
        public Task<IAdornmentSession> CreateAndStartAsync(
            IViAdornment viAdornment,
            ITextView textView,
            SnapshotSpan snapshotSpan,
            IFixtureItemPeekResultContent fixtureItemPeekResultContent,
            IDocumentWriter documentWriter,
            IPeekBroker peekBroker);
    }
}