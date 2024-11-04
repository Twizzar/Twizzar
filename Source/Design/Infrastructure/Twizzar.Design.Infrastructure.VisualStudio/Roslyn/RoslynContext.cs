using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <inheritdoc cref="IRoslynContext" />
    public class RoslynContext : ValueObject, IRoslynContext
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynContext"/> class.
        /// </summary>
        /// <param name="semanticModel"></param>
        /// <param name="document"></param>
        /// <param name="rootNode"></param>
        /// <param name="compilation"></param>
        public RoslynContext(
            SemanticModel semanticModel,
            Document document,
            SyntaxNode rootNode,
            Compilation compilation)
        {
            this.SemanticModel = semanticModel;
            this.Document = document;
            this.RootNode = rootNode;
            this.Compilation = compilation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynContext"/> class.
        /// </summary>
        /// <param name="context"></param>
        public RoslynContext(IRoslynContext context)
            : this(context.SemanticModel, context.Document, context.RootNode, context.Compilation)
        {
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public SemanticModel SemanticModel { get; }

        /// <inheritdoc />
        public Document Document { get; }

        /// <inheritdoc />
        public SyntaxNode RootNode { get; }

        /// <inheritdoc />
        public Compilation Compilation { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.SemanticModel;
            yield return this.Document;
            yield return this.RootNode;
            yield return this.Compilation;
        }

        #endregion
    }
}