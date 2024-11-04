using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn
{
    /// <summary>
    /// Interface for wrapping the <see cref="SymbolFinder"/>.
    /// </summary>
    public interface ISymbolFinder
    {
        /// <summary>
        /// Finds the accessible class or struct types that implement the given interface.
        /// Finds all the derived classes of the given type.
        /// Finds the derived interfaces of the given interfaces.
        /// </summary>
        /// <param name="typeSymbol"><see cref="INamespaceSymbol"/>.</param>
        /// <param name="solution">The current <see cref="Solution"/>.</param>
        /// <returns>The Implementations and the derived types of the given symbol.</returns>
        Task<IEnumerable<ITypeSymbol>> FindImplementationsAndDerivedTypesAsync(ITypeSymbol typeSymbol, Solution solution);

        /// <summary>
        /// Finds the accessible class or struct types that implement the given interface.
        /// </summary>
        /// <param name="typeSymbol"><see cref="INamespaceSymbol"/>.</param>
        /// <param name="solution">The current <see cref="Solution"/>.</param>
        /// <returns>The accessible class or struct types that implement the given interface.</returns>
        Task<IEnumerable<ITypeSymbol>> FindImplementationsAsync(ITypeSymbol typeSymbol, Solution solution);

        /// <summary>
        /// Finds all the derived classes of the given type. Implementations of an interface are not considered,
        /// but can be found with FindImplementationsAsync.
        /// </summary>
        /// <param name="typeSymbol"><see cref="INamespaceSymbol"/>.</param>
        /// <param name="solution">The current <see cref="Solution"/>.</param>
        /// <returns>All the derived classes of the given type.</returns>
        Task<IEnumerable<ITypeSymbol>> FindDerivedClassesAsync(ITypeSymbol typeSymbol, Solution solution);
    }
}