using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Twizzar.Analyzer;
using Twizzar.Analyzer.Core.CompositionRoot;
using Twizzar.Analyzer.Core.Interfaces;
using Twizzar.Analyzer2022.App.IncrementalPipeline;
using Twizzar.Analyzer2022.App.Interfaces;
using Twizzar.Analyzer2022.App.SourceTextGenerators;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;
using Twizzar.Design.Shared.Infrastructure;
using Twizzar.Design.Shared.Infrastructure.Discovery;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.Design.Shared.Infrastructure.PathTree;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.Util;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

using static Twizzar.Analyzer.Core.Diagnostics.ItemConfigPathDiagnostics;

using ItemBuilderCreationArray =
    System.Collections.Immutable.ImmutableArray<(
        Twizzar.Design.Shared.Infrastructure.Discovery.ItemBuilderCreationInformation ItemBuilderCreationInformation,
        Twizzar.Design.Shared.Infrastructure.Discovery.PathProviderInformation PathProviderInformation)>;
using MemberSelectionArray =
    System.Collections.Immutable.ImmutableArray<(Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax
        IdentifierNameSyntax, Microsoft.CodeAnalysis.SemanticModel SemanticModel,
        Twizzar.Design.Shared.Infrastructure.Discovery.PathProviderInformation PathProviderInformation)>;
using PathProviderGroups =
    System.Collections.Immutable.ImmutableArray<System.Collections.Generic.KeyValuePair<
        Twizzar.Design.Shared.Infrastructure.Discovery.PathProviderInformation, System.Collections.Immutable.
        ImmutableArray<(Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax IdentiferName,
            Microsoft.CodeAnalysis.SemanticModel SemanticModel)>>>;

#pragma warning disable SA1118 // Parameter should not span multiple lines

namespace Twizzar.Analyzer2022.App
{
    /// <summary>
    /// IncrementalGenerator entry point.
    /// </summary>
    [Generator]
    public class ItemConfigPathSourceGenerator : IIncrementalGenerator
    {
        #region members

        /// <inheritdoc/>
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            try
            {
                var iocOrchestrator = new IocOrchestrator();
                var pathSourceTextGenerator = iocOrchestrator.Resolve<IPathSourceTextGenerator>();

                var builderExtensionMethodSourceTextGenerator =
                    iocOrchestrator.Resolve<IBuilderExtensionMethodSourceTextGenerator>();

                var discoverer = iocOrchestrator.Resolve<IDiscoverer>();

                var stepFactory = new IncrementalOperationFactory(context);

                var allOperation = stepFactory.Init(
                    (_, _) => true,
                    (tuple, _) => tuple);

                var objectCreationsOperation = stepFactory.Init(
                    (node, _) => node is ObjectCreationExpressionSyntax objectCreationExpression &&
                                 objectCreationExpression.Type.ToString().StartsWith(ApiNames.ItemBuilderName),
                    (tuple, _) => tuple);

                var classDeclarationOperation = stepFactory.Init(
                    (node, _) => node is ClassDeclarationSyntax,
                    (tuple, _) => tuple);

                var itemBuilderCreations = discoverer.DiscoverItemBuilderCreation(objectCreationsOperation);
                var discoverCustomItemBuilder = discoverer.DiscoverCustomItemBuilder(classDeclarationOperation);
                var memberSelections = discoverer.DiscoverMemberSelection(allOperation);

                var pathSelections = discoverCustomItemBuilder.Collect()
                    .Combine(memberSelections.Collect())
                    .Combine(itemBuilderCreations.Collect())
                    .Select(
                        (tuple, _) => (pathSelections: tuple.Left.Left, memberSelections: tuple.Left.Right,
                            itemBuilderCreations: tuple.Right))
                    .SelectMany(this.GroupByPathProvider);

                var pathRoot = pathSelections.Select(
                        (pair, _) =>
                        {
                            try
                            {
                                var root = PathTreeBuilder.ConstructRootNode(pair.Value);
                                return Maybe.Some(new PathRootNode(pair.Key, root));
                            }
                            catch (Exception ex)
                            {
                                this.Log(ex);
                                return Maybe.None<PathRootNode>();
                            }
                        })
                    .Somes();

                var sourceTexts = pathRoot
                    .Select((tuple, token) => GeneratePaths(tuple, pathSourceTextGenerator, token));

                context.RegisterSourceOutput(
                    sourceTexts.ToIncrementalValuesProvider(),
                    this.AddPathSource);

                var itemBuilderCreationInformationEqualityComparer =
                    new HashEqualityComparer<ItemBuilderCreationInformation>(
                        information => information.FixtureItemFullName.GetHashCode());

                context.RegisterSourceOutput(
                    itemBuilderCreations
                        .Select((tuple, _) => tuple.ItemBuilderCreationInformation)
                        .Collect()
                        .ToIncrementalValueProvider(),
                    (productionContext, itemBuilderCreationInformation) =>
                        this.AddExtensionMethodsClass(
                            productionContext,
                            itemBuilderCreationInformation
                                .ToImmutableHashSet(itemBuilderCreationInformationEqualityComparer),
                            builderExtensionMethodSourceTextGenerator));
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
            catch (Exception exp)
            {
                this.HandleException(exp);
            }
        }

