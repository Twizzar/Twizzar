using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Twizzar.Analyzer.App.SourceTextGenerators;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.Util;

using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Shared.Infrastructure.PathTree
{
    /// <inheritdoc cref="IPathNode" />
    public sealed class PathNode : IPathNode
    {
        #region ctors

        private PathNode(Maybe<IPathNode> parent, string memberName)
        {
            this.Children = ImmutableDictionary.Create<string, IPathNode>();
            this.Parent = parent;
            this.MemberName = memberName;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public IImmutableDictionary<string, IPathNode> Children { get; private set; }

        /// <inheritdoc />
        public Maybe<ITypeSymbol> TypeSymbol { get; private set; }

        /// <inheritdoc />
        public Maybe<InvocationExpressionSyntax> InvocationSyntax { get; private set; }

        /// <inheritdoc />
        public Maybe<IPathNode> Parent { get; }

        /// <inheritdoc />
        public string MemberName { get; }

        /// <inheritdoc />
        public int MaxDepth { get; private set; }

        /// <inheritdoc />
        public IPathNode this[string memberName] => this.Children[memberName];

        #endregion

        #region members

        /// <summary>
        /// Construct the root node with all his descendants.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns>A new <see cref="IPathNode"/>.</returns>
        public static IPathNode ConstructRoot(
            IEnumerable<IEnumerable<(string MemberName, Maybe<ITypeSymbol> TypeSymbol, Maybe<InvocationExpressionSyntax>
                InvocationExpression)>> paths)
        {
            var root = new PathNode(Maybe.None(), "root");
            root.Children = CreateChildren(paths, root).ToImmutableDictionary(node => node.MemberName);
            root.MaxDepth = root.CalculateMaxDepth();
            return root;
        }

        /// <summary>
        /// Construct the root node with all his descendants.
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A new <see cref="IPathNode"/>.</returns>
        public static IPathNode ConstructRoot(
            ImmutableArray<ImmutableArray<(string MemberName, Maybe<ITypeSymbol> TypeSymbol,
                Maybe<InvocationExpressionSyntax>
                InvocationExpression)>> paths,
            CancellationToken cancellationToken)
        {
            var root = new PathNode(Maybe.None(), "root");

            root.Children = CreateChildren(
                    paths.Select(
                        array =>
                            (IEnumerable<(string, Maybe<ITypeSymbol>, Maybe<InvocationExpressionSyntax>)>)array),
                    root,
                    cancellationToken)
                .ToImmutableDictionary(node => node.MemberName);

            root.MaxDepth = root.CalculateMaxDepth();
            return root;
        }

        /// <summary>
        /// Construct the root node with all his descendants.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns>A new <see cref="IPathNode"/>.</returns>
        public static IPathNode ConstructRoot(IEnumerable<IEnumerable<string>> paths)
        {
            var root = new PathNode(Maybe.None(), "root");

            root.Children = CreateChildren(
                    paths
                        .Select(
                            enumerable => enumerable.Select(
                                s => (s, Maybe.None<ITypeSymbol>(), Maybe.None<InvocationExpressionSyntax>()))),
                    root)
                .ToImmutableDictionary(node => node.MemberName);

            root.MaxDepth = root.CalculateMaxDepth();
            return root;
        }

        /// <inheritdoc />
        public int CountMemberNameDuplicates() =>
            this.Parent
                .Map(
                    parent =>
                        parent.MemberName == this.MemberName
                            ? parent.CountMemberNameDuplicates() + 1
                            : 0)
                .SomeOrProvided(0);

        /// <inheritdoc />
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            this.Children.Values.OfType<PathNode>().OrderBy(node => node.MemberName).ForEach(node => node.Print(sb, 0));
            sb.AppendLine();

            return sb.ToString();
        }

        /// <inheritdoc/>
        public IEnumerable<IPathNode> DescendantNodes()
        {
            return this.Children
                .Select(pair => pair.Value)
                .Concat(this.Children.SelectMany(pair => pair.Value.DescendantNodes()));
        }

        /// <inheritdoc />
        public string ConstructPathToRoot(string rootName)
        {
            if (this.Parent.IsNone)
            {
                return rootName;
            }
            else
            {
                return this.Parent
                           .Map(item => $"{item.ConstructPathToRoot(rootName)}.")
                           .SomeOrProvided(string.Empty) +
                       this.MemberName;
            }
        }

        /// <inheritdoc />
        public bool Equals(PathNode other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.MemberName == other.MemberName &&
                   this.MaxDepth == other.MaxDepth &&
                   this.Children.Count == other.Children.Count &&
                   this.Children.SequenceEqual(other.Children) &&
                   this.SymbolEquals(this.TypeSymbol, other.TypeSymbol);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((PathNode)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Children != null
                    ? HashEqualityComparer.CombineHashes(this.Children.Select(pair => pair.Value.GetHashCode()))
                    : 0;

                hashCode = (hashCode * 397) ^ (this.MemberName != null ? this.MemberName.GetHashCode() : 0);

                hashCode = (hashCode * 397) ^
                           this.TypeSymbol.Map(symbol => new TzSymbolEqualityComparer(this).GetHashCode(symbol))
                               .SomeOrProvided(0);

                return hashCode;
            }
        }

        private int CalculateMaxDepth()
        {
            if (this.Children.Count == 0)
            {
                return 1;
            }
            else
            {
                return this.Children.Select(pair => pair.Value)
                    .OfType<PathNode>()
                    .Select(node => node.MaxDepth + 1)
                    .Max();
            }
        }

        private bool SymbolEquals(Maybe<ITypeSymbol> a, Maybe<ITypeSymbol> b)
        {
            if (a.IsNone && b.IsNone)
            {
                return true;
            }

            return (
                    from t1 in a
                    from t2 in b
                    select new TzSymbolEqualityComparer(this).Equals(t1, t2))
                .SomeOrProvided(false);
        }

        private void Print(StringBuilder sb, int intend)
        {
            sb.AppendLine($"{Tab(intend)}⊢{this.MemberName}");

            foreach (var pathNode in this.Children.Values.OfType<PathNode>())
            {
                pathNode.Print(sb, intend + 2);
            }
        }

        private static string Tab(int intend) =>
            new(Enumerable.Repeat(' ', intend).ToArray());

        private static IEnumerable<IPathNode> CreateChildren(
            IEnumerable<IEnumerable<(string MemberName, Maybe<ITypeSymbol> Symbol, Maybe<InvocationExpressionSyntax>
                InvocationExpression)>> paths,
            IPathNode parent,
            CancellationToken cancellationToken = default)
        {
            foreach (var pathGroup in paths
                         .GroupBy(
                             enumerable =>
                                 enumerable.FirstOrNone().Map(tuple => tuple.MemberName).SomeOrProvided(string.Empty)))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var node = new PathNode(Maybe.Some(parent), pathGroup.Key);

                var children = CreateChildren(
                    pathGroup
                        .Where(enumerable => enumerable.Count() > 1)
                        .Select(enumerable => enumerable.Skip(1)),
                    node,
                    cancellationToken);

                var type = pathGroup
                    .Select(tuples => tuples.FirstOrNone())
                    .Somes()
                    .Select(tuple => tuple.Symbol)
                    .Somes()
                    .LastOrNone();

                var invocationSyntax = pathGroup
                    .Select(tuples => tuples.FirstOrNone())
                    .Somes()
                    .Select(tuple => tuple.InvocationExpression)
                    .Somes()
                    .LastOrNone();

                node.TypeSymbol = type;
                node.Children = children.ToImmutableDictionary(pathNode => pathNode.MemberName);
                node.MaxDepth = node.CalculateMaxDepth();
                node.InvocationSyntax = invocationSyntax;
                yield return node;
            }
        }

        #endregion
    }
}