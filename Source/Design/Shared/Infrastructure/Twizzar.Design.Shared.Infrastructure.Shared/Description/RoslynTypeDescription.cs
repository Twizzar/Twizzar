using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.Infrastructure.Extension;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.SharedKernel.CoreInterfaces.Util;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;

namespace Twizzar.Design.Shared.Infrastructure.Description
{
    /// <summary>
    /// The type description for roslyn for more information see <see cref="ITypeDescription"/>.
    /// </summary>
    public class RoslynTypeDescription : TypeDescription, IRoslynTypeDescription
    {
        #region fields

        private readonly IRoslynDescriptionFactory _roslynDescriptionFactory;

        private readonly Lazy<ImmutableArray<IMethodDescription>> _declaredConstructors;
        private readonly Lazy<ImmutableArray<IFieldDescription>> _declaredFields;
        private readonly Lazy<ImmutableArray<IMethodDescription>> _declaredMethods;
        private readonly Lazy<ImmutableArray<IPropertyDescription>> _declaredProperties;
        private readonly DefaultDictionary<int, ITypeDescription> _genericTypeDescriptionCache;
        private readonly ITypeSymbol _typeSymbol;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynTypeDescription"/> class.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <param name="baseTypeService"></param>
        /// <param name="roslynDescriptionFactory"></param>
        public RoslynTypeDescription(
            ITypeSymbol typeSymbol,
            IBaseTypeService baseTypeService,
            IRoslynDescriptionFactory roslynDescriptionFactory)
            : base(baseTypeService)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(typeSymbol, nameof(typeSymbol))
                .Parameter(baseTypeService, nameof(baseTypeService))
                .Parameter(roslynDescriptionFactory, nameof(roslynDescriptionFactory))
                .ThrowWhenNull();

            this._roslynDescriptionFactory = roslynDescriptionFactory;
            this._typeSymbol = typeSymbol;
            var maybeNamedTypeSymbol = ToMaybe(typeSymbol as INamedTypeSymbol);

            this._declaredConstructors = new Lazy<ImmutableArray<IMethodDescription>>(() =>
                maybeNamedTypeSymbol.Match(
                    namedTypeSymbol => namedTypeSymbol.InstanceConstructors
                        .Select(symbol =>
                            new RoslynMethodDescription(symbol, true, baseTypeService, roslynDescriptionFactory))
                        .Cast<IMethodDescription>()
                        .ToImmutableArray(),
                    ImmutableArray<IMethodDescription>.Empty));

            this._declaredFields = new Lazy<ImmutableArray<IFieldDescription>>(() =>
                GetMemberSymbols<IFieldSymbol>(typeSymbol)
                    .Select(symbol => new RoslynFieldDescription(symbol, baseTypeService, roslynDescriptionFactory))
                    .Cast<IFieldDescription>()
                    .ToImmutableArray());

            this._declaredMethods = new Lazy<ImmutableArray<IMethodDescription>>(() =>
                GetMemberSymbols<IMethodSymbol>(typeSymbol)
                    .Select(symbol =>
                        new RoslynMethodDescription(symbol, false, baseTypeService, roslynDescriptionFactory))
                    .Distinct(new HashEqualityComparer<IMethodDescription>(description =>
                        description.UniqueName.GetHashCode()))
                    .ToImmutableArray());

            this._declaredProperties = new Lazy<ImmutableArray<IPropertyDescription>>(() =>
                GetMemberSymbols<IPropertySymbol>(typeSymbol)
                    .Select(symbol => new RoslynPropertyDescription(symbol, baseTypeService, roslynDescriptionFactory))
                    .Cast<IPropertyDescription>()
                    .ToImmutableArray());

            this.TypeFullName = typeSymbol.GetTypeFullName();
            this.BaseType = ToMaybe(typeSymbol.BaseType).Map(symbol => symbol.GetTypeFullName());
            this.AccessModifier = typeSymbol.GetAccessModifier();

