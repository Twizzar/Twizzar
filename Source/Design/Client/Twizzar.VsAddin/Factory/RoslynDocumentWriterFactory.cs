using System.Diagnostics.CodeAnalysis;

using Autofac;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Common.FixtureItem.Adornment;
using Twizzar.SharedKernel.Factories;

namespace Twizzar.VsAddin.Factory
{
    /// <inheritdoc cref="IRoslynDocumentWriterFactory"/>
    [ExcludeFromCodeCoverage]
    public class RoslynDocumentWriterFactory : FactoryBase, IRoslynDocumentWriterFactory
    {
        #region fields

        private readonly DocumentWriterFactory _documentWriterFactory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynDocumentWriterFactory"/> class.
        /// </summary>
        /// <param name="componentContext">The autofac component context.</param>
        /// <param name="documentWriterFactory">The document writer factory.</param>
        public RoslynDocumentWriterFactory(
            IComponentContext componentContext,
            DocumentWriterFactory documentWriterFactory)
            : base(componentContext)
        {
            this._documentWriterFactory = documentWriterFactory;
        }

        #endregion

        #region delegates

        /// <summary>
        /// Factory used by autofac.
        /// </summary>
        /// <param name="filePath">The file path to the document.</param>
        /// <returns>A new document writer resolved by autofac.</returns>
        public delegate IDocumentWriter DocumentWriterFactory(string filePath);

        #endregion

        #region members

        /// <inheritdoc />
        public IDocumentWriter CreateDocumentWriter(string documentFilePath) =>
            this._documentWriterFactory(documentFilePath);

        #endregion
    }
}