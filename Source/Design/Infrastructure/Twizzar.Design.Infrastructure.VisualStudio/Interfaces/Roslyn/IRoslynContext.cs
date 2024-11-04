using Microsoft.CodeAnalysis;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <summary>
    /// Context for working with roslyn.
    /// </summary>
    public interface IRoslynContext
    {
        #region properties

        /// <summary>
        /// Gets the semantic model.
        /// </summary>
        SemanticModel SemanticModel { get; }

        /// <summary>
        /// Gets the document.
        /// </summary>
        Document Document { get; }

        /// <summary>
        /// Gets the root node.
        /// </summary>
        SyntaxNode RootNode { get; }

        /// <summary>
        /// Gets the compilations.
        /// </summary>
        Compilation Compilation { get; }

        #endregion
    }
}