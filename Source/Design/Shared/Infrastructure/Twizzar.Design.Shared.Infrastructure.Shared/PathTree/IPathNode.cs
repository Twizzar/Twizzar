using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Shared.Infrastructure.PathTree
{
    /// <summary>
    /// A path node represents a segment of a path and knows his parent path segments and all his children path segments.
    /// </summary>
    public interface IPathNode : IEquatable<PathNode>
    {
        /// <summary>
        /// Gets the children of this node.
        /// </summary>
        IImmutableDictionary<string, IPathNode> Children { get; }

        /// <summary>
        /// Gets the type symbol. If the type differs for the member type this will be some else none.
        /// </summary>
        public Maybe<ITypeSymbol> TypeSymbol { get; }

        /// <summary>
        /// Gets the invocation syntax which only is some in a leaf node. Which describes the value configuration.
        /// </summary>
        Maybe<InvocationExpressionSyntax> InvocationSyntax { get; }

        /// <summary>
        /// Gets the parent node if this is the root this will be none.
        /// </summary>
        Maybe<IPathNode> Parent { get; }

        /// <summary>
        /// Gets the member name.
        /// </summary>
        string MemberName { get; }

        /// <summary>
        ///  Gets the max depth of the tree.
        /// </summary>
        int MaxDepth { get; }

        /// <summary>
        /// Gets a child if exists.
        /// </summary>
        /// <param name="memberName">The member name of the child.</param>
        /// <returns><see cref="IPathNode"/> if the child with the member name exists.</returns>
        IPathNode this[string memberName] { get; }

        /// <summary>
        /// Count the duplicates member names the tree up. This will count duplicate as long as the parent is the same.
        /// And returns when they are not.
        /// </summary>
        /// <returns>A count of the duplicates.</returns>
        int CountMemberNameDuplicates();

        /// <summary>
        /// Get all children and all children of the children and so on recursively.
        /// </summary>
        /// <returns>All the descendant of the node.</returns>
        IEnumerable<IPathNode> DescendantNodes();

        /// <summary>
        /// Gets the full path to this item.
        /// </summary>
        /// <param name="rootName"></param>
        /// <returns>.</returns>
        string ConstructPathToRoot(string rootName);
    }
}