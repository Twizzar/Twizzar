using Microsoft.CodeAnalysis;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.Shared.Infrastructure.Interfaces
{
    /// <summary>
    /// Interface for roslyn type description for infrastructure project.
    /// </summary>
    public interface IRoslynTypeDescription : ITypeDescription
    {
        /// <summary>
        /// Gets the underlying type symbol to a corresponding type description.
        /// </summary>
        /// <returns>The instance of <see cref="ITypeSymbol"/>.</returns>
        ITypeSymbol GetTypeSymbol();
    }
}