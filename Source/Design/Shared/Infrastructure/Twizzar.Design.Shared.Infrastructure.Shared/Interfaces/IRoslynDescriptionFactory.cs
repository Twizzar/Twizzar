using Microsoft.CodeAnalysis;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.Shared.Infrastructure.Interfaces
{
    /// <summary>
    /// Factory for a roslyn <see cref="ITypeDescription"/>.
    /// </summary>
    public interface IRoslynDescriptionFactory
    {
        /// <summary>
        /// Creates a new type description.
        /// </summary>
        /// <param name="symbol">The symbol retrieved with roslyn.</param>
        /// <returns>A new <see cref="ITypeDescription"/>.</returns>
        public ITypeDescription CreateDescription(ITypeSymbol symbol);
    }
}