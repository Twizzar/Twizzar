using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Shared.CoreInterfaces.RoslynPipeline;
using Twizzar.Design.Shared.Infrastructure.Extension;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.CoreInterfaces.Util;

using ViCommon.Functional.Monads.MaybeMonad;
using static Twizzar.Design.Shared.Infrastructure.ApiNames;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;

#pragma warning disable SA1629 // Documentation text should end with a period

namespace Twizzar.Design.Shared.Infrastructure.Discovery
{
    /// <inheritdoc cref="IDiscoverer" />
    public class Discoverer : IDiscoverer
    {
        #region members

        /// <inheritdoc />
        public IValuesOperation<(ItemBuilderCreationInformation ItemBuilderCreationInformation, PathProviderInformation PathProviderInformation)>
            DiscoverItemBuilderCreation(IValuesOperation<(SyntaxNode Node, SemanticModel SemanticModel)> operation) => operation
                .Where(
                    tuple => tuple.Node is ObjectCreationExpressionSyntax objectCreationExpression &&
                             objectCreationExpression.Type.ToString().StartsWith(ItemBuilderName))
                .Select(
                    (tuple, token) =>
                    {
                        var objectCreationExpression = (ObjectCreationExpressionSyntax)tuple.Node;

                        return (
                            tuple.SemanticModel,
                            objectCreationExpression,
                            tuple.SemanticModel.GetSymbolInfo(objectCreationExpression.Type, token).Symbol);
                    })
                .Where(tuple => tuple.Symbol is INamedTypeSymbol)
                .Select(
                    (tuple, _) => (
                        tuple.SemanticModel,
                        tuple.objectCreationExpression,
                        builderType: (INamedTypeSymbol)tuple.Symbol,
                        builderTypeFullName: ((INamedTypeSymbol)tuple.Symbol).GetTypeFullName()))

                // check if the created type has at least one generic argument. And it inherits or is of the type ItemBuilder<T>
                .Where(
                    tuple => tuple.builderType.TypeArguments.Length > 0 &&
                             tuple.builderTypeFullName.FullName.StartsWith($"{ApiNamespace}.{ItemBuilderT1Name}"))
                .WithComparer(new HashEqualityComparer<(SemanticModel SemanticModel, ObjectCreationExpressionSyntax ObjectCreationExpression, INamedTypeSymbol BuilderType, ITypeFullName BuilderTypeFullName)>(
                    tuple =>
                                (tuple.SemanticModel.GetHashCode() * 397) ^
                                (SymbolEqualityComparer.Default.GetHashCode(tuple.BuilderType) * 397) ^
                                (tuple.BuilderTypeFullName.FullName.GetHashCode() * 397) ^
                                (tuple.ObjectCreationExpression.ToString().GetHashCode() * 397)))
                .Select(
                    (tuple, _) =>
                        (
                            tuple.SemanticModel,
                            tuple.objectCreationExpression,
                            tuple.builderType,
                            tuple.builderTypeFullName,
                            fixtureItemType: tuple.builderType.TypeArguments[0]))
                .Where(tuple => !IsUnboundGeneric(tuple.fixtureItemType))
                .WithComparer(new HashEqualityComparer<(SemanticModel SemanticModel, ObjectCreationExpressionSyntax ObjectCreationExpression, INamedTypeSymbol BuilderType, ITypeFullName BuilderTypeFullName, ITypeSymbol FixtureItemType)>(
                    tuple => new[]
                    {
                        tuple.ObjectCreationExpression.ToString().GetHashCode(),
                        SymbolEqualityComparer.IncludeNullability.GetHashCode(tuple.FixtureItemType),
                        SymbolEqualityComparer.IncludeNullability.GetHashCode(tuple.BuilderType),
                    }))
                .Select(
                    (tuple, _) =>
                    {
                        var fixtureItemTypeFullName = tuple.fixtureItemType.GetTypeFullName();
                        var ns = tuple.builderTypeFullName.GetNameSpace();
                        var pathProviderName = $"{tuple.fixtureItemType.GetTypeFullName().GetFriendlyCSharpTypeName().ToSourceVariableCodeFriendly()}{PathProviderPostfix}";
                        var sourceTypeSymbol = tuple.SemanticModel.Compilation.Assembly;

                        var itemBuilderInformation = new ItemBuilderCreationInformation(
                            fixtureItemTypeFullName.FullName,
                            tuple.objectCreationExpression,
                            tuple.builderType.TypeArguments[0],
                            $"{ns}.{pathProviderName}");

                        var pathProviderInformation = new PathProviderInformation(
                            pathProviderName,
                            ns,
                            tuple.fixtureItemType,
                            sourceTypeSymbol,
                            tuple.SemanticModel);

                        return (itemBuilderInformation, pathProviderInformation);
                    });

