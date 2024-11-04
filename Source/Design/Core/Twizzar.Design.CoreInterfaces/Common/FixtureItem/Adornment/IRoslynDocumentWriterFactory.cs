using Twizzar.Design.CoreInterfaces.Adornment;

namespace Twizzar.Design.CoreInterfaces.Common.FixtureItem.Adornment
{
    /// <summary>
    /// Factory for creating a <see cref="IDocumentWriter"/> which uses the roslyn writer.
    /// </summary>
    public interface IRoslynDocumentWriterFactory
    {
        /// <summary>
        /// Create a new <see cref="IDocumentWriter"/>.
        /// </summary>
        /// <param name="documentFilePath">The path to the document file.</param>
        /// <returns>A new instance of <see cref="IDocumentWriter"/>.</returns>
        IDocumentWriter CreateDocumentWriter(string documentFilePath);
    }
}