        private void AddPathSource(
            SourceProductionContext productionContext,
            IResult<(string HintName, SourceText SourceText), PathGenerationFailure> tuple)
        {
            try
            {
                tuple.Do(
                    t =>
                    {
                        if (t.HintName is null || t.SourceText is null)
                        {
                            return;
                        }

                        productionContext.CancellationToken.ThrowIfCancellationRequested();

                        productionContext.AddSource(
                            t.HintName,
                            t.SourceText);
                    },
                    f =>
                    {
                        if (f.Diagnostic is null)
                        {
                            return;
                        }

                        this.Log(f.Message, LogLevel.Error);
                        productionContext.ReportDiagnostic(Diagnostic.Create(
                            f.Diagnostic,
                            f.FixtureItemSymbol?.Locations.FirstOrDefault(),
                            f.Message));
                    });
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
            catch (Exception exp)
            {
                this.HandleException(exp);

                productionContext.ReportDiagnostic(Diagnostic.Create(
                    UnexpectedDiagnosticDescriptor,
                    null,
                    exp.Message));
            }
        }

        private PathProviderGroups GroupByPathProvider(
            (ImmutableArray<PathProviderInformation> PathSelections, MemberSelectionArray MemberSelections,
                ItemBuilderCreationArray ItemBuilderCreations) tuple,
            CancellationToken token)
        {
            var dict =
                new Dictionary<PathProviderInformation, ImmutableArray<(IdentifierNameSyntax, SemanticModel)>>(
                    PathProviderInformation.NameSpaceTypeNameComparer);

            try
            {
                foreach (var information in tuple.ItemBuilderCreations
                             .Select(valueTuple => valueTuple.PathProviderInformation)
                             .Concat(tuple.PathSelections))
                {
                    token.ThrowIfCancellationRequested();

                    dict.AddIfNotExists(
                        information,
                        ImmutableArray.Create<(IdentifierNameSyntax, SemanticModel)>());
                }

                foreach (var (identifierNameSyntax, semanticModel, pathProviderInformation) in tuple.MemberSelections)
                {
                    token.ThrowIfCancellationRequested();

                    if (dict.ContainsKey(pathProviderInformation))
                    {
                        dict[pathProviderInformation] = dict[pathProviderInformation]
                            .Add((identifierNameSyntax, semanticModel));
                    }
                    else
                    {
                        dict.Add(
                            pathProviderInformation,
                            ImmutableArray.Create<(IdentifierNameSyntax, SemanticModel)>()
                                .Add((identifierNameSyntax, semanticModel)));
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // this need to be thrown to cancel the operation.
                throw;
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }

            return dict.ToImmutableArray();
        }

        private static IResult<(string HintName, SourceText SourceText), PathGenerationFailure> GeneratePaths(
            PathRootNode rootNode,
            IPathSourceTextGenerator pathSourceTextGenerator,
            CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var sourceTextResult = pathSourceTextGenerator.GenerateSourceText(
                    rootNode.Information.TypeName,
                    rootNode.Information.NameSpace,
                    rootNode.Information.FixtureItemSymbol,
                    rootNode.Root,
                    true,
                    rootNode.Information.SemanticModel.Compilation,
                    rootNode.Information.SourceType,
                    token);

                return sourceTextResult.MapSuccess(sourceText =>
                    (HintName: rootNode.Information.TypeName + rootNode.Information.NameSpace.GetHashCode(), sourceText));
            }
            catch (OperationCanceledException)
            {
                return Result.Failure<(string HintName, SourceText SourceText), PathGenerationFailure>(
                    new PathGenerationFailure(null, null, null));
            }
            catch (Exception ex)
            {
                return Result.Failure<(string HintName, SourceText SourceText), PathGenerationFailure>(
                    new PathGenerationFailure(ex.ToString(), null, UnexpectedDiagnosticDescriptor));
            }
        }

        private void AddExtensionMethodsClass(
            SourceProductionContext context,
            IImmutableSet<ItemBuilderCreationInformation> itemBuilderCreations,
            IBuilderExtensionMethodSourceTextGenerator builderExtensionMethodSourceTextGenerator)
        {
            try
            {
                var result = builderExtensionMethodSourceTextGenerator.GenerateClass(
                    itemBuilderCreations,
                    context.CancellationToken);

                result.Do(
                    code =>
                    {
                        const string configFilePath = "ItemBuilderExtensionMethods.cs";
                        var node = SyntaxFactory.ParseCompilationUnit(code).NormalizeWhitespace();
                        context.AddSource(configFilePath, node.GetText(Encoding.UTF8));
                    },
                    failure =>
                    {
                        switch (failure)
                        {
                            case OperationCanceledFailure _:
                                break;
                            case { } f:
                                this.Log(f.Message);
                                break;
                        }
                    });
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
        }

        private void HandleException(Exception exp)
        {
            this.Log(exp);

#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
                Debugger.Break();
            }
#endif
        }

        #endregion
    }
}