        /// <inheritdoc/>
        public IValuesOperation<PathProviderInformation> DiscoverCustomItemBuilder(IValuesOperation<(SyntaxNode Node, SemanticModel SemanticModel)> operation) =>
            operation
                .Where(tuple => tuple.Node is ClassDeclarationSyntax)
                .Select((tuple, token) => (tuple.SemanticModel, classDeclarationSymbol: tuple.SemanticModel.GetDeclaredSymbol(tuple.Node, token)))
                .Where(tuple => tuple.classDeclarationSymbol is INamedTypeSymbol)
                .Select((tuple, _) => (tuple.SemanticModel, classDeclarationSymbol: (INamedTypeSymbol)tuple.classDeclarationSymbol))

                // check for class MyBuilder : ItemBuilder<T, TPathProvider>
                .Select(
                    (t, token) => t.classDeclarationSymbol.FindBaseSymbol(ItemBuilderT2Name, token)
                        .Map(symbol => (t.SemanticModel, t.classDeclarationSymbol, itemBuilderSymbol: symbol)))
                .Somes()
                .WithComparer(
                    new HashEqualityComparer<(SemanticModel SemanticModel, INamedTypeSymbol ClassDeclarationSymbol, INamedTypeSymbol ItemBuilderSymbol)>(
                    tuple => new[]
                    {
                        SymbolEqualityComparer.Default.GetHashCode(tuple.ItemBuilderSymbol),
                        SymbolEqualityComparer.Default.GetHashCode(tuple.ClassDeclarationSymbol),
                    }))
                .Select(
                    (tuple, _) =>
                    {
                        if (tuple.itemBuilderSymbol.GetTypeFullName().GetNameSpace() != ApiNamespace)
                        {
                            return None();
                        }

                        var fixtureItemSymbol = tuple.itemBuilderSymbol.TypeArguments[0];

                        if (IsUnboundGeneric(fixtureItemSymbol))
                        {
                            return None();
                        }

                        var pathProviderName = tuple.itemBuilderSymbol.TypeArguments[1];
                        var classTypeFullName = tuple.classDeclarationSymbol.GetTypeFullName();

                        var information = new PathProviderInformation(
                            pathProviderName.Name,
                            classTypeFullName.GetNameSpace(),
                            fixtureItemSymbol,
                            tuple.classDeclarationSymbol,
                            tuple.SemanticModel);

                        return Some(information);
                    })
                .Somes();

        /// <inheritdoc/>
        public IValuesOperation<(IdentifierNameSyntax IdentifierNameSyntax, SemanticModel SemanticModel, PathProviderInformation PathProviderInformation)>
            DiscoverMemberSelection(IValuesOperation<(SyntaxNode Node, SemanticModel SemanticModel)> operation)
        {
            /*
             * Check if the syntax node is a lambda expression and the lambda expression has an expression body.
             * Try to get the symbol of the lambda expression, the symbol should be a method symbol with one parameter.
             * Then check if the parameter of the method symbol is an error symbol.
             * Because the With methods gets generated by us, the lambda parameter type is at this time not
             * known yet. So the symbol is of the type IErrorTypeSymbol.
             *
             * This should be true for statements like this: p => p.MyMember1.Value(3)
             */
            var lambdaExpressions = operation
                .Where(
                    tuple => tuple.Node is SimpleLambdaExpressionSyntax { ExpressionBody: { } } lambdaExpression &&
                             tuple.SemanticModel.GetSymbolInfo(lambdaExpression).Symbol is IMethodSymbol
                             {
                                 Parameters.Length: 1,
                             } methodSymbol &&
                             methodSymbol.Parameters[0].Type is IErrorTypeSymbol)
                .Select((tuple, _) => (tuple.SemanticModel, (SimpleLambdaExpressionSyntax)tuple.Node));

            var withMemberSelections = DiscoverWithMemberSelection(lambdaExpressions);

            /*
             * If the member selection is not written in the With statement then we can discover all the symbols we need from the
             * path alone. This lets us discover every statement written in this manner: p.MyMember1.Value(3) where p is a PathProvider.
             */
            var identifierNames = operation
                .Where(tuple => tuple.Node is IdentifierNameSyntax)
                .Select((tuple, _) => (tuple.SemanticModel, (IdentifierNameSyntax)tuple.Node));

            var typedMemberSelections = DiscoverTypedMemberSelection(identifierNames);

            return withMemberSelections.Collect()
                .Combine(typedMemberSelections.Collect())
                .SelectMany((tuple, _) => tuple.Left.Concat(tuple.Right).ToImmutableArray());
        }

