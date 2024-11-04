using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.Infrastructure.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.Shared.Infrastructure.VisualStudio2022.Extension;

/// <summary>
/// Extension methods for roslyn descriptions.
/// </summary>
public static class RoslynDescriptionExtensions
{
    /// <summary>
    /// Get the underlying symbol.
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    public static ISymbol GetSymbol(this IBaseDescription description) =>
        ((IRoslynBaseDescription)description).GetSymbol();
}