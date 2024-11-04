using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.FixtureItem.Description;
using Twizzar.Design.Shared.Infrastructure.Extension;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using MethodKind = Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums.MethodKind;

namespace Twizzar.Design.Shared.Infrastructure.Description
{
    /// <summary>
    /// Method description for roslyn. For mor information see <see cref="IMethodDescription"/>.
    /// </summary>
    public class RoslynMethodDescription : MethodDescription, IDesignMethodDescription, IRoslynBaseDescription
    {
        #region fields

        private readonly IMethodSymbol _methodSymbol;
        private readonly IRoslynDescriptionFactory _roslynDescriptionFactory;
        private readonly Lazy<ITypeDescription> _typeDescription;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynMethodDescription"/> class.
        /// </summary>
        /// <param name="methodSymbol"></param>
        /// <param name="isConstructor"></param>
        /// <param name="baseTypeService"></param>
        /// <param name="roslynDescriptionFactory"></param>
        public RoslynMethodDescription(
            IMethodSymbol methodSymbol,
            bool isConstructor,
            IBaseTypeService baseTypeService,
            IRoslynDescriptionFactory roslynDescriptionFactory)
            : base(baseTypeService)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(methodSymbol, nameof(methodSymbol))
                .Parameter(baseTypeService, nameof(baseTypeService))
                .Parameter(roslynDescriptionFactory, nameof(roslynDescriptionFactory))
                .ThrowWhenNull();

            this._methodSymbol = methodSymbol;
            this._roslynDescriptionFactory = roslynDescriptionFactory;
            this.Name = methodSymbol.Name;
            this.TypeFullName = methodSymbol.ReturnType.GetTypeFullName();
            this.AccessModifier = methodSymbol.GetAccessModifier();
            this.IsConstructor = isConstructor;
            this.IsAbstract = methodSymbol.IsAbstract;
            this.IsStatic = methodSymbol.IsStatic;
            this.IsGeneric = methodSymbol.IsGenericMethod;
            this.OverrideKind = OverrideKind.Create(methodSymbol.IsVirtual);
            this.GenericTypeArguments = methodSymbol.TypeArguments.MapGenericArguments();

            this.IsClass = methodSymbol.ReturnType.TypeKind == TypeKind.Class;
            this.IsInterface = methodSymbol.ReturnType.TypeKind == TypeKind.Interface;
            this.IsEnum = methodSymbol.ReturnType.TypeKind == TypeKind.Enum;
            this.IsStruct = methodSymbol.ReturnType.TypeKind == TypeKind.Struct;
            this.IsArray = methodSymbol.ReturnType.TypeKind == TypeKind.Array;
            this.ArrayDimension = methodSymbol.ReturnType.GetArrayDimension().ToImmutableArray();
            this.IsTypeParameter = methodSymbol.ReturnType is ITypeParameterSymbol;

            this.MethodKind = this._methodSymbol.MethodKind switch
            {
                Microsoft.CodeAnalysis.MethodKind.Constructor => MethodKind.Constructor,
                Microsoft.CodeAnalysis.MethodKind.Ordinary => MethodKind.Ordinary,
                Microsoft.CodeAnalysis.MethodKind.PropertyGet => MethodKind.Property,
                Microsoft.CodeAnalysis.MethodKind.PropertySet => MethodKind.Property,
                _ => MethodKind.Other,
            };

            this.IsArray = methodSymbol.ReturnType.TypeKind == TypeKind.Array;

            this.DeclaredParameters =
                methodSymbol.Parameters
                    .Select(
                        symbol => (IParameterDescription)new RoslynParameterDescription(
                            symbol,
                            baseTypeService,
                            roslynDescriptionFactory))
                    .ToImmutableArray();

            this._typeDescription = new Lazy<ITypeDescription>(
                () =>
                    roslynDescriptionFactory.CreateDescription(this._methodSymbol.ReturnType));
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public override ImmutableArray<IParameterDescription> DeclaredParameters { get; }

        /// <inheritdoc />
        public override ITypeDescription DeclaredDescription =>
            this._roslynDescriptionFactory.CreateDescription(this._methodSymbol.ContainingType);

        /// <inheritdoc/>
        public string FriendlyParameterTypes =>
            this.DeclaredParameters
                .Select(description => description.TypeFullName)
                .Select(fullName => fullName.GetFriendlyCSharpTypeName())
                .ToCommaSeparated();

        #endregion

        #region members

        /// <inheritdoc />
        public override ITypeDescription GetReturnTypeDescription() => this._typeDescription.Value;

        #endregion

        #region Implementation of IRoslynBaseDescription

        /// <inheritdoc />
        public ISymbol GetSymbol() => this._methodSymbol;

        #endregion
    }
}