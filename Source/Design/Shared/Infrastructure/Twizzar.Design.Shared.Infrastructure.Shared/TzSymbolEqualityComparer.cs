using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

using Twizzar.Design.Shared.Infrastructure.Extension;
using Twizzar.Design.Shared.Infrastructure.PathTree;
using Twizzar.SharedKernel.CoreInterfaces.Util;

using ViCommon.Functional.Monads.MaybeMonad;

using static ViCommon.Functional.Monads.MaybeMonad.Maybe;

namespace Twizzar.Analyzer.App.SourceTextGenerators
{
    /// <summary>
    /// Equality comparer used for comparing <see cref="ISymbol"/> for caching in the IncrementalGenerator.
    /// </summary>
    public class TzSymbolEqualityComparer : IEqualityComparer<ISymbol>
    {
        #region fields

        private readonly Maybe<IPathNode> _rootNode;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="TzSymbolEqualityComparer"/> class.
        /// </summary>
        /// <param name="rootNode"></param>
        public TzSymbolEqualityComparer(Maybe<IPathNode> rootNode)
        {
            this._rootNode = rootNode;
        }

        #endregion

        #region members

        /// <inheritdoc/>
        public bool Equals(ISymbol x, ISymbol y)
        {
            if (x?.MetadataName != y?.MetadataName)
            {
                return false;
            }

            return x switch
            {
                INamedTypeSymbol t1 => this.Equals(t1, y),
                ITypeSymbol t1 => this.Equals(t1, y),
                _ => SymbolEqualityComparer.Default.Equals(x, y),
            };
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1024:Compare symbols correctly", Justification = "Analyzer is buggy.")]
        public int GetHashCode(ISymbol obj) =>
            HashEqualityComparer.CombineHashes(
                this.GetHashCodes(obj, None(), new HashSet<ISymbol>(SymbolEqualityComparer.Default)));

        private IEnumerable<int> GetMemberHashCodes(
            IPropertySymbol symbol,
            Maybe<IPathNode> currentNode,
            HashSet<ISymbol> visitedSymbols)
        {
            yield return symbol.Name.GetHashCode();
            yield return symbol.DeclaredAccessibility.GetHashCode();

            foreach (var hasCode in this.GetHashCodes(symbol.Type, currentNode, visitedSymbols))
            {
                yield return hasCode;
            }
        }

        private IEnumerable<int> GetMemberHashCodes(
            IFieldSymbol symbol,
            Maybe<IPathNode> currentNode,
            HashSet<ISymbol> visitedSymbols)
        {
            yield return symbol.Name.GetHashCode();
            yield return symbol.DeclaredAccessibility.GetHashCode();

            foreach (var hasCode in this.GetHashCodes(symbol.Type, currentNode, visitedSymbols))
            {
                yield return hasCode;
            }
        }

        private IEnumerable<int> GetMemberHashCodes(
            IMethodSymbol symbol,
            Maybe<IPathNode> currentNode,
            HashSet<ISymbol> visitedSymbols)
        {
            yield return symbol.Name.GetHashCode();
            yield return symbol.DeclaredAccessibility.GetHashCode();

            foreach (var hashCode in symbol.Parameters
                         .SelectMany(symbolParameter => this.GetHashCodes(symbolParameter, currentNode, visitedSymbols)))
            {
                yield return hashCode;
            }

            foreach (var hasCode in this.GetHashCodes(symbol.ReturnType, currentNode, visitedSymbols))
            {
                yield return hasCode;
            }
        }

        private IEnumerable<int> GetMemberHashCodes(
            IParameterSymbol symbol,
            Maybe<IPathNode> currentNode,
            HashSet<ISymbol> visitedSymbols)
        {
            yield return symbol.Name.GetHashCode();
            yield return symbol.DeclaredAccessibility.GetHashCode();

            foreach (var hasCode in this.GetHashCodes(symbol.Type, currentNode, visitedSymbols))
            {
                yield return hasCode;
            }
        }

        private IEnumerable<int> GetHashCodes(
            ISymbol obj,
            Maybe<IPathNode> currentNode,
            HashSet<ISymbol> visitedSymbols) =>
            obj switch
            {
                IPropertySymbol x => this.GetMemberHashCodes(x, currentNode, visitedSymbols),
                IFieldSymbol x => this.GetMemberHashCodes(x, currentNode, visitedSymbols),
                IMethodSymbol x => this.GetMemberHashCodes(x, currentNode, visitedSymbols),
                IParameterSymbol x => this.GetMemberHashCodes(x, currentNode, visitedSymbols),
                ITypeSymbol x => this.GetHashCodes(x, currentNode, visitedSymbols),
                { } x => new[]
                {
                    x.MetadataName.GetHashCode(),
                    x.DeclaredAccessibility.GetHashCode(),
                    x.Kind.GetHashCode(),
                },
                _ => Enumerable.Empty<int>(),
            };

        private IEnumerable<int> GetHashCodes(
            ITypeSymbol typeSymbol,
            Maybe<IPathNode> currentNode,
            HashSet<ISymbol> visitedSymbols)
        {
            if (typeSymbol == null)
            {
                yield return 0;
                yield break;
            }

            if (currentNode.IsNone && visitedSymbols.Contains(typeSymbol))
            {
                yield return 0;
                yield break;
            }

            visitedSymbols.Add(typeSymbol);

            // return a distinguished value for 'object' so we can return the same value for 'dynamic'.
            // That's because the hash code ignores the distinction between dynamic and object.  It also
            // ignores custom modifiers.
            if (typeSymbol.SpecialType == SpecialType.System_Object)
            {
                yield return (int)SpecialType.System_Object;
                yield break;
            }

            var original = typeSymbol.OriginalDefinition;
            yield return original.MetadataName.GetHashCode();
            yield return original.GetTypeFullName().FullName.GetHashCode();

            if (typeSymbol is INamedTypeSymbol { TypeArguments.Length: > 0 } namedTypeSymbol)
            {
                foreach (var hashCode in namedTypeSymbol.DescendantTypeArguments()
                             .SelectMany(symbol => this.GetHashCodes(symbol, None(), visitedSymbols)))
                {
                    yield return hashCode;
                }
            }

            foreach (var hashCode in this.GetHashCodes((ISymbol)original.BaseType, None(), visitedSymbols))
            {
                yield return hashCode;
            }

            foreach (var interfaceHash in original.AllInterfaces
                         .SelectMany(symbol => this.GetHashCodes((ISymbol)symbol, None(), visitedSymbols))
                         .OrderBy(i => i))
            {
                yield return interfaceHash;
            }

            if (currentNode.AsMaybeValue() is SomeValue<IPathNode> someNode)
            {
                var node = someNode.Value;

                foreach (var memberHash in original.GetTwizzarRelevantMembers()
                             .Where(symbol => node.Children.ContainsKey(symbol.GetUniqueName()) || node.Children.ContainsKey(symbol.Name))
                             .SelectMany(
                                 symbol => this.GetHashCodes(
                                     symbol,
                                     node.Children.GetMaybe(symbol.GetUniqueName())
                                         .BindNone(() => node.Children.GetMaybe(symbol.Name)),
                                     visitedSymbols)))
                {
                    yield return memberHash;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1024:Compare symbols correctly", Justification = "Analyzer is buggy.")]
        private bool CompareByHash(ITypeSymbol t1, ITypeSymbol other)
        {
            using var en1 = this.GetHashCodes(
                    t1,
                    this._rootNode,
                    new HashSet<ISymbol>(SymbolEqualityComparer.Default))
                .GetEnumerator();

            using var en2 = this.GetHashCodes(
                    other,
                    this._rootNode,
                    new HashSet<ISymbol>(SymbolEqualityComparer.Default))
                .GetEnumerator();

            while (en1.MoveNext() && en2.MoveNext())
            {
                if (en1.Current != en2.Current)
                {
                    return false;
                }
            }

            // check if both sequences have the both size.
            return !en1.MoveNext() && !en2.MoveNext();
        }

        private bool Equals(ITypeSymbol t1, ISymbol t2) =>
            t2 switch
            {
                ITypeSymbol other => this.CompareByHash(t1, other),
                _ => false,
            };

        /// <summary>
        /// Compares this type to another type.
        /// </summary>
        private bool Equals(INamedTypeSymbol t1, ISymbol t2)
        {
            if (t2 is not INamedTypeSymbol other)
            {
                return false;
            }

            if (t1.IsUnboundGenericType != other.IsUnboundGenericType)
            {
                return false;
            }

            if (t1.SpecialType != other.SpecialType)
            {
                return false;
            }

            return this.CompareByHash(t1, other);
        }

        #endregion
    }
}