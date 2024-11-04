using System;
using System.Collections.Generic;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.Infrastructure.DomainService.GenericParameterTree;

/// <summary>
/// Represents a generic parameter to a concrete type form the provided value mapping.
/// </summary>
/// <param name="Parent"></param>
/// <param name="UnboundType"></param>
/// <param name="ValueType"></param>
public record GenericParameterNode(Maybe<IGenericTypeNode> Parent, Type UnboundType, Type ValueType) : IGenericTypeNode
{
    /// <summary>
    /// Gets the child notes. This is a leaf node and has no children.
    /// </summary>
    public IList<IGenericTypeNode> Children => new List<IGenericTypeNode>();
}