using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Shared.Infrastructure;
using Twizzar.Design.Shared.Infrastructure.Extension;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <inheritdoc cref="IRoslynConfigFinder" />
    public class RoslynConfigFinder : IRoslynConfigFinder
    {
        #region static fields and constants

        private static readonly string
            ItemConfigFullName =
                $"{ApiNames.ApiNamespace}.{ApiNames.ItemBuilderBase}";

        #endregion

        #region members

        /// <inheritdoc />
        public Maybe<IBuilderInformation> FindConfigClass(IViSpan span, IRoslynContext context) =>
            from objectCreation in FindObjectCreation(context, span)
            from t in FindConfigurationTypeSymbol(context, objectCreation)
            from codeBehindClass in FindCodeBehindClass(t.ConfigClassSymbol.DeclaringSyntaxReferences)
            select (IBuilderInformation)new BuilderInformation(
                t.ConfigClassSymbol,
                t.ItemConfigSymbol,
                codeBehindClass,
                objectCreation);

        private static Maybe<ClassDeclarationSyntax> FindCodeBehindClass(
            IEnumerable<SyntaxReference> syntaxReferences) =>
            FindCodeBehindClasses(syntaxReferences).FirstOrNone();

        private static IEnumerable<ClassDeclarationSyntax> FindCodeBehindClasses(
            IEnumerable<SyntaxReference> syntaxReferences) =>
            from syntaxReference in syntaxReferences
            let syntaxNode = syntaxReference.GetSyntax()
            where syntaxNode is ClassDeclarationSyntax
            let classNode = (ClassDeclarationSyntax)syntaxNode
            select classNode;

        private static Maybe<ObjectCreationExpressionSyntax> FindObjectCreation(IRoslynContext context, IViSpan span) =>
            context.RootNode.FindNode(new TextSpan(span.Start, 1))
                .DescendantNodesAndSelf()
                .OfType<ObjectCreationExpressionSyntax>()
                .FirstOrNone();

        private static Maybe<(ITypeSymbol ConfigClassSymbol, INamedTypeSymbol ItemConfigSymbol)>
            FindConfigurationTypeSymbol(IRoslynContext context, ObjectCreationExpressionSyntax objectCreation) =>
            FindConfigurationTypeSymbols(context, objectCreation)
                .FirstOrNone();

        private static IEnumerable<(ITypeSymbol ConfigClassSymbol, INamedTypeSymbol ItemConfigSymbol)>
            FindConfigurationTypeSymbols(
                IRoslynContext context,
                SyntaxNode invocationNode) =>
            from node in invocationNode.DescendantNodesAndSelf()
            let symbol = context.SemanticModel.GetTypeInfo(node)
            let itemConfigSymbol = context.Compilation.GetTypeByMetadataName(ItemConfigFullName)
            where itemConfigSymbol != null
            where symbol.Type != null
            let configItem = FindItemConfig(symbol.Type)
            where configItem.IsSome
            select (symbol.Type, configItem.GetValueUnsafe());

        /// <summary>
        /// Find the ItemConfig base class recursively also if it is defined in the base class of the base class.
        /// If there no ItemConfig class is found return None;
        /// else return the first TypeArgument which is the FixtureItem type.
        /// </summary>
        /// <param name="configSymbol">The symbol of the config class.</param>
        /// <returns>The <see cref="ITypeSymbol"/> of the FixtureItem.</returns>
        private static Maybe<INamedTypeSymbol> FindItemConfig(ITypeSymbol configSymbol) =>
            configSymbol.GetBaseTypesAndThis()
                .OfType<INamedTypeSymbol>()
                .FirstOrNone(symbol => symbol.MetadataName.StartsWith(ApiNames.ItemBuilderT2Name));

        #endregion
    }
}