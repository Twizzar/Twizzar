using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn;
using Twizzar.Design.Shared.CoreInterfaces.FixtureItem.Description;
using Twizzar.Design.Shared.Infrastructure.Extension;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.Design.Shared.Infrastructure.PathTree;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

using static ViCommon.Functional.Monads.MaybeMonad.Maybe;
using static ViCommon.Functional.Monads.ResultMonad.Result;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <inheritdoc cref="IRoslynConfigurationItemReader" />
    public sealed class RoslynConfigurationItemReader : IRoslynConfigurationItemReader
    {
        #region fields

        private readonly IConfigurationItemFactory _configurationItemFactory;
        private readonly IRoslynContextQuery _roslynContextQuery;
        private readonly IRoslynDescriptionFactory _typeDescriptionFactory;
        private readonly IRoslynMemberConfigurationFinder _memberConfigurationFinder;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynConfigurationItemReader"/> class.
        /// </summary>
        /// <param name="configurationItemFactory"></param>
        /// <param name="roslynContextQuery"></param>
        /// <param name="memberConfigurationFinder"></param>
        /// <param name="typeDescriptionFactory"></param>
        public RoslynConfigurationItemReader(
            IConfigurationItemFactory configurationItemFactory,
            IRoslynContextQuery roslynContextQuery,
            IRoslynMemberConfigurationFinder memberConfigurationFinder,
            IRoslynDescriptionFactory typeDescriptionFactory)
        {
            this.EnsureMany()
                .Parameter(configurationItemFactory, nameof(configurationItemFactory))
                .Parameter(roslynContextQuery, nameof(roslynContextQuery))
                .Parameter(memberConfigurationFinder, nameof(memberConfigurationFinder))
                .Parameter(typeDescriptionFactory, nameof(typeDescriptionFactory))
                .ThrowWhenNull();

            this._configurationItemFactory = configurationItemFactory;
            this._roslynContextQuery = roslynContextQuery;
            this._memberConfigurationFinder = memberConfigurationFinder;
            this._typeDescriptionFactory = typeDescriptionFactory;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public async Task<IImmutableDictionary<FixtureItemId, IConfigurationItem>> ReadConfigurationItemsAsync(
            IBuilderInformation builderInformation,
            CancellationToken cancellationToken)
        {
            // This dictionary will be updated by FindConfigurationsInClassDeclaration
            var additionalConfigurationItems = new Dictionary<FixtureItemId, IConfigurationItem>();

            var fixtureItemSymbol = builderInformation.ItemBuilderSymbol.TypeArguments.FirstOrNone()
                .SomeOrProvided(() => throw new InternalException("Cannot find the fixture item type."));

            var pathProviderSymbol = builderInformation.ItemBuilderSymbol.TypeArguments.IndexOrNone(1)
                .SomeOrProvided(() => throw new InternalException("Cannot find the pathProvider type."));

            var fixtureItemDescription = this._typeDescriptionFactory.CreateDescription(fixtureItemSymbol);
            var fixtureItemType = fixtureItemDescription.TypeFullName;

            // for all partial classes of the config class.
            foreach (var declaringSyntaxReference in builderInformation.CustomItemBuilderSymbol.DeclaringSyntaxReferences)
            {
                var syntaxNode = await declaringSyntaxReference.GetSyntaxAsync(cancellationToken);

                var ctorBlockInfos =
                    from context in await this._roslynContextQuery
                        .GetContextAsync(declaringSyntaxReference.SyntaxTree.FilePath, cancellationToken)
                        .MapSuccessAsync(
                            c => new Context(
                                c,
                                fixtureItemType.FullName))
                    from ctorBody in syntaxNode.DescendantNodes()
                        .OfType<ConstructorDeclarationSyntax>()
                        .FirstOrNone()
                        .Bind(syntax => ToMaybe(syntax.Body))
                        .ToResult("Constructor Body cannot be found.")
                    let root = this._memberConfigurationFinder.FindMemberConfiguration(
                        context,
                        ctorBody,
                        pathProviderSymbol.Name,
                        None(),
                        cancellationToken)
                    select (root, context);

                ctorBlockInfos.Do(
                    t =>
                        this.CreateAndAppendConfigurationItems(fixtureItemDescription, t.root, true, t.context, additionalConfigurationItems),
                    failure =>
                        this.Log(failure.Message, LogLevel.Debug));
            }

            var utBlockInfos =
                from context in await this._roslynContextQuery
                    .GetContextAsync(builderInformation.ObjectCreationExpression.SyntaxTree.FilePath, cancellationToken)
                    .MapSuccessAsync(
                        roslynContext => new Context(
                            roslynContext,
                            fixtureItemType.FullName))
                let body = builderInformation.ObjectCreationExpression
                    .Ancestors()
                    .FirstOrNone(syntax => syntax is BlockSyntax)
                    .SomeOrProvided(() => builderInformation.ObjectCreationExpression.SyntaxTree.GetRoot())
                let root = this._memberConfigurationFinder.FindMemberConfiguration(
                    context,
                    body,
                    pathProviderSymbol.Name,
                    builderInformation.ObjectCreationExpression,
                    cancellationToken)
                select (context, root);

            utBlockInfos.Do(
                t =>
                    this.CreateAndAppendConfigurationItems(fixtureItemDescription, t.root, false, t.context, additionalConfigurationItems),
                failure =>
                    this.Log(failure.Message, LogLevel.Debug));

            // add the rootConfigurationItem
            return additionalConfigurationItems.ToImmutableDictionary();
        }

        private void CreateAndAppendConfigurationItems(
            ITypeDescription typeDescription,
            IPathNode root,
            bool isInBuilderBlock,
            Context context,
            Dictionary<FixtureItemId, IConfigurationItem> configurationItems)
        {
            var result = this.CreateConfigurationItems(
                    typeDescription,
                    root,
                    isInBuilderBlock,
                    context)
                .ToImmutableDictionary(item => item.Id);

            // Merge configurations
            foreach (var bKey in result.Keys)
            {
                if (configurationItems.ContainsKey(bKey))
                {
                    configurationItems[bKey]
                        .Merge(result[bKey])
                        .Do(
                            item => configurationItems[bKey] = item,
                            failure => this.Log(failure.Message, LogLevel.Error));
                }
                else
                {
                    configurationItems.Add(bKey, result[bKey]);
                }
            }
        }

        private IEnumerable<IConfigurationItem> CreateConfigurationItems(
            ITypeDescription memberTypeDescription,
            IPathNode parentPathNode,
            bool isInBuilderBlock,
            Context context)
        {
            IMemberConfiguration AdjustSource(IMemberConfiguration memberConfiguration)
            {
                if (isInBuilderBlock && memberConfiguration.Source is FromCode fromCode)
                {
                    return memberConfiguration.WithSource(new FromBuilderClass(fromCode));
                }
                else
                {
                    return memberConfiguration;
                }
            }

            if (memberTypeDescription.IsBaseType)
            {
                return Enumerable.Empty<IConfigurationItem>();
            }

            var typeFullName = parentPathNode.TypeSymbol
                .Map(symbol => symbol.GetTypeFullName())
                .SomeOrProvided(() => memberTypeDescription.TypeFullName);

            var id = FixtureItemId.CreateNamed(
                    parentPathNode.ConstructPathToRoot(context.RootPathName),
                    typeFullName)
                .WithRootItemPath(context.RootPathName);

            var configurationItem = this._configurationItemFactory.CreateConfigurationItem(id);
            var result = Enumerable.Empty<IConfigurationItem>();

            foreach (var memberDescription in memberTypeDescription.GetMembers())
            {
                var memberName = memberDescription.GetMemberPathName();

                if (!parentPathNode.Children.ContainsKey(memberName))
                {
                    continue;
                }

                var childPathNode = parentPathNode[memberName];

                var memberOriginalReturnTypeDescription = memberDescription.GetReturnTypeDescription();

                var memberReturnTypeDescription = childPathNode.TypeSymbol
                    .Map(symbol => this._typeDescriptionFactory.CreateDescription(symbol))
                    .SomeOrProvided(() => memberOriginalReturnTypeDescription);

                if (memberDescription is IMethodDescription { IsConstructor: true } ctorDescription)
                {
                    var parameterMemberConfigs = new List<IMemberConfiguration>();
                    foreach (var parameterDescription in ctorDescription.DeclaredParameters
                                 .Where(description => childPathNode.Children.ContainsKey(description.Name)))
                    {
                        var parameterPathNode = childPathNode[parameterDescription.Name];
                        var parameterOriginalReturnTypeDescription = parameterDescription.GetReturnTypeDescription();
                        var parameterReturnTypeDescription = parameterPathNode.TypeSymbol
                            .Map(symbol => this._typeDescriptionFactory.CreateDescription(symbol))
                            .SomeOrProvided(() => parameterOriginalReturnTypeDescription);

                        var memberConfig = CreateMemberConfiguration(
                            context,
                            parameterPathNode,
                            parameterDescription.Name,
                            parameterDescription,
                            parameterReturnTypeDescription,
                            parameterOriginalReturnTypeDescription)
                            .MapSuccess(AdjustSource);

                        memberConfig.Do(
                            c => parameterMemberConfigs.Add(AdjustSource(c)),
                            failure => this.Log(failure.Message, LogLevel.Error));

                        result = result.Concat(
                            this.CreateConfigurationItems(parameterReturnTypeDescription, parameterPathNode, isInBuilderBlock, context));
                    }

                    var ctorMemberConfig = CtorMemberConfiguration.Create(
                        context.Source,
                        parameterMemberConfigs.ToArray());

                    configurationItem = configurationItem.AddOrUpdateMemberConfiguration(ctorMemberConfig);
                }
                else
                {
                    var memberConfig = CreateMemberConfiguration(
                        context,
                        childPathNode,
                        memberDescription.Name,
                        memberDescription,
                        memberReturnTypeDescription,
                        memberOriginalReturnTypeDescription)
                        .MapSuccess(AdjustSource);

                    memberConfig.Do(
                        c => configurationItem = configurationItem.AddOrUpdateMemberConfiguration(c),
                        failure => this.Log(failure.Message, LogLevel.Error));

                    result = result.Concat(
                        this.CreateConfigurationItems(memberReturnTypeDescription, childPathNode, isInBuilderBlock, context));
                }
            }

            return result.Prepend(configurationItem);
        }

        private static IResult<IMemberConfiguration, Failure> CreateMemberConfiguration(
            Context context,
            IPathNode childPathNode,
            string memberName,
            IBaseDescription memberDescription,
            ITypeDescription memberReturnTypeDescription,
            ITypeDescription memberOriginalReturnTypeDescription) =>
                childPathNode.InvocationSyntax
                    .Match(
                        invocationSyntax =>
                            CreateMemberConfiguration(
                                childPathNode,
                                context,
                                invocationSyntax,
                                memberName,
                                memberDescription,
                                memberReturnTypeDescription,
                                memberOriginalReturnTypeDescription),
                        () =>
                            Success<IMemberConfiguration, Failure>(
                                CreateTypeLinkMemberConfiguration(
                                    childPathNode,
                                    memberReturnTypeDescription.TypeFullName,
                                    new UnknownDocumentLocation(),
                                    context)));

        private static IResult<IMemberConfiguration, Failure> CreateMemberConfiguration(
            IPathNode pathNode,
            Context context,
            InvocationExpressionSyntax invocationSyntax,
            string memberName,
            IBaseDescription memberDescription,
            ITypeDescription memberReturnDescription,
            ITypeDescription memberOriginalReturnTypeDescription) =>
            from identifierName in invocationSyntax.Expression.ChildNodes()
                .OfType<NameSyntax>()
                .FirstOrNone()
                .Bind(nameSyntax => nameSyntax.DescendantTokens().FirstOrNone(token => token.IsKind(SyntaxKind.IdentifierToken)))
                .ToResult($"Cannot find the identifier name of {invocationSyntax.Expression}")
            from memberConfig in CreateMemberConfiguration(
                memberName,
                memberDescription,
                memberReturnDescription,
                memberOriginalReturnTypeDescription,
                pathNode,
                identifierName,
                invocationSyntax.ArgumentList,
                context)
            select memberConfig;

        private static IResult<IMemberConfiguration, Failure> CreateMemberConfiguration(
            string memberName,
            IBaseDescription memberDescription,
            ITypeDescription memberReturnDescription,
            ITypeDescription memberOriginalReturnTypeDescription,
            IPathNode pathNode,
            SyntaxToken nameToken,
            ArgumentListSyntax argumentListSyntax,
            Context context) =>
            memberDescription is IMethodDescription methodDescription
                ? CreateMethodConfiguration(memberName, methodDescription, memberReturnDescription, memberOriginalReturnTypeDescription, pathNode, nameToken, argumentListSyntax, context)
                : CreateSimpleMemberConfiguration(memberName, memberReturnDescription, pathNode, nameToken, argumentListSyntax, context);

        private static IResult<IMemberConfiguration, Failure> CreateMethodConfiguration(
            string memberName,
            IMethodDescription methodDescription,
            ITypeDescription memberReturnDescription,
            ITypeDescription memberOriginalReturnTypeDescription,
            IPathNode pathNode,
            SyntaxToken nameToken,
            ArgumentListSyntax argumentListSyntax,
            Context context) =>
                from memberConfiguration in CreateSimpleMemberConfiguration(
                    memberName,
                    memberReturnDescription,
                    pathNode,
                    nameToken,
                    argumentListSyntax,
                    context)
                select MethodConfiguration.Create(
                    methodDescription,
                    context.Source,
                    memberConfiguration);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1114:Parameter list should follow declaration", Justification = "Pattermatching with comment")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:Parameter should follow comma", Justification = "Pattermatching with comment")]
        private static IResult<IMemberConfiguration, Failure> CreateSimpleMemberConfiguration(
            string memberName,
            ITypeDescription memberReturnDescription,
            IPathNode pathNode,
            SyntaxToken nameToken,
            ArgumentListSyntax argumentListSyntax,
            Context context) =>
            nameToken.Text switch
            {
                "Value" =>
                    argumentListSyntax.Arguments
                        .FirstOrNone()
                        .ToResult(new Failure("Value has no arguments."))
                        .MapSuccess(
                            syntax =>
                                GetValueArgument(syntax, context)
                                    .Match(

                                        // is literal value
                                        value =>
                                            value switch
                                            {
                                                Unit => (IMemberConfiguration)new NullValueMemberConfiguration(
                                                    memberName,
                                                    context.Source),
                                                _ => new ValueMemberConfiguration(
                                                    memberName,
                                                    value,
                                                    context.Source with { DocumentLocation = new RoslynDocumentLocation(syntax.GetLocation()) }),
                                            },

                                        // is not an literal value
                                        () =>
                                            new CodeValueMemberConfiguration(
                                            memberName,
                                            syntax.ToFullString(),
                                            context.Source with { DocumentLocation = new RoslynDocumentLocation(syntax.GetLocation()) }))),

                "Unique" =>
                    Success<IMemberConfiguration, Failure>(
                        new UniqueValueMemberConfiguration(
                            memberName,
                            context.Source with { DocumentLocation = new RoslynDocumentLocation(nameToken.GetLocation()) })),

                "Undefined" =>
                    Success<IMemberConfiguration, Failure>(
                        new UndefinedMemberConfiguration(
                            memberName,
                            memberReturnDescription.TypeFullName,
                            context.Source with { DocumentLocation = new RoslynDocumentLocation(nameToken.GetLocation()) })),

                "Stub" => CreateTypeLinkMemberConfiguration(pathNode, new RoslynDocumentLocation(nameToken.GetLocation()), context),

                "InstanceOf" => CreateTypeLinkMemberConfiguration(
                    pathNode,
                    new RoslynDocumentLocation(nameToken.GetLocation()),
                    context),

                _ => Failure<IMemberConfiguration, Failure>(
                    new Failure("Expected member configuration to end with Value, Unique, Stub or InstanceOf")),
            };

        private static IResult<IMemberConfiguration, Failure> CreateTypeLinkMemberConfiguration(
            IPathNode pathNode,
            IDocumentLocation location,
            Context context) =>
                pathNode.TypeSymbol
                    .ToResult(new Failure("Stub or InstanceOf are is not called with its generic argument."))
                    .MapSuccess(symbol => CreateTypeLinkMemberConfiguration(pathNode, symbol, location, context));

        private static IMemberConfiguration CreateTypeLinkMemberConfiguration(
            IPathNode pathNode,
            ITypeSymbol typeSymbol,
            IDocumentLocation location,
            Context context) =>
            CreateTypeLinkMemberConfiguration(pathNode, typeSymbol.GetTypeFullName(), location, context);

        private static IMemberConfiguration CreateTypeLinkMemberConfiguration(
            IPathNode pathNode,
            ITypeFullName typeFullName,
            IDocumentLocation location,
            Context context)
        {
            var id = FixtureItemId.CreateNamed(pathNode.ConstructPathToRoot(context.RootPathName), typeFullName)
                .WithRootItemPath(context.RootPathName);

            return new LinkMemberConfiguration(pathNode.MemberName, id, context.Source with
            {
                DocumentLocation = location,
            });
        }

        private static Maybe<object> GetValueArgument(ArgumentSyntax argumentSyntax, Context context)
        {
            var literal = argumentSyntax
                .ChildNodes()
                .OfType<LiteralExpressionSyntax>()
                .FirstOrNone();

            if (literal.AsMaybeValue() is SomeValue<LiteralExpressionSyntax> someLiteral)
            {
                return someLiteral.Value.Token.Value switch
                {
                    null => Unit.New,
                    string s => s,
                    char c => c,
                    _ => new SimpleLiteralValue(someLiteral.Value.Token.Text.Trim()),
                };
            }
            else
            {
                switch (argumentSyntax.Expression)
                {
                    case MemberAccessExpressionSyntax memberAccessExpression:
                        {
                            var declaredSymbol = ModelExtensions.GetSymbolInfo(context.SemanticModel, memberAccessExpression).Symbol;

                            if (declaredSymbol is IFieldSymbol fieldSymbol && fieldSymbol.Type.TypeKind == TypeKind.Enum)
                            {
                                return new EnumValue(
                                    fieldSymbol.Type.GetTypeFullName(),
                                    memberAccessExpression.Name.ToString());
                            }

                            break;
                        }
                }
            }

            return None();
        }

        #endregion

        #region Nested type: Context

        private sealed class Context : RoslynContext
        {
            #region ctors

            public Context(
                IRoslynContext context,
                string rootPathName)
                : base(context)
            {
                this.RootPathName = rootPathName;
                this.Source = new FromCode(this.Document.FilePath, new UnknownDocumentLocation());
            }

            #endregion

            #region properties

            public FromCode Source { get; }

            public string RootPathName { get; }

            #endregion
        }

        #endregion
    }
}