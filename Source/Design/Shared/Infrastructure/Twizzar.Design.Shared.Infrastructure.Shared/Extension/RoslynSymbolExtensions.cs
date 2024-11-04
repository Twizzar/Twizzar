using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Shared.Infrastructure.VisualStudio2019.Name;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;

namespace Twizzar.Design.Shared.Infrastructure.Extension
{
    /// <summary>
    /// Extension class for the <see cref="ISymbol"/> interfaces.
    /// </summary>
    public static class RoslynSymbolExtensions
    {
        #region static fields and constants

        private static readonly IEnsureHelper EnsureHelper = ViCommon.EnsureHelper.EnsureHelper.GetDefault;

        #endregion

        #region members

        /// <summary>
        /// Gets the type full name from a type symbol.
        /// </summary>
        /// <param name="typeSymbol">The type symbol.</param>
        /// <returns>The type full name.</returns>
        public static ITypeFullName GetTypeFullName(this ITypeSymbol typeSymbol)
        {
            EnsureHelper.Parameter(typeSymbol, nameof(typeSymbol)).ThrowWhenNull();
            return TypeFullName.CreateFromToken(new SymbolTypeFullNameToken(typeSymbol));
        }

        /// <summary>
        /// Convert the <see cref="Accessibility"/> enum form roslyn to <see cref="AccessModifier"/>.
        /// </summary>
        /// <param name="accessibility"></param>
        /// <returns>A new <see cref="AccessModifier"/>.</returns>
        public static AccessModifier ToAccessModifier(this Accessibility accessibility) =>
            new(
                accessibility == Accessibility.Private,
                accessibility == Accessibility.Public,
                accessibility == Accessibility.Protected,
                accessibility == Accessibility.Internal);

        /// <summary>
        /// Get the access modifier form a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>A new <see cref="AccessModifier"/>.</returns>
        public static AccessModifier GetAccessModifier(this ISymbol symbol) =>
            symbol.DeclaredAccessibility.ToAccessModifier();

        /// <summary>
        /// Map generic arguments form the roslyn <see cref="ITypeSymbol"/> to <see cref="GenericParameterType"/>s.
        /// </summary>
        /// <param name="genericParameters"></param>
        /// <returns>A immutable dictionary with position and generic parameter type.</returns>
        public static ImmutableDictionary<int, GenericParameterType> MapGenericArguments(
            this ImmutableArray<ITypeSymbol> genericParameters)
        {
            var builder = ImmutableDictionary.CreateBuilder<int, GenericParameterType>();

            for (var i = 0; i < genericParameters.Length; i++)
            {
                var genericParameter = genericParameters[i];

                if (genericParameter is ITypeParameterSymbol parameterSymbol)
                {
                    var constrains = parameterSymbol.ConstraintTypes.Select(symbol => symbol.GetTypeFullName())
                        .ToImmutableArray();

                    var genericParameterType = new GenericParameterType(
                        Some(genericParameters[i].GetTypeFullName()),
                        genericParameters[i].Name,
                        i,
                        constrains,
                        parameterSymbol.HasNotNullConstraint,
                        parameterSymbol.HasConstructorConstraint,
                        parameterSymbol.HasReferenceTypeConstraint,
                        parameterSymbol.HasUnmanagedTypeConstraint,
                        parameterSymbol.HasValueTypeConstraint);

                    builder.Add(i, genericParameterType);
                }
                else
                {
                    var genericParameterType = new GenericParameterType(
                        Some(genericParameters[i].GetTypeFullName()),
                        genericParameters[i].Name,
                        i,
                        ImmutableArray<ITypeFullName>.Empty);

                    builder.Add(i, genericParameterType);
                }
            }

            return builder.ToImmutable();
        }

        /// <summary>
        /// Checks whether given type is inherited from baseType.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <returns><c>true></c> if inherited type, otherwise <c>false</c>.</returns>
        public static bool InheritsFromOrEquals(
            this ITypeSymbol type, ITypeSymbol baseType)
        {
            if (type is null || baseType is null)
            {
                return false;
            }

            return type.GetBaseTypesAndThis().Concat(type.AllInterfaces)
                .Any(t => t.GetTypeFullName().Equals(baseType.GetTypeFullName()));
        }

        /// <summary>
        /// Checks whether given type is inherited from the baseType type name.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="metadataName"></param>
        /// <returns><c>true></c> if inherited type, otherwise <c>false</c>.</returns>
        public static bool InheritsFromOrEquals(
            this ITypeSymbol type, string metadataName)
        {
            if (type is null || metadataName is null)
            {
                return false;
            }

            return type.GetBaseTypesAndThis().Concat(type.AllInterfaces)
                .Any(t => t.MetadataName == metadataName);
        }

