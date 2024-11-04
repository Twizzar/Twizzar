using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;

using Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces
{
    /// <inheritdoc cref="IRoslynContextQuery" />
    public class RoslynContextQuery : IRoslynContextQuery
    {
        #region fields

        private readonly Workspace _workspace;
        private readonly Dictionary<string, DocumentId> _documentIdCache = new();

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynContextQuery"/> class.
        /// </summary>
        /// <param name="workspace"></param>
        public RoslynContextQuery(Workspace workspace)
        {
            this.EnsureParameter(workspace, nameof(workspace)).ThrowWhenNull();

            this._workspace = workspace;
            this._workspace.DocumentClosed += this.WorkspaceOnDocumentClosed;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public async Task<IResult<IRoslynContext, Failure>> GetContextAsync(
            string filePath,
            CancellationToken cancellationToken = default)
        {
            var document = this.GetDocument(filePath);
            var rootNode = await document.MapSuccessAsync(d => GetRootNodeAsync(d, cancellationToken));
            var compilation = await document.MapSuccessAsync(d => GetCompilationAsync(d, cancellationToken));

            return
                from node in rootNode
                from doc in document
                from comp in compilation
                from sm in Result.Success<SemanticModel, Failure>(comp.GetSemanticModel(node.SyntaxTree, true))
                select new RoslynContext(sm, doc, node, comp);
        }

        private static async Task<Compilation> GetCompilationAsync(Document document, CancellationToken cancellationToken)
        {
            var c = await document.Project.GetCompilationAsync(cancellationToken);
            return c.WithOptions(c.Options.WithMetadataImportOptions(MetadataImportOptions.All));
        }

        private void WorkspaceOnDocumentClosed(object sender, DocumentEventArgs e)
        {
            if (e.Document.FilePath is not null)
            {
                this._documentIdCache.Remove(e.Document.FilePath);
            }
        }

        private IResult<Document, Failure> GetDocument(string filePath) =>
            from documentId in this.GetDocumentId(filePath)
            from document in Maybe.ToMaybe(this._workspace.CurrentSolution.GetDocument(documentId))
                .ToResult(new Failure($"Cannot get document for the document id {documentId}"))
            select document;

        private IResult<DocumentId, Failure> GetDocumentId(string filePath) =>
            this._documentIdCache.GetMaybe(filePath)
                .ToResult(new Failure($"Document Id {filePath} not found in the cache."))
                .BindFailure(
                    _ =>
                        this._workspace.CurrentSolution.GetDocumentIdsWithFilePath(filePath)
                            .FirstOrNone()
                            .ToResult(new Failure($"Cannot find document under {filePath}")));

        private static Task<SyntaxNode> GetRootNodeAsync(Document document, CancellationToken cancellation) =>
            document.GetSyntaxTreeAsync(cancellation)
                .Map(syntaxTree => syntaxTree.GetRoot());

        #endregion
    }
}