using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn.DocumentReader
{
    /// <summary>
    /// Walks through the syntax tree and finds ItemBuilder object creations.
    /// </summary>
    public interface IItemBuilderFinder
    {
        /// <summary>
        /// Find the item builder in the given syntax tree.
        /// </summary>
        /// <param name="rootNode"></param>
        /// <returns>A sequence of ItemBuilder information.</returns>
        IEnumerable<IItemBuilderInformation> FindBuilderInformation(SyntaxNode rootNode);
    }
}