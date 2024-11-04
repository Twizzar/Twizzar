using System;
using System.Collections.Generic;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.Infrastructure.DomainService.GenericParameterTree;

/// <summary>
/// Represents a normal node in the tree which is not a generic parameter.
/// </summary>
public class Node : IGenericTypeNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Node"/> class.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="uboundType"></param>
    public Node(Maybe<IGenericTypeNode> parent, Type uboundType)
    {
        this.Parent = parent;
        this.UboundType = uboundType;
    }

    /// <inheritdoc />
    public Maybe<IGenericTypeNode> Parent { get; init; }

    /// <inheritdoc />
    public IList<IGenericTypeNode> Children { get; } = new List<IGenericTypeNode>();

    /// <summary>
    /// Gets the type of this node.
    /// </summary>
    public Type UboundType { get; init; }
}