        private static IValuesOperation<(IdentifierNameSyntax IdentifierNameSyntax, SemanticModel SemanticModel, PathProviderInformation PathProviderInformation)>
            DiscoverTypedMemberSelection(IValuesOperation<(SemanticModel SemanticModel, IdentifierNameSyntax IdentifierNameSyntax)> step)
        {
            // if we are in an lambda the symbol will be a parameter symbol. Else the identifier is probably a local symbol.
            return step.Select(
                    (tuple, token) =>
                    {
                        var pathProviderTypeSymbol =
                            tuple.SemanticModel.GetSymbolInfo(tuple.IdentifierNameSyntax, token).Symbol switch
                            {
                                IParameterSymbol x => x.Type,
                                ILocalSymbol x => x.Type,
                                _ => null,
                            };

                        return (semanticModel: tuple.SemanticModel, identifierNameSyntax: tuple.IdentifierNameSyntax, pathProviderTypeSymbol);
                    })

                // check if the symbol inherits form the PathProvider.
                .Select((tuple, token) =>
                    tuple.pathProviderTypeSymbol.FindBaseSymbol(PathProviderBaseClassName, token)
                         .Map(symbol => (tuple.semanticModel, tuple.identifierNameSyntax, tuple.pathProviderTypeSymbol, basePathProviderSymbol: symbol)))
                 .Somes()

                // check if it is really our ItemBuilder<T> by checking the full name
                .Where(
                    tuple => tuple.basePathProviderSymbol.TypeArguments.Length > 0 &&
                             tuple.basePathProviderSymbol.GetTypeFullName()
                                 .FullName.StartsWith($"{ApiNamespace}."))

                 .Select(
                    (tuple, _) =>
                    {
                        var pathProviderTypeFullName = tuple.pathProviderTypeSymbol.GetTypeFullName();
                        var fixtureItemSymbol = tuple.basePathProviderSymbol.TypeArguments[0];

                        var information = new PathProviderInformation(
                            pathProviderTypeFullName.GetTypeName(),
                            pathProviderTypeFullName.GetNameSpace(),
                            fixtureItemSymbol,
                            tuple.pathProviderTypeSymbol,
                            tuple.semanticModel);

                        return (tuple.identifierNameSyntax, tuple.semanticModel, information);
                    });
        }

