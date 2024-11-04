using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.Functional;
using ViCommon.Functional.Monads.ResultMonad;
using static ViCommon.Functional.Monads.ResultMonad.Result;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <inheritdoc cref="ITypeSymbolQuery" />
    public class TypeSymbolQuery : ITypeSymbolQuery
    {
        /// <inheritdoc/>
        public IResult<ITypeSymbol, Failure> GetTypeSymbol(
            Compilation compilation,
            ITypeFullName typeFullName) =>
                GetTypeSymbolInternal(compilation, typeFullName);

        private static IResult<ITypeSymbol, Failure> GetTypeSymbolInternal(Compilation compilation, ITypeFullName typeFullName)
        {
            var tFullName = typeFullName.Cast();

            if (tFullName.IsArray())
            {
                return GetArrayTypeSymbol(compilation, tFullName);
            }

            var typeSymbol = compilation.GetTypeByMetadataName(tFullName.GetTypeFullNameWithoutGenerics());
            if (typeSymbol == null)
            {
                return Failure<ITypeSymbol, Failure>(
                    new Failure($"Cannot find type {tFullName.FullName} in the compilation {compilation}"));
            }

            // When the type symbol is generic then construct the real type.
            // The typeSymbol System.Tuple´2 will be constructed to System.Tuple´2[[System.Int32, ca], [System.String, ca]] for example where ca is the containing assembly.
            if (typeSymbol.IsGenericType)
            {
                var typeArgumentSymbols = CreateNamedTypeSymbols(compilation, typeSymbol, typeFullName);

                return typeArgumentSymbols.MapSuccess(
                    symbols => typeSymbol.Construct(symbols.ToArray()) as ITypeSymbol);
            }
            else
            {
                return Success<ITypeSymbol, Failure>(typeSymbol);
            }
        }

        private static IResult<ITypeSymbol, Failure> GetArrayTypeSymbol(Compilation compilation, TypeFullName typeFullName)
        {
            ITypeSymbol arrayElementTypeSymbol = compilation.GetTypeByMetadataName(
                typeFullName.ArrayElementType().Match(
                    some => some.GetTypeFullNameWithoutGenerics(),
                    () => throw new InternalException("Array type but invalid TypeFullName")));

            if (arrayElementTypeSymbol is null)
            {
                return Failure<ITypeSymbol, Failure>(
                    new Failure($"Cannot resolve array element type symbol {typeFullName.ArrayElementType()} in the compilation {compilation}"));
            }

            IArrayTypeSymbol arraySymbol = null;
            foreach (var dim in typeFullName.ArrayDimension().Reverse())
            {
                arraySymbol = compilation.CreateArrayTypeSymbol(arrayElementTypeSymbol, dim);
                arrayElementTypeSymbol = arraySymbol;
            }

            return Success<ITypeSymbol, Failure>(arraySymbol);
        }

        // Create names type symbols for the give type ful name
        // For example for Tuple<int, List<string>> this method will create a int type symbol and recursively creates a string type symbol and constructs with it a List<sting> type symbol.
        private static IResult<IEnumerable<ITypeSymbol>, Failure> CreateNamedTypeSymbols(
            Compilation compilation,
            INamedTypeSymbol namedTypeSymbol,
            ITypeFullName typeFullName)
        {
            var result = new List<ITypeSymbol>();

            if (namedTypeSymbol.IsGenericType)
            {
                var names = typeFullName.GenericTypeArguments();

                foreach (var genericTypeArgument in names)
                {
                    var entry = GetTypeSymbolInternal(compilation, genericTypeArgument);
                    switch (entry.AsResultValue())
                    {
                        case FailureValue<Failure> failureValue:
                            return Failure<IEnumerable<ITypeSymbol>, Failure>(failureValue.Value);
                        case SuccessValue<ITypeSymbol> successValue:
                            result.Add(successValue.Value);
                            break;
                        default:
                            throw PatternErrorBuilder.PatternCase(nameof(entry.AsResultValue))
                                .IsNotOneOf(nameof(FailureValue<Failure>), nameof(SuccessValue<ITypeSymbol>));
                    }
                }
            }

            return Success<IEnumerable<ITypeSymbol>, Failure>(result);
        }
    }
}