            this.IsClass = this.ValidateTypeKind(TypeKind.Class);
            this.IsInterface = this.ValidateTypeKind(TypeKind.Interface);
            this.IsStruct = this.ValidateTypeKind(TypeKind.Struct);
            this.IsArray = this.ValidateTypeKind(TypeKind.Array);
            this.IsDelegate = this.ValidateTypeKind(TypeKind.Delegate);
            this.IsEnum = this.ValidateTypeKind(TypeKind.Enum);
            this.IsAbstract = this._typeSymbol.IsAbstract;
            this.IsNested = this._typeSymbol.ContainingType != null;
            this.IsSealed = this._typeSymbol.IsSealed;
            this.IsStatic = this._typeSymbol.IsStatic;
            this.IsGeneric = this._typeSymbol is INamedTypeSymbol { IsGenericType: true };
            this.ArrayDimension = this._typeSymbol.GetArrayDimension().ToImmutableArray();
            this.IsTypeParameter = this._typeSymbol is ITypeParameterSymbol;

            this.GenericTypeArguments =
                maybeNamedTypeSymbol.Match(
                    namedTypeSymbol => namedTypeSymbol.TypeArguments.MapGenericArguments(),
                    ImmutableDictionary<int, GenericParameterType>.Empty);

            this.ImplementedInterfaceNames =
                maybeNamedTypeSymbol.Match(
                    namedTypeSymbol =>
                        namedTypeSymbol.AllInterfaces
                            .Select(symbol => symbol.GetTypeFullName().FullName)
                            .ToImmutableArray(),
                    ImmutableArray<string>.Empty);

            this._genericTypeDescriptionCache = new DefaultDictionary<int, ITypeDescription>(i =>
                maybeNamedTypeSymbol.Match(
                    namedTypeSymbol =>
                        this._roslynDescriptionFactory.CreateDescription(
                            namedTypeSymbol.TypeArguments[i] as INamedTypeSymbol),
                    () => throw new InternalException("Cannot get generic information of none NamedTypeSymbols")));
        }

        #endregion

        #region members

        /// <inheritdoc />
        public override ITypeDescription GetReturnTypeDescription() => this;

        /// <inheritdoc />
        public override ImmutableArray<IMethodDescription> GetDeclaredConstructors() =>
            this._declaredConstructors.Value;

        /// <inheritdoc />
        public override ImmutableArray<IFieldDescription> GetDeclaredFields() => this._declaredFields.Value;

        /// <inheritdoc />
        public override ImmutableArray<IMethodDescription> GetDeclaredMethods() => this._declaredMethods.Value;

        /// <inheritdoc />
        public override ImmutableArray<IPropertyDescription> GetDeclaredProperties() => this._declaredProperties.Value;

        /// <inheritdoc />
        public override ITypeDescription GetGenericTypeArgumentDescription(int position) =>
            this._genericTypeDescriptionCache[position];

        /// <inheritdoc />
        public ITypeSymbol GetTypeSymbol() => this._typeSymbol;

        private static IEnumerable<T> GetMemberSymbols<T>(ITypeSymbol typeSymbol)
            where T : ISymbol
        {
            static IEnumerable<T> GetMembers(INamespaceOrTypeSymbol typeSymbol) => typeSymbol.GetMembers().OfType<T>();

            if (typeSymbol is not INamedTypeSymbol)
            {
                return Enumerable.Empty<T>();
            }

            return typeSymbol.GetBaseTypesAndThis()
                .Concat(typeSymbol.AllInterfaces)
                .Select(
                    symbol => GetMembers(symbol)
                        .ToImmutableDictionary(info => GetUniqueName(info), x => x))
                .Aggregate(
                    (IImmutableDictionary<string, T>)ImmutableDictionary.Create<string, T>(),
                    (old, @new) => @new.Merge(old))
                .Values;
        }

        private static string GetUniqueName(ISymbol symbol) =>
            symbol switch
            {
                IMethodSymbol x =>
                    $"{x.ReturnType.GetTypeFullName()} " +
                    x.Name +
                    x.TypeArguments
                        .Select(s => s.Name)
                        .ToDisplayString(", ", "<", ">") +
                    x.Parameters
                        .Select(s => $"{s.ToDisplayString()} {s.Name}")
                        .ToDisplayString(", ", "(", ")"),
                _ => symbol.Name,
            };

        private bool ValidateTypeKind(TypeKind kind) => this._typeSymbol.TypeKind == kind;

        #endregion
    }
}