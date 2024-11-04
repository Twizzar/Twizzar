using Microsoft.CodeAnalysis;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn.DocumentReader
{
    /// <summary>
    /// Factory for creating an <see cref="IItemBuilderFinder"/>.
    /// </summary>
    public interface IItemBuilderFinderFactory
    {
        /// <summary>
        /// Create an new <see cref="IItemBuilderFinder"/>.
        /// </summary>
        /// <param name="semanticModel"></param>
        /// <returns>.</returns>
        IItemBuilderFinder Create(SemanticModel semanticModel);
    }
}