        /// <summary>
        /// Discover the with member selection form a lambda expression.
        ///
        /// This should look something like this:
        /// <code>
        /// builder.With(p => p.Member.Value(5))
        /// </code>
        /// or
        /// <code>
        /// scope.Verify(p => p.Method)
        /// </code>
        /// where both builder and scope implements IPathSelectionProvider. Either IPathSelectionProviderT1 then we create a default path or
        /// IPathSelectionProviderT2 where the second generic parameter defines the path type name.
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        private static IValuesOperation<(IdentifierNameSyntax IdentifierNameSyntax, SemanticModel SemanticModel, PathProviderInformation PathProviderInformation)>
            DiscoverWithMemberSelection(IValuesOperation<(SemanticModel SemanticModel, SimpleLambdaExpressionSyntax LambdaExpression)> step)
        {
            /*
             * Here we need some trickery to find the fixture item type.
             * Because the lambda parameter has an error symbol we need to find the Builder where
             * the .With method is called from.
             *
             * Syntax tree will look similar to this:
             *
             * Invocation Expression: new ItemBuilder<Car>().With(...)
             * ├─MemberAccessExpression: new ItemBuilder<Car>()
             * │   └─ObjectCreationExpression: new
             * └─ArgumentList: (p => p.Engine.Cylinder.Stub<ICylinder>())
             *     └─LambdaExpression: p => p.Engine.Cylinder.Stub<ICylinder>()
             *
             * if not the its probably a call in a CustomBuilder class or the item scope.
             */
            Maybe<INamedTypeSymbol> GetPathSelectionProviderSymbol(
                SimpleLambdaExpressionSyntax lambdaExpression,
                SemanticModel semanticModel,
                CancellationToken cancellationToken) =>
                    lambdaExpression.Ancestors()
                        .OfType<InvocationExpressionSyntax>()
                        .FirstOrNone()
                        .Select(syntax =>
                            {
                                var current = syntax.Expression;
                                while (current is MemberAccessExpressionSyntax or InvocationExpressionSyntax)
                                {
                                    current = current switch
                                    {
                                        MemberAccessExpressionSyntax x => x.Expression,
                                        InvocationExpressionSyntax x => x.Expression,
                                        _ => throw new InternalException("Unreachable pattern reached."),
                                    };
                                }

                                return current;
                            })
                        .Bind(syntax => ToMaybe(semanticModel.GetSymbolInfo(syntax, cancellationToken).Symbol))
                        .Bind(symbol => symbol switch
                        {
                            // new ItemBuilder
                            IMethodSymbol { MethodKind: MethodKind.Constructor, ReceiverType: INamedTypeSymbol x } =>
                                Some(x),
                            IMethodSymbol { ReturnType: INamedTypeSymbol x } =>
                                Some(x),
                            INamedTypeSymbol nSymbol =>
                                Some(nSymbol.ContainingType),
                            ILocalSymbol { Type: IErrorTypeSymbol } =>
                                None(),
                            ILocalSymbol { Type: INamedTypeSymbol x } =>
                                Some(x),

                            // this.With(..)
                            IParameterSymbol { Type: INamedTypeSymbol x } =>
                                Some(x),
                            _ => None(),
                        });

            return step
                .Where(tuple => tuple.LambdaExpression.ExpressionBody != null)
                .Select(
                    (tuple, token) => GetPathSelectionProviderSymbol(tuple.LambdaExpression, tuple.SemanticModel, token)
                        .Map(symbol => (tuple.LambdaExpression, tuple.SemanticModel, callerSymbol: symbol)))
                .Somes()

                // check if the builder inherits form IPathSelectionProvider<T>
                .Select(
                    (tuple, token) => tuple.callerSymbol.FindInheritedInterface(IPathSelectionProviderT1Name)
                        .Map(
                            symbol => (tuple.LambdaExpression, tuple.SemanticModel, tuple.callerSymbol,
                                pathSelectionT2Symbol: symbol)))
                .Somes()

                // check if it is really our IPathSelectionProvider<T> by checking the namespace
                .Where(
                    tuple => tuple.pathSelectionT2Symbol.TypeArguments.Length > 0 &&
                             tuple.pathSelectionT2Symbol.GetTypeFullName()
                                 .FullName.StartsWith($"{ApiNamespace}."))

                .Select(
                    (tuple, token) =>
                    {
                        // caller should be ItemBuilder<TFixtureItem>, an CustomItemBuilder which derives form ItemBuilder<TFixtureItem, TPathProvider>,
                        // ItemScope<TFixtureItem> or ItemScope<TFixtureItem, TPathProvider>
                        var callerTypeFullName = tuple.callerSymbol.GetTypeFullName();
                        var fixtureItemType = tuple.pathSelectionT2Symbol.TypeArguments[0];

                        if (IsUnboundGeneric(fixtureItemType))
                        {
                            return None();
                        }

                        var lambdaMethodSymbol = tuple.SemanticModel.GetSymbolInfo(tuple.LambdaExpression, token).Symbol;
                        if (lambdaMethodSymbol is null)
                        {
                            return None();
                        }

                        var sourceTypeSymbol = lambdaMethodSymbol.ContainingAssembly;

                        var pathProviderName = $"{fixtureItemType.GetTypeFullName().GetFriendlyCSharpTypeName().ToSourceVariableCodeFriendly()}{PathProviderPostfix}";
                        var ns = callerTypeFullName.GetNameSpace();

                        // if the builder inherits form IPathSelectionProviderT2Name<T1, T2> then T2 defines the path provider name.
                        if (tuple.callerSymbol.FindInheritedInterface(IPathSelectionProviderT2Name).AsMaybeValue() is
                                SomeValue<INamedTypeSymbol> customBuilderSymbol &&
                            customBuilderSymbol.Value.TypeArguments.Length > 1)
                        {
                            ns = lambdaMethodSymbol.ContainingNamespace?.IsGlobalNamespace == true
                                ? string.Empty
                                : lambdaMethodSymbol.ContainingNamespace?.ToString() ?? string.Empty;

                            pathProviderName = customBuilderSymbol.Value.TypeArguments[1].Name;
                        }

                        /*
                         * if the code looks like this: p.Member1.Value()
                         * The Syntax Tree should look like this:
                         * SimpleLambdaExpression
                         * ├─Parameter
                         * └─InvocationExpression
                         *   └─MemberAccessExpression
                         *     ├─MemberAccessExpression
                         *     │ ├─ IdentifierName: p
                         *     │ ├─ DotToken
                         *     │ └─ IdentifierName: Member1
                         *     ├─ DotToken
                         *     └─ IdentifierName: Value
                         * We are interested in the p IdentifierName.
                         */
                        return tuple.LambdaExpression!.ExpressionBody
                            .DescendantNodesAndSelf()
                            .OfType<IdentifierNameSyntax>()
                            .Where(syntax => syntax.Identifier.Text == tuple.LambdaExpression.Parameter.Identifier.Text)
                            .LastOrNone()
                            .Map(
                                syntax => (
                                    syntax,
                                    tuple.SemanticModel,
                                    new PathProviderInformation(
                                    pathProviderName,
                                    ns,
                                    fixtureItemType,
                                    sourceTypeSymbol,
                                    tuple.SemanticModel)));
                    })
                .Somes();
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