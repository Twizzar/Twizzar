using Microsoft.CodeAnalysis;

using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Analyzer2022.App.Interfaces;

/// <summary>
/// Service for <see cref="ISymbol"/> specific methods.
/// </summary>
public interface ISymbolService
{
    /// <summary>
    /// Check if a member return type is accessible withing an other symbol.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <param name="memberDescription">The description of the member to check..</param>
    /// <param name="withing">The symbol it should be accessible to.</param>
    /// <returns>True if accessible; else false.</returns>
    bool IsSymbolAccessibleWithin(Compilation compilation, IBaseDescription memberDescription, ISymbol withing);
}