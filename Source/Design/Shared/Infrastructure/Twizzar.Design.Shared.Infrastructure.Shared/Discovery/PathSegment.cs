using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Shared.Infrastructure.Discovery
{
    /// <summary>
    /// Describes one segment of a path.
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Symbol"></param>
    [ExcludeFromCodeCoverage]
    public record PathSegment(string Name, Maybe<ITypeSymbol> Symbol);
}