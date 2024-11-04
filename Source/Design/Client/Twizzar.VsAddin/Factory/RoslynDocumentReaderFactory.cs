using System.Diagnostics.CodeAnalysis;

using Autofac;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Common.FixtureItem.Adornment;
using Twizzar.SharedKernel.Factories;

namespace Twizzar.VsAddin.Factory
{
    /// <inheritdoc cref="IRoslynDocumentReaderFactory"/>
    [ExcludeFromCodeCoverage]
    public class RoslynDocumentReaderFactory : FactoryBase, IRoslynDocumentReaderFactory
    {
        private readonly DocumentReaderFactory _documentReaderFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynDocumentReaderFactory"/> class.
        /// </summary>
        /// <param name="componentContext">The autofac component context.</param>
        /// <param name="documentReaderFactory">The document reader factory.</param>
        public RoslynDocumentReaderFactory(IComponentContext componentContext, DocumentReaderFactory documentReaderFactory)
            : base(componentContext)
        {
            this._documentReaderFactory = documentReaderFactory;
        }

        /// <summary>
        /// Factory used by autofac.
        /// </summary>
        /// <param name="filePath">The file path to the document.</param>
        /// <param name="projectName">The project name.</param>
        /// <returns>A new document reader resolved by autofac.</returns>
        public delegate IDocumentReader DocumentReaderFactory(
            string filePath,
            string projectName);

        #region Implementation of IRoslynDocumentReaderFactory

        /// <inheritdoc />
        public IDocumentReader CreateViContainerReader(string documentFilePath, string projectName) =>
            this._documentReaderFactory(
                documentFilePath,
                projectName);

        #endregion
    }
}