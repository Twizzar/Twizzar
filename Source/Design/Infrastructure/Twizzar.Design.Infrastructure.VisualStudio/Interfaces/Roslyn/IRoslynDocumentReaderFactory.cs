using Twizzar.Design.CoreInterfaces.Adornment;

namespace Twizzar.Design.CoreInterfaces.Common.FixtureItem.Adornment
{
    /// <summary>
    /// Factory for creating a <see cref="IDocumentReader"/> which uses the roslyn analyzer.
    /// </summary>
    public interface IRoslynDocumentReaderFactory
    {
        /// <summary>
        /// Create a new <see cref="IDocumentReader"/>.
        /// </summary>
        /// <param name="documentFilePath">The path to the document file.</param>
        /// <param name="projectName">The project name.</param>
        /// <returns>A new instance of <see cref="IDocumentReader"/>.</returns>
        IDocumentReader CreateViContainerReader(string documentFilePath, string projectName);
    }
}