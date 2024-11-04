using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Design.Shared.Infrastructure.Discovery
{
    /// <summary>
    /// Information for a discovered path selection.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed record PathProviderInformation
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="PathProviderInformation"/> class.
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="nameSpace"></param>
        /// <param name="fixtureItemSymbol"></param>
        /// <param name="sourceType"></param>
        /// <param name="semanticModel"></param>
        public PathProviderInformation(
            string typeName,
            string nameSpace,
            ITypeSymbol fixtureItemSymbol,
            ISymbol sourceType,
            SemanticModel semanticModel)
        {
            EnsureHelper.GetDefault
                .Parameter(typeName, nameof(typeName))
                .IsNotNullAndNotEmpty()
                .ThrowOnFailure();

            EnsureHelper.GetDefault.Many()
                .Parameter(fixtureItemSymbol, nameof(fixtureItemSymbol))
                .Parameter(nameSpace, nameof(nameSpace))
                .Parameter(semanticModel, nameof(semanticModel))
                .ThrowWhenNull();

            this.TypeName = typeName;
            this.NameSpace = nameSpace;
            this.FixtureItemSymbol = fixtureItemSymbol;
            this.SourceType = sourceType;
            this.SemanticModel = semanticModel;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets type name of the path provider.
        /// </summary>
        public string TypeName { get; init; }

        /// <summary>
        /// Gets the namespace of the path provider.
        /// </summary>
        public string NameSpace { get; init; }

        /// <summary>
        /// Gets the fixture item type symbol for which the paths provider provides the paths.
        /// </summary>
        public ITypeSymbol FixtureItemSymbol { get; init; }

        /// <summary>
        /// Gets the type from which the path will be constructed.
        /// </summary>
        public ISymbol SourceType { get; init; }

        /// <summary>
        /// Gets the semantic model.
        /// </summary>
        public SemanticModel SemanticModel { get; init; }

        /// <summary>
        /// Gets the equality comparer which only compares the namespace and typename.
        /// </summary>
        public static IEqualityComparer<PathProviderInformation> NameSpaceTypeNameComparer { get; } =
            new NameSpaceTypeNameEqualityComparer();

        #endregion

        #region members

        /// <summary>
        /// Check for equality.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="symbolComparer">The comparer used for comparing fixture item symbol.</param>
        /// <returns></returns>
        public bool Equals(PathProviderInformation other, IEqualityComparer<ISymbol> symbolComparer)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.NameSpace.Trim() == other?.NameSpace?.Trim() &&
                   this.TypeName.Trim() == other?.TypeName?.Trim() &&
                   symbolComparer.Equals(this.FixtureItemSymbol, other?.FixtureItemSymbol);
        }

        /// <inheritdoc/>
        public bool Equals(PathProviderInformation other) =>
            this.Equals(other, SymbolEqualityComparer.Default);

        /// <summary>
        /// Get the hash code.
        /// </summary>
        /// <param name="symbolComparer">Symbol compared to calculate the hashcode of the fixtureItemSymbol.</param>
        /// <returns></returns>
        public int GetHashCode(IEqualityComparer<ISymbol> symbolComparer)
        {
            unchecked
            {
                var hashCode = symbolComparer.GetHashCode(this.FixtureItemSymbol);
                hashCode = (hashCode * 397) ^ (this.NameSpace != null ? this.NameSpace.Trim().GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.TypeName != null ? this.TypeName.Trim().GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <inheritdoc/>
        public override int GetHashCode() =>
            this.GetHashCode(SymbolEqualityComparer.Default);

        #endregion

        #region Nested type: NameSpaceTypeNameEqualityComparer

        private sealed class NameSpaceTypeNameEqualityComparer : IEqualityComparer<PathProviderInformation>
        {
            #region members

            public bool Equals(PathProviderInformation x, PathProviderInformation y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x is null)
                {
                    return false;
                }

                if (y is null)
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }

                return x.NameSpace == y.NameSpace && x.TypeName == y.TypeName;
            }

            public int GetHashCode(PathProviderInformation obj)
            {
                unchecked
                {
                    return ((obj.NameSpace != null ? obj.NameSpace.GetHashCode() : 0) * 397) ^
                           (obj.TypeName != null ? obj.TypeName.GetHashCode() : 0);
                }
            }

            #endregion
        }

        #endregion
    }
}