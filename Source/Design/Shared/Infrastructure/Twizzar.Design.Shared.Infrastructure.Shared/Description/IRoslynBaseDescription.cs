using Microsoft.CodeAnalysis;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.Shared.Infrastructure.Description;

/// <summary>
/// Members for the Roslyn part of the <see cref="IBaseDescription"/>.
/// </summary>
public interface IRoslynBaseDescription
{
    /// <summary>
    /// Get the underlying symbol.
    /// </summary>
    /// <returns></returns>
    public ISymbol GetSymbol();
}