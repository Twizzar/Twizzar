using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn.DocumentReader;

namespace Twizzar.VsAddin.Factory
{
    /// <summary>
    /// Factory for creating an <see cref="IItemBuilderFinderFactory"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ItemBuilderFinderFactory : IItemBuilderFinderFactory
    {
        #region fields

        private readonly Factory _factory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemBuilderFinderFactory"/> class.
        /// </summary>
        /// <param name="factory"></param>
        public ItemBuilderFinderFactory(Factory factory)
        {
            this._factory = factory;
        }

        #endregion

        #region delegates

        /// <summary>
        /// Delegate for Autofac.
        /// </summary>
        /// <param name="semanticModel"></param>
        /// <returns>An instance of <see cref="IItemBuilderFinder"/>.</returns>
        public delegate IItemBuilderFinder Factory(SemanticModel semanticModel);

        #endregion

        #region members

        /// <inheritdoc/>
        public IItemBuilderFinder Create(SemanticModel semanticModel) =>
            this._factory(semanticModel);

        #endregion
    }
}