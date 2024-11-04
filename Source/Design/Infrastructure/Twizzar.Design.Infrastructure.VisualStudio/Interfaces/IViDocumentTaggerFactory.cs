using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces
{
    /// <summary>
    /// Factory for creating a <see cref="IViDocumentTagger"/>.
    /// </summary>
    public interface IViDocumentTaggerFactory
    {
        /// <summary>
        /// Create a new <see cref="IViDocumentTagger"/>.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="peekBroker"></param>
        /// <param name="documentFilePath">The document file path.</param>
        /// <param name="projectName">The project name.</param>
        /// <returns>A new <see cref="IViDocumentTagger"/>.</returns>
        IViDocumentTagger Create(IWpfTextView view, IPeekBroker peekBroker, string documentFilePath, string projectName);
    }
}