        /// <summary>
        /// Checks if the symbols is equal to or inherits from a type.
        /// Where the type is declared with unbound generic type arguments.
        /// If the symbol is of the type MyType�1[System.Int32] then it will be converted to
        /// the unbound type MyType�1.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="metadataName"></param>
        /// <returns>True if it inherits form or is equal to the given type.</returns>
        public static bool InheritsFromOrEqualsUnbound(
            this ITypeSymbol type, string metadataName)
        {
            if (type is null || metadataName is null)
            {
                return false;
            }

            return type.GetBaseTypesAndThis()
                .Concat(type.AllInterfaces)
                .Select(
                    symbol => symbol is INamedTypeSymbol { IsGenericType: true, IsUnboundGenericType: false } namedTypeSymbol
                        ? namedTypeSymbol.ConstructUnboundGenericType()
                        : symbol)
                .Any(t => t.MetadataName == metadataName);
        }

        /// <summary>
        /// Find the base type symbol.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Some if the symbol was found; else none.</returns>
        public static Maybe<INamedTypeSymbol> FindBaseSymbol(this ITypeSymbol type, string baseType, CancellationToken cancellationToken = default)
        {
            if (type is null || baseType is null)
            {
                return None();
            }

            return type.GetBaseTypesAndThis(cancellationToken)
                .OfType<INamedTypeSymbol>()
                .FirstOrNone(t => t.MetadataName == baseType);
        }

        /// <summary>
        /// Find a interface this type inherits form.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interface"></param>
        /// <returns>Some if the symbol was found; else none.</returns>
        public static Maybe<INamedTypeSymbol> FindInheritedInterface(this ITypeSymbol type, string @interface)
        {
            if (type is null || @interface is null)
            {
                return None();
            }

            return type.AllInterfaces
                .FirstOrNone(t => t.MetadataName == @interface);
        }

        /// <summary>
        /// Gets the array dimension of a given symbol.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns>The array dimension or an empty enumerable.</returns>
        public static IEnumerable<int> GetArrayDimension(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol is not IArrayTypeSymbol arraySymbol)
            {
                yield break;
            }

            var arrayElementType = arraySymbol.ElementType;

            foreach (var value in GetArrayDimension(arrayElementType))
            {
                yield return value;
            }

            yield return arraySymbol.Rank;
        }

        /// <summary>
        /// Find all descendant type arguments and return their symbols.
        /// If the type is <c>List&lt;Tuple&lt;int, string&gt;&gt;</c> then the returned symbols will be
        /// Tuple, int and string.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns>A sequence of all found symbols.</returns>
        public static IEnumerable<ISymbol> DescendantTypeArguments(this INamedTypeSymbol typeSymbol) =>
            typeSymbol.TypeArguments
                .SelectMany(
                    symbol =>
                        symbol is INamedTypeSymbol namedTypeSymbol
                            ? DescendantTypeArguments(namedTypeSymbol).Prepend(symbol)
                            : new[] { symbol });

        /// <summary>
        /// Gets all base types of the given type symbol.
        /// </summary>
        /// <param name="type">The type symbol.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>An enumerable containing all base types of the given type.</returns>
        public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol type, CancellationToken cancellationToken = default)
        {
            var current = type;
            while (current != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return current;
                current = current.BaseType;
            }
        }

        /// <summary>
        /// Gets all members of this symbol these includes members of all base types and implemented interfaces.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns>All found member symbols.</returns>
        public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol typeSymbol) =>
            typeSymbol.GetBaseTypesAndThis()
                .Concat(typeSymbol.AllInterfaces)
                .SelectMany(symbol => symbol.GetMembers());

        /// <summary>
        /// Gets all members of this type symbol that have a particular name, these includes members of all base types and implemented interfaces.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <param name="name">The name to filer for.</param>
        /// <returns>All found member symbols.</returns>
        public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol typeSymbol, string name) =>
            typeSymbol.GetBaseTypesAndThis()
                .Concat(typeSymbol.AllInterfaces)
                .SelectMany(symbol => symbol.GetMembers(name));

        /// <summary>
        /// Get all relevant member for twizzar.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns></returns>
        public static IEnumerable<ISymbol> GetTwizzarRelevantMembers(this ITypeSymbol typeSymbol) =>
            typeSymbol.GetMembers()
                .Where(
                    symbol => symbol is IPropertySymbol or IFieldSymbol or IMethodSymbol or IParameterSymbol);

        /// <summary>
        /// Gets the unique name of the symbol this is always the name except for methods there it is name + method parameter types.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns></returns>
        public static string GetUniqueName(this ISymbol typeSymbol) =>
            typeSymbol switch
            {
                IParameterSymbol x => x.Name,
                IPropertySymbol x => x.Name,
                IFieldSymbol x => x.Name,
                IMethodSymbol { MethodKind: Microsoft.CodeAnalysis.MethodKind.Constructor } => "Ctor",
                IMethodSymbol x => x.Name + GetMethodPostfix(x),
                _ => typeSymbol.Name,
            };

        private static string GetMethodPostfix(IMethodSymbol methodSymbol)
        {
            var parameterString = GetParameterString(methodSymbol);

            return string.IsNullOrEmpty(parameterString)
                ? string.Empty
                : $"__{parameterString}";
        }

        private static string GetParameterString(IMethodSymbol methodSymbol) =>
            methodSymbol.Parameters
                .Select(symbol => symbol.Type.Name)
                .ToDisplayString("_")
                .ToSourceVariableCodeFriendly();

        #endregion
    }
}