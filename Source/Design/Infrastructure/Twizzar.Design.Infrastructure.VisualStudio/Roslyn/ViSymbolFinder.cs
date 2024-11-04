using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

using Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <summary>
    /// Class implements the <see cref="ISymbolFinder"/> and is a wrapper for <see cref="SymbolFinder"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ViSymbolFinder : ISymbolFinder, IQuery
    {
        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public async Task<IEnumerable<ITypeSymbol>> FindImplementationsAndDerivedTypesAsync(
            ITypeSymbol typeSymbol,
            Solution solution)
        {
            this.EnsureMany()
                .Parameter(typeSymbol, nameof(typeSymbol))
                .Parameter(solution, nameof(solution))
                .ThrowWhenNull();

            var implementations = await this.FindImplementationsAsync(typeSymbol.OriginalDefinition, solution);
            var derivedClasses = await this.FindDerivedClassesAsync(typeSymbol.OriginalDefinition, solution);
            var derivedInterfaces = await this.FindDerivedInterfacesAsync(typeSymbol.OriginalDefinition, solution);

            return implementations
                .Concat(derivedClasses)
                .Concat(derivedInterfaces);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ITypeSymbol>> FindImplementationsAsync(ITypeSymbol typeSymbol, Solution solution)
        {
            this.EnsureMany()
                .Parameter(typeSymbol, nameof(typeSymbol))
                .Parameter(solution, nameof(solution))
                .ThrowWhenNull();

            return typeSymbol switch
            {
                INamedTypeSymbol namedTypeSymbol =>
                    await this.DoExternalSymbolFinderCallAsync(
                        SymbolFinder
                            .FindImplementationsAsync(namedTypeSymbol, solution, true, solution.Projects.ToImmutableHashSet())),
                IArrayTypeSymbol _ => new List<ITypeSymbol>(),
                _ => new List<ITypeSymbol>(),
            };
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ITypeSymbol>> FindDerivedClassesAsync(ITypeSymbol typeSymbol, Solution solution)
        {
            this.EnsureMany()
                .Parameter(typeSymbol, nameof(typeSymbol))
                .Parameter(solution, nameof(solution))
                .ThrowWhenNull();

            return typeSymbol switch
            {
                INamedTypeSymbol namedTypeSymbol =>
                    await this.DoExternalSymbolFinderCallAsync(
                        SymbolFinder.FindDerivedClassesAsync(namedTypeSymbol, solution)),
                IArrayTypeSymbol _ => new List<ITypeSymbol>(),
                _ => new List<ITypeSymbol>(),
            };
        }

        private async Task<IEnumerable<ITypeSymbol>> FindDerivedInterfacesAsync(ITypeSymbol typeSymbol, Solution solution)
        {
            this.EnsureMany()
                .Parameter(typeSymbol, nameof(typeSymbol))
                .Parameter(solution, nameof(solution))
                .ThrowWhenNull();

            return typeSymbol switch
            {
                INamedTypeSymbol namedTypeSymbol =>
                    await this.DoExternalSymbolFinderCallAsync(
                        SymbolFinder.FindDerivedInterfacesAsync(namedTypeSymbol, solution)),
                IArrayTypeSymbol _ => new List<ITypeSymbol>(),
                _ => new List<ITypeSymbol>(),
            };
        }

        [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Ok in this case.")]
        private async Task<IEnumerable<ITypeSymbol>> DoExternalSymbolFinderCallAsync(Task<IEnumerable<INamedTypeSymbol>> symbolFinderTask)
        {
            try
            {
                return await symbolFinderTask;
            }
            catch (Exception exception)
            {
                this.Logger?.Log(exception);
                return new List<ITypeSymbol>();
            }
        }

        #endregion
    }
}