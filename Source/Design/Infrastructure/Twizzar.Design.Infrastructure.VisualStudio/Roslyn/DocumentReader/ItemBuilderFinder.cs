using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Twizzar.Design.Shared.Infrastructure.Extension;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using static Twizzar.Design.Shared.Infrastructure.ApiNames;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn.DocumentReader
{
    /// <inheritdoc cref="IItemBuilderFinder" />
    public class ItemBuilderFinder : IItemBuilderFinder
    {
        #region fields

        private readonly SemanticModel _semanticModel;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemBuilderFinder"/> class.
        /// </summary>
        /// <param name="semanticModel"></param>
        public ItemBuilderFinder(SemanticModel semanticModel)
        {
            EnsureHelper.GetDefault.Parameter(semanticModel, nameof(semanticModel))
                .ThrowWhenNull();

            this._semanticModel = semanticModel;
        }

        #endregion

        #region members

        /// <inheritdoc />
        public IEnumerable<IItemBuilderInformation> FindBuilderInformation(SyntaxNode rootNode)
        {
            foreach (var node in rootNode.DescendantNodesAndSelf().
                         OfType<ObjectCreationExpressionSyntax>())
            {
                // Wen the object creation has no argument parentheses.
                if ((node.ArgumentList?.FullSpan.Length ?? 0) <= 0)
                {
                    continue;
                }

                // get the type symbol
                if (this._semanticModel.GetSymbolInfo(node.Type).Symbol is not INamedTypeSymbol builderSymbol ||
                    builderSymbol.FindBaseSymbol(ItemBuilderBase).AsMaybeValue() is not SomeValue<INamedTypeSymbol>
                        someItemBuilderSymbol)
                {
                    continue;
                }

                var itemBuilderSymbol = someItemBuilderSymbol.Value;
                var builderTypeFullName = builderSymbol.GetTypeFullName();
                var itemBuilderTypeFullName = itemBuilderSymbol.GetTypeFullName();

                // check if the created type has at least one generic argument. And has the right namespace.
                if (itemBuilderSymbol.TypeArguments.Length < 0 &&
                    !itemBuilderTypeFullName.FullName.StartsWith($"{ApiNamespace}."))
                {
                    continue;
                }

                var fixtureItemType = itemBuilderSymbol.TypeArguments[0];

                if (IsUnboundGeneric(fixtureItemType))
                {
                    continue;
                }

                yield return
                    new ItemBuilderInformation(
                        node,
                        fixtureItemType,
                        !builderTypeFullName.FullName.StartsWith($"{ApiNamespace}.{ItemBuilderT1Name}"));
            }
        }

        // Check if the type has an unbound generic. If an type variable is used we cannot figure out all the needed information.
        private static bool IsUnboundGeneric(ITypeSymbol typeSymbol) =>
            typeSymbol.Kind == SymbolKind.TypeParameter ||
            (typeSymbol is INamedTypeSymbol namedType &&
             namedType
                 .DescendantTypeArguments()
                 .Any(symbol => symbol.Kind == SymbolKind.TypeParameter));

        #endregion
    }
}