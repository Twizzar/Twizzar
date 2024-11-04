using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;
using Twizzar.Design.Shared.Infrastructure.Discovery;
using Twizzar.Design.Shared.Infrastructure.Shared.Extension;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Shared.Infrastructure.PathTree
{
    /// <summary>
    /// Path tree builder class to comfortably build path tree structures.
    /// </summary>
    public static class PathTreeBuilder
    {
        #region fields

        private static readonly string[] TypeChangingConfigurationMethodNames =
        {
            "InstanceOf",
            "Stub",
        };

        #endregion

        #region members

        /// <summary>
        /// Construct the root node of the path tree.
        /// </summary>
        /// <param name="semanticModel"></param>
        /// <param name="identifierNames"></param>
        /// <returns>The root node.</returns>
        public static IPathNode ConstructRootNode(
            SemanticModel semanticModel,
            params IdentifierNameSyntax[] identifierNames) =>
            ConstructRootNode(identifierNames, semanticModel);

        /// <summary>
        /// Construct the root node of the path tree.
        /// </summary>
        /// <param name="identifierNames"></param>
        /// <param name="semanticModel"></param>
        /// <returns>The root node.</returns>
        public static IPathNode ConstructRootNode(
            IEnumerable<IdentifierNameSyntax> identifierNames,
            SemanticModel semanticModel) =>
            PathNode.ConstructRoot(
                identifierNames.Select(syntax => FindMaybePathSegments(syntax, semanticModel)));

        /// <summary>
        /// Construct the root node of the path tree.
        /// </summary>
        /// <param name="tuples"></param>
        /// <returns>The root node.</returns>
        public static IPathNode ConstructRootNode(
            IEnumerable<(IdentifierNameSyntax IdentifierNameSyntax, SemanticModel SemanticModel)> tuples) =>
            PathNode.ConstructRoot(
                tuples.Select(t => FindMaybePathSegments(t.IdentifierNameSyntax, t.SemanticModel)));

        /// <summary>
        /// Construct the root node of the path tree.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns>The root node.</returns>
        public static IValuesOperation<(PathProviderInformation PathProviderInformation, IPathNode RootNode)> ConstructRootNodes(
            IValuesOperation<KeyValuePair<PathProviderInformation, ImmutableArray<IdentifierNameSyntax>>> operation) =>
            operation
                .Select(
                    (pair, _) =>
                    {
                        var a = pair.Value.Select(syntax => FindMaybePathSegments(syntax, pair.Key.SemanticModel));
                        var root = PathNode.ConstructRoot(a);
                        return (pair.Key, root);
                    });

        private static IEnumerable<(string MemberName, Maybe<ITypeSymbol> TypeSymbol, Maybe<InvocationExpressionSyntax> InvocationExpression)>
            FindMaybePathSegments(
                IdentifierNameSyntax identifierName,
                SemanticModel semanticModel) =>
            /*
             * If we have the expression With(A.B.Stub<T>()) then the syntax tree will look like this:
             * InvocationExpression: With(A.B.Stub<T>())
             * ├─ MemberExpression: A.B.Stub<T>()
             * │  ├─ IdentifierName: B
             * │  └─ MemberExpression: A.B
             * │     ├─ MemberExpression: A
             * │     └─ IdentifierName: A *
             * └─ ArgumentList ()
             *
             * Where the * marks the identifierName. Only the Method hast a ArgumentList as a sibling.
             */
            (
                from memberAccess in identifierName.Ancestors().OfType<MemberAccessExpressionSyntax>()
                let symbol = GetMemberSymbol(memberAccess, semanticModel)
                let invocationExpression = FindInvocationExpression(memberAccess)
                select (memberAccess, symbol, invocationExpression))
            .TakeWhile(t => !IsMethodInvocation(t.memberAccess))
            .Select(t => (t.memberAccess.Name.Identifier.Text, t.symbol, t.invocationExpression));

        /// <summary>
        /// Check if the syntax node has a ArgumentList as a Sibling. The we assume this node is a Method invocation.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>True if the node is a method invocation.</returns>
        private static bool IsMethodInvocation(SyntaxNode node) =>
            node
                .Siblings()
                .OfType<ArgumentListSyntax>()
                .FirstOrNone()
                .IsSome;

        /// <summary>
        /// Find the generic names: InstanceOf or Stub and get the generic symbol.
        /// </summary>
        /// <param name="expressionSyntax"></param>
        /// <param name="semanticModel"></param>
        /// <returns>The symbol of the generic argument of the InstanceOf or Stub method.</returns>
        private static Maybe<ITypeSymbol> GetMemberSymbol(ExpressionSyntax expressionSyntax, SemanticModel semanticModel) =>
            Maybe.ToMaybe(expressionSyntax.Parent)
                .Bind(FindStubOrInstanceOf)
                .Bind(genericNameSyntax => genericNameSyntax.TypeArgumentList.Arguments.FirstOrNone())
                .Bind(typeSyntax => Maybe.ToMaybe(semanticModel.GetTypeInfo(typeSyntax).Type));

        private static Maybe<InvocationExpressionSyntax> FindInvocationExpression(ExpressionSyntax expressionSyntax) =>
            Maybe.ToMaybe(expressionSyntax.Parent?.Parent)
                .Bind(node => Maybe.ToMaybe(node as InvocationExpressionSyntax));

        private static Maybe<GenericNameSyntax> FindStubOrInstanceOf(SyntaxNode node) =>
            node
                .ChildNodes()
                .OfType<GenericNameSyntax>()
                .Where(
                    syntax => TypeChangingConfigurationMethodNames.Contains(syntax.Identifier.ValueText))
                .FirstOrNone();

        #endregion
    }
}