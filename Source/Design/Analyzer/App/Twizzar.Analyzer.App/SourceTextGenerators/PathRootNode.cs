using System.Diagnostics.CodeAnalysis;
using Twizzar.Analyzer.App.SourceTextGenerators;
using Twizzar.Design.Shared.Infrastructure.Discovery;
using Twizzar.Design.Shared.Infrastructure.PathTree;
using Twizzar.SharedKernel.CoreInterfaces.Util;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Analyzer2022.App.SourceTextGenerators;

/// <summary>
/// Contains all Information for generating a Paths class.
/// </summary>
/// <param name="Information"></param>
/// <param name="Root"></param>
[ExcludeFromCodeCoverage]
public sealed record PathRootNode(
    PathProviderInformation Information,
    IPathNode Root)
{
    #region members

    /// <inheritdoc/>
    public bool Equals(PathRootNode other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Root.Equals(other?.Root) &&
               this.Information.Equals(other?.Information, new TzSymbolEqualityComparer(Maybe.Some(this.Root)));
    }

    /// <inheritdoc />
    public override int GetHashCode() =>
        HashEqualityComparer.CombineHashes(
            this.Root.GetHashCode(),
            this.Information.GetHashCode(new TzSymbolEqualityComparer(Maybe.Some(this.Root))));

    #endregion
}