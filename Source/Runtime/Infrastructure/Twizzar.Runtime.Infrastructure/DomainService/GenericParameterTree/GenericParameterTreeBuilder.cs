using System;
using System.Collections.Generic;
using System.Linq;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Runtime.Infrastructure.DomainService.GenericParameterTree;

/// <summary>
/// The Type List[Tuple[int, float]] can be stored in a tree which looks like this:
/// <code>
///  List
///   |
///  Tuple
///    ⋀
/// int float
/// </code>
/// This helper class creates such a tree form two types.
/// The first type is a unbound type for example List[List[T]]
/// and the second one is a bound type for example List[List[int]].
/// The tree created tries to match all nodes of the unbound type with the nodes of the bound type:
/// <code>
/// List -> List
/// List -> List
/// T -> int
/// </code>
/// </summary>
public static class GenericParameterTreeBuilder
{
    /// <summary>
    /// Build a tree form a unbound type and a bound type.
    /// </summary>
    /// <param name="unboundType"></param>
    /// <param name="boundType"></param>
    /// <returns></returns>
    public static Result<IGenericTypeNode, Failure> Build(Type unboundType, Type boundType)
    {
        if (unboundType.IsGenericParameter)
        {
            return new GenericParameterNode(Maybe.None(), unboundType, boundType);
        }
        else
        {
            var unboundTypeDefinition = unboundType.GetGenericTypeDefinitionIfPossible();
            if (boundType != typeof(object)
                && !(boundType.IsArray && unboundType.IsArray)
                && unboundTypeDefinition != boundType.GetGenericTypeDefinitionIfPossible())
            {
                var newBoundType = boundType.GetParentTypes()
                    .FirstOrNone(t => t.GetGenericTypeDefinitionIfPossible() == unboundTypeDefinition);

                if (newBoundType.AsMaybeValue() is not SomeValue<Type> someBoundType)
                {
                    return new Failure($"The type {boundType.ToTypeFullName().GetFriendlyCSharpTypeName()} is not assignable to {unboundType.ToTypeFullName().GetFriendlyCSharpTypeName()}");
                }

                boundType = someBoundType.Value;
            }

            var root = new Node(Maybe.None(), unboundType);

            try
            {
                root.Children.AddRange(BuildChildren(unboundType, boundType, root));
            }
            catch (Exception ex)
            {
                return new Failure(
                    $"The type {boundType.ToTypeFullName().GetFriendlyCSharpTypeName()} and the type {unboundType.ToTypeFullName().GetFriendlyCSharpTypeName()} does not match: " +
                    ex.Message);
            }

            return root;
        }
    }

    /// <summary>
    /// DFS with pre order traversal.
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static IEnumerable<IGenericTypeNode> TraverseDepthFirst(this IGenericTypeNode self)
    {
        yield return self;

        foreach (var node in self.Children)
        {
            foreach (var descendants in node.TraverseDepthFirst())
            {
                yield return descendants;
            }
        }
    }

    private static IEnumerable<IGenericTypeNode> BuildChildren(Type unboundType, Type boundType, IGenericTypeNode parent)
    {
        if (boundType == typeof(object))
        {
            foreach (var uGenericParam in unboundType.GetGenericArgumentsAndItemTypes())
            {
                if (uGenericParam.IsGenericParameter)
                {
                    yield return new GenericParameterNode(Maybe.Some(parent), uGenericParam, boundType);
                }
                else
                {
                    var node = new Node(Maybe.Some(parent), uGenericParam);
                    node.Children.AddRange(BuildChildren(uGenericParam, boundType, node));
                    yield return node;
                }
            }
            yield break;
        }

        if (unboundType.GetGenericArgumentsAndItemTypes().Length != boundType.GetGenericArgumentsAndItemTypes().Length)
        {
            throw new InternalException($"The generic arguments count does not match for the type {unboundType.ToTypeFullName().GetFriendlyCSharpTypeName()} to the type {boundType.ToTypeFullName().GetFriendlyCSharpTypeName()}.");
        }

        foreach (var (uGenericParam, bGenericParam) in unboundType.GetGenericArgumentsAndItemTypes().Zip(boundType.GetGenericArgumentsAndItemTypes(), (a, b) => (a, b)))
        {
            if (uGenericParam.IsGenericParameter)
            {
                yield return new GenericParameterNode(Maybe.Some(parent), uGenericParam, bGenericParam);
            }
            else
            {
                var node = new Node(Maybe.Some(parent), uGenericParam);
                node.Children.AddRange(BuildChildren(uGenericParam, bGenericParam, node));
                yield return node;
            }
        }
    }

    private static Type[] GetGenericArgumentsAndItemTypes(this Type type)
    {
        return type switch
        {
            { IsArray: true } when type.GetElementType() is not null =>
                new[] { type.GetElementType() },
            _ => type.GetGenericArguments(),
        };
    }
}