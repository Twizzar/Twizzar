using System.Collections.Generic;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.Infrastructure.DomainService.GenericParameterTree;

/// <summary>
/// Represents a type from a generic Type.
/// </summary>
public interface IGenericTypeNode
{
    /// <summary>
    /// Gets the parent node if None this node is the root node.
    /// </summary>
    Maybe<IGenericTypeNode> Parent { get; }

    /// <summary>
    /// Gets the child nodes.
    /// </summary>
    IList<IGenericTypeNode> Children { get; }
}