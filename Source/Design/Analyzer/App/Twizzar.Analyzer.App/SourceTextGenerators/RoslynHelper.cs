using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Shared.Infrastructure.Extension;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.Resources;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Analyzer.SourceTextGenerators
{
    /// <summary>
    /// Static helper method for types in the namespace Microsoft.CodeAnalysis.
    /// </summary>
    public static class RoslynHelper
    {
        #region members

        /// <summary>
        /// Get the access modifier token for a generated class. Only public and public are supported.
        /// And when the type cannot be determined public will be used.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>A result: if valid success else failure.</returns>
        public static IResult<string, Failure> GetAccessModifierToken(ITypeSymbol symbol) =>
            GetAllAccessibility(symbol)
                .OrderBy(accessibility => (int)accessibility)
                .FirstOrNone()
                .ToResult($"Cannot find any AccessModifier on type {symbol.Name}")
                .Bind(s => GetAccessModifierToken(s));

        /// <summary>
        /// Get the parameter type name where generic parameter get replace by object or TzAnyStruct.
        /// </summary>
        /// <param name="parameterDescription"></param>
        /// <param name="compilation"></param>
        /// <returns></returns>
        public static IResult<string, Failure> GetParameterTypeName(
            IParameterDescription parameterDescription,
            Compilation compilation)
        {
            var type = (IRoslynTypeDescription)parameterDescription.GetReturnTypeDescription();
            var symbol = type.GetTypeSymbol();

            return GetParameterTypeName(symbol, compilation);
        }

        /// <summary>
        /// Get the parameter type name where generic parameter get mapped accordingly to the genericParameterMapping.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="genericParameterMapping"></param>
        /// <param name="usedGenericParameter"></param>
        /// <returns></returns>
        public static string GetTypeNameWithMappedGenericParameters(
            IBaseDescription description,
            IReadOnlyDictionary<string, string> genericParameterMapping,
            HashSet<string> usedGenericParameter = null)
        {
            var type = (IRoslynTypeDescription)description.GetReturnTypeDescription();
            var symbol = type.GetTypeSymbol();

            usedGenericParameter ??= new HashSet<string>();

            return GetTypeNameWithMappedGenericParametersInternal(symbol, genericParameterMapping, usedGenericParameter);
        }

        /// <summary>
        /// Get the parameter type name where generic parameter get mapped accordingly to the genericParameterMapping.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="genericParameterMapping"></param>
        /// <param name="usedGenericParameter"></param>
        /// <returns></returns>
        public static string GetTypeNameWithMappedGenericParameters(
            ITypeSymbol symbol,
            IReadOnlyDictionary<string, string> genericParameterMapping,
            HashSet<string> usedGenericParameter = null)
        {
            usedGenericParameter ??= new HashSet<string>();
            return GetTypeNameWithMappedGenericParametersInternal(symbol, genericParameterMapping, usedGenericParameter);
        }

        private static IResult<string, Failure> GetParameterTypeName(ITypeSymbol symbol, Compilation compilation) =>
            symbol switch
            {
                ITypeParameterSymbol =>
                    Result.Success<string, Failure>("object"),

                IArrayTypeSymbol { ElementType: INamedTypeSymbol { IsGenericType: true } elementType } =>
                    GetParameterTypeName(elementType, compilation).MapSuccess(s => $"{s}[]"),

                IArrayTypeSymbol { ElementType: ITypeParameterSymbol elementType } =>
                    GetParameterTypeName(elementType, compilation).MapSuccess(s => $"{s}[]"),

                INamedTypeSymbol { IsGenericType: true } typeSymbol =>
                    GetGenericTypeParameterNames(compilation, typeSymbol)
                        .MapSuccess(typeArguments => typeSymbol.ConstructedFrom.Construct(typeArguments))
                        .MapSuccess(
                            constructedType => constructedType.GetTypeFullName().GetFriendlyCSharpTypeFullName()),

                _ => Result.Success<string, Failure>(symbol.GetTypeFullName().GetFriendlyCSharpTypeFullName()),
            };

        private static string GetTypeNameWithMappedGenericParametersInternal(
            ITypeSymbol symbol,
            IReadOnlyDictionary<string, string> genericParameterMapping,
            ICollection<string> usedGenericParameter)
        {
            switch (symbol)
            {
                case ITypeParameterSymbol x:
                    usedGenericParameter.AddIfNotExists(genericParameterMapping[x.Name]);
                    return genericParameterMapping[x.Name];
                case INamedTypeSymbol { IsGenericType: true } x:
                    return x.GetTypeFullName().GetFriendlyTypeFullNameWithoutGenerics() +
                           "<" +
                           x.TypeArguments.Select(argument =>
                                   GetTypeNameWithMappedGenericParametersInternal(argument, genericParameterMapping, usedGenericParameter))
                               .ToCommaSeparated() +
                           ">";
                case IArrayTypeSymbol { ElementType: INamedTypeSymbol { IsGenericType: true } elementType }:
                    return $"{GetTypeNameWithMappedGenericParametersInternal(elementType, genericParameterMapping, usedGenericParameter)}[]";
                case IArrayTypeSymbol { ElementType: ITypeParameterSymbol elementType }:
                    return $"{GetTypeNameWithMappedGenericParametersInternal(elementType, genericParameterMapping, usedGenericParameter)}[]";
                default:
                    return symbol.GetTypeFullName().GetFriendlyCSharpTypeFullName();
            }
        }

        private static Result<ITypeSymbol[], Failure> GetGenericTypeParameterNames(
            Compilation compilation,
            INamedTypeSymbol typeSymbol)
        {
            var typeArguments = new ITypeSymbol[typeSymbol.TypeParameters.Length];

            for (var i = 0; i < typeSymbol.TypeParameters.Length; i++)
            {
                var parameter = typeSymbol.TypeParameters[i];

                switch (parameter)
                {
                    case { HasValueTypeConstraint: true }:
                        typeArguments[i] = compilation.GetTypeByMetadataName("Twizzar.Fixture.TzAnyStruct");
                        break;
                    case { ConstraintTypes.Length: 1 }:
                        typeArguments[i] = parameter.ConstraintTypes.First();
                        break;
                    case { ConstraintTypes.Length: > 1 }:
                        return new Failure("Multi constrains are not supported");
                    default:
                        typeArguments[i] = compilation.ObjectType;
                        break;
                }
            }

            return typeArguments;
        }

        private static IEnumerable<Accessibility> GetAllAccessibility(ITypeSymbol symbol) =>
            symbol switch
            {
                INamedTypeSymbol { IsGenericType: true } namedTypeSymbol =>
                    namedTypeSymbol.TypeArguments
                        .SelectMany(GetAllAccessibility)
                        .Append(symbol.DeclaredAccessibility),
                _ => Enumerable.Empty<Accessibility>().Append(symbol.DeclaredAccessibility),
            };

        private static Result<string, Failure> GetAccessModifierToken(Accessibility accessibility) =>
            accessibility switch
            {
                Accessibility.Internal => Result.Success("internal"),
                Accessibility.Public => Result.Success("public"),
                Accessibility.NotApplicable => Result.Success("public"),
                _ => Result.Failure(
                    new Failure(string.Format(
                        ErrorMessages._0__is_not_supported__Only_public_and_internal_types_are_supported,
                        accessibility))),
            };

        #endregion
    }
}