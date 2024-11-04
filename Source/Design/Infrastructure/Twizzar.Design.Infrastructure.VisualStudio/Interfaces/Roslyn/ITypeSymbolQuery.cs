using Microsoft.CodeAnalysis;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn
{
    /// <summary>
    /// Service for getting <see cref="ITypeSymbol"/>.
    /// </summary>
    public interface ITypeSymbolQuery
    {
        /// <summary>
        /// Get a type symbol by a full name.
        /// </summary>
        /// <param name="compilation">The compilation to be searched.</param>
        /// <param name="typeFullName">The type full name.</param>
        /// <returns>Success if the type was found; else failure.</returns>
        IResult<ITypeSymbol, Failure> GetTypeSymbol(Compilation compilation, ITypeFullName typeFullName);
    }
}