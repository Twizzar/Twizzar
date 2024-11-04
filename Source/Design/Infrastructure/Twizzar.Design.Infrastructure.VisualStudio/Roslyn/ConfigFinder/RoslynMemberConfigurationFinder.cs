using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;
using Twizzar.Design.Shared.Infrastructure;
using Twizzar.Design.Shared.Infrastructure.Discovery;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.Design.Shared.Infrastructure.PathTree;
using Twizzar.Design.Shared.Infrastructure.VisualStudio.RoslynPipeline.SimpleOperations;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <inheritdoc cref="IRoslynDescriptionFactory"/>
    public class RoslynMemberConfigurationFinder : IRoslynMemberConfigurationFinder
    {
        #region fields

        private readonly IDiscoverer _discoverer;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynMemberConfigurationFinder"/> class.
        /// </summary>
        /// <param name="discoverer"></param>
        public RoslynMemberConfigurationFinder(IDiscoverer discoverer)
        {
            this._discoverer = discoverer;
        }

        #endregion

        #region members

        /// <inheritdoc />
        public IPathNode FindMemberConfiguration(
            IRoslynContext context,
            SyntaxNode block,
            string pathProviderName,
            Maybe<ObjectCreationExpressionSyntax> objectCreationSyntax,
            CancellationToken cancellationToken)
        {
            var factory = new SimpleOperationFactory(block, context.SemanticModel, cancellationToken);

            var nodes = factory
                .Init(
                    (_, _) => true,
                    (tuple, _) => tuple)
                .Collect()
                .SelectMany(
                    (array, token) =>
                    {
                        return array.SkipWhile(
                            t =>
                                objectCreationSyntax
                                    .Map(start => start != t.Node)
                                    .SomeOrProvided(false));
                    });

            var memberSelections = this._discoverer.DiscoverMemberSelection(nodes);

            var dict = memberSelections
                .Where(tuple => tuple.PathProviderInformation.TypeName == pathProviderName)
                .Where(
                    tuple => objectCreationSyntax.Match(
                        syntax => IsInTheInvocationBlock(tuple.IdentifierNameSyntax, syntax, context),
                        () => true))

                .Collect()
                .Select(
                    (pairs, token) =>
                        pairs.Aggregate(
                            new DefaultDictionary<PathProviderInformation, ImmutableArray<IdentifierNameSyntax>>(
                                _ => ImmutableArray.Create<IdentifierNameSyntax>()),
                            (dict, t) =>
                            {
                                token.ThrowIfCancellationRequested();
                                dict[t.PathProviderInformation] = dict[t.PathProviderInformation].Add(t.IdentifierNameSyntax);
                                return dict;
                            }))
                .SelectMany((dictionary, _) => dictionary);

            return PathTreeBuilder.ConstructRootNodes(dict).ToSimpleOperation().Evaluate().FirstOrNone()
                .Map(tuple => tuple.RootNode)
                .SomeOrProvided(() => PathNode.ConstructRoot(Enumerable.Empty<IEnumerable<string>>()));
        }

        /// <summary>
        /// Check if the syntax is in the Invocation block. Where the invocation block could look like this:
        /// <code>
        /// new MyCustomBuilder()
        ///     .With(p => p.Member.Value(3))
        ///     .Build();
        /// </code>
        /// The syntax could be the <c>p</c> form <c>p.Member.Value(3)</c>.
        /// </summary>
        /// <param name="syntax"></param>
        /// <param name="objectCreationExpression"></param>
        /// <param name="context"></param>
        /// <returns>True if the syntax is in the invocation block; else false.</returns>
        private static bool IsInTheInvocationBlock(
            IdentifierNameSyntax syntax,
            ObjectCreationExpressionSyntax objectCreationExpression,
            IRoslynContext context) =>
                objectCreationExpression
                    .Ancestors()
                    .OfType<ExpressionSyntax>()
                    .Where(e => IsApiMethod(e, context.SemanticModel))
                    .LastOrNone()
                    .Match(
                        parentNode => parentNode.Contains(syntax),
                        () => false);

        private static bool IsApiMethod(ExpressionSyntax expressionSyntax, SemanticModel semanticModel)
        {
            var info = semanticModel.GetSymbolInfo(expressionSyntax);
            var symbol = Maybe.ToMaybe(info.Symbol)
                .BindNone(() => info.CandidateSymbols.FirstOrNone())
                .SomeOrProvided(() => null);

            return symbol is IMethodSymbol methodSymbol &&
                   methodSymbol.ContainingNamespace.ToString() == ApiNames.ApiNamespace;
        }

        #endregion
    }
}