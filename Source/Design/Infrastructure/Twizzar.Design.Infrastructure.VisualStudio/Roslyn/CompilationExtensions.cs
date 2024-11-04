using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Twizzar.Design.Infrastructure.VisualStudio2019.Roslyn;

/// <summary>
/// Extension methods for the <see cref="Compilation"/>.
/// </summary>
public static class CompilationExtensions
{
    /// <summary>
    /// Get all types which are assignable to an object for a compilation.
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IEnumerable<INamedTypeSymbol> GetAllTypes(this Compilation compilation, CancellationToken cancellationToken = default) =>
        GetAllTypes(compilation.GlobalNamespace, cancellationToken);

    private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol @namespace, CancellationToken cancellationToken)
    {
        foreach (var namedTypeSymbol in @namespace.GetTzTypeMembers()
                     .SelectMany(symbol => GetNestedTypes(symbol, cancellationToken))
                     .Concat(@namespace.GetNamespaceMembers()
                         .SelectMany(symbol => GetAllTypes(symbol, cancellationToken))))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return namedTypeSymbol;
        }
    }

    private static IEnumerable<INamedTypeSymbol> GetTzTypeMembers(
        this INamespaceOrTypeSymbol symbol) =>
        symbol.GetTypeMembers()
            .Where(typeSymbol =>
                typeSymbol.TypeKind is TypeKind.Class or TypeKind.Interface or TypeKind.Enum or TypeKind.Struct)
            .Where(typeSymbol =>
                !typeSymbol.IsStatic && typeSymbol.CanBeReferencedByName);

    private static IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol type, CancellationToken cancellationToken)
    {
        yield return type;

        foreach (var nestedType in type.GetTzTypeMembers()
                     .SelectMany(symbol => GetNestedTypes(symbol, cancellationToken)))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return nestedType;
        }
    }
}