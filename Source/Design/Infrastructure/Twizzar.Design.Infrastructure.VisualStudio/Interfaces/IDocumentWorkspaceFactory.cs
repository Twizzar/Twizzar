using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces
{
    /// <summary>
    /// Factory for creating <see cref="IDocumentWorkspace"/>.
    /// </summary>
    public interface IDocumentWorkspaceFactory : IFactory
    {
        /// <summary>
        /// Create a new <see cref="IDocumentWorkspace"/>.
        /// </summary>
        /// <param name="projectName">The project name.</param>
        /// <param name="documentFilePath">The file path ot the document.</param>
        /// <param name="peekBroker">The peek broker.</param>
        /// <param name="wpfTextView"></param>
        /// <returns>A new instance of <see cref="IDocumentWorkspace"/>.</returns>
        IDocumentWorkspace Create(string projectName, string documentFilePath, IPeekBroker peekBroker, IWpfTextView wpfTextView);
    }
}