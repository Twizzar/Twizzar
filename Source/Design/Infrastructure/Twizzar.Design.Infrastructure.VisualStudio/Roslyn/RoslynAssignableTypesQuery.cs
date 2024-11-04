using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Common.FixtureItem.Description.Services;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn;
using Twizzar.Design.Shared.Infrastructure.Extension;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <summary>
    /// Class implement <see cref="IAssignableTypesQuery"/> using Roslyn Api.
    /// </summary>
    public class RoslynAssignableTypesQuery : IAssignableTypesQuery
    {
        #region fields

        private readonly Workspace _workspace;
        private readonly ISymbolFinder _symbolFinder;
        private readonly IRoslynDescriptionFactory _roslynTypeDescriptionFactory;
        private readonly ITypeDescriptionQuery _typeDescriptionQuery;

        private readonly Dictionary<IBaseDescription, IEnumerable<IBaseDescription>> _cache = new();

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynAssignableTypesQuery"/> class.
        /// </summary>
        /// <param name="workspace">The roslyn workspace.</param>
        /// <param name="symbolFinder">the symbol finder.</param>
        /// <param name="typeDescriptionQuery">The type description query.</param>
        /// <param name="roslynDescriptionFactory">Roslyn symbol type factory.</param>
        public RoslynAssignableTypesQuery(
            Workspace workspace,
            ISymbolFinder symbolFinder,
            ITypeDescriptionQuery typeDescriptionQuery,
            IRoslynDescriptionFactory roslynDescriptionFactory)
        {
            this.EnsureMany()
                .Parameter(workspace, nameof(workspace))
                .Parameter(symbolFinder, nameof(symbolFinder))
                .Parameter(typeDescriptionQuery, nameof(typeDescriptionQuery))
                .Parameter(roslynDescriptionFactory, nameof(roslynDescriptionFactory))
                .ThrowWhenNull();

            this._workspace = workspace;
            this._symbolFinder = symbolFinder;
            this._typeDescriptionQuery = typeDescriptionQuery;
            this._roslynTypeDescriptionFactory = roslynDescriptionFactory;
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
        public async Task<IEnumerable<IBaseDescription>> GetAssignableTypesAsync(IBaseDescription typeDescription)
        {
            if (this._cache.TryGetValue(typeDescription, out var cachedResult))
            {
                return cachedResult;
            }

            var assignableSymbols = new List<IBaseDescription> { typeDescription };

            try
            {
                var roslynTypeDescription =
                    Maybe.ToMaybe(typeDescription.GetReturnTypeDescription() as IRoslynTypeDescription);

                var maybeTypeSymbol = await this.UpgradeTypeSymbolAsync(roslynTypeDescription, CancellationToken.None);

                if (maybeTypeSymbol.AsMaybeValue() is not SomeValue<ITypeSymbol> typeSymbol ||
                    roslynTypeDescription.Map(description => description.IsBaseType).SomeOrProvided(true))
                {
                    return assignableSymbols;
                }

                var foundSymbols = await this._symbolFinder
                    .FindImplementationsAndDerivedTypesAsync(typeSymbol.Value, this._workspace.CurrentSolution);

                var foundTypeFullNames = foundSymbols
                    .Select(symbol => this._roslynTypeDescriptionFactory.CreateDescription(symbol));

                assignableSymbols.AddRange(foundTypeFullNames);
            }
            catch (Exception exception)
            {
                this.Logger?.Log(exception);
            }

            this._cache[typeDescription] = assignableSymbols;
            return assignableSymbols;
        }

        /// <inheritdoc />
        public async Task<Maybe<IBaseDescription>> IsAssignableTo(
            IBaseDescription baseDescription,
            ITypeFullName typeFullName,
            Maybe<string> rootItemPath)
        {
            // check parameters
            if (baseDescription is null || typeFullName is null)
            {
                return Maybe.None();
            }

            // resolve type description
            var resolvedTypeDescription =
                await this._typeDescriptionQuery.GetTypeDescriptionAsync(typeFullName, rootItemPath);

            if (resolvedTypeDescription.IsFailure)
            {
                return Maybe.None();
            }

            // get roslyn symbols and check if typeDescription inherits from baseDescription
            var roslynTypeDescription =
                resolvedTypeDescription.GetSuccessUnsafe().GetReturnTypeDescription() as IRoslynTypeDescription;

            var typeSymbol = roslynTypeDescription?.GetTypeSymbol();

            var roslynBaseTypeDescription = baseDescription.GetReturnTypeDescription() as IRoslynTypeDescription;

            var baseTypeSymbol =
                await this.UpgradeTypeSymbolAsync(Maybe.ToMaybe(roslynBaseTypeDescription), CancellationToken.None);

            if (typeSymbol is null || baseTypeSymbol.IsNone)
            {
                return Maybe.None();
            }

            if (typeSymbol.InheritsFromOrEquals(baseTypeSymbol.GetValueUnsafe()))
            {
                return Maybe.Some<IBaseDescription>(roslynTypeDescription);
            }

            return Maybe.None();
        }

        /// <inheritdoc />
        public Task InitializeAsync(ICompilationTypeQuery compilationTypeQuery)
        {
            this._cache.Clear();

            this._cache[compilationTypeQuery.ObjectTypeDescription] =
                compilationTypeQuery.AllTypes.ToList();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Tries to upgrade the typeSymbol.
        /// If not successful returns the old type symbol.
        /// If it fails to get the old type symbol returns None.
        /// </summary>
        /// <param name="roslynTypeDescription"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private Task<Maybe<ITypeSymbol>> UpgradeTypeSymbolAsync(
            Maybe<IRoslynTypeDescription> roslynTypeDescription,
            CancellationToken cancellationToken)
        {
            var currentSolution = this._workspace.CurrentSolution;
            var oldSymbol = roslynTypeDescription.Map(description => description.GetTypeSymbol());

            return oldSymbol
                .Bind(symbol =>
                    Maybe.Some(symbol switch
                    {
                        IArrayTypeSymbol s => s.ElementType.ContainingAssembly.Name,
                        _ => symbol?.ContainingAssembly?.Name,
                    }))
                .Bind(assemblyName => currentSolution.GetProjectByName(assemblyName))
                .MapAsync(project => project.GetCompilationAsync(cancellationToken))
                .BindAsync(compilation =>
                    Maybe.ToMaybe<ITypeSymbol>(
                        compilation.GetTypeByMetadataName(roslynTypeDescription.GetValueUnsafe()
                            .TypeFullName.FullName)))
                .BindNoneAsync(() => oldSymbol);
        }

        #endregion
    }
}