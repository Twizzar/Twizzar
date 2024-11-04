using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.CoreInterfaces.FixtureItem.Description;
using Twizzar.Design.Shared.Infrastructure.Extension;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;

namespace Twizzar.Design.Shared.Infrastructure.Description
{
    /// <summary>
    /// The property description for roslyn for more information see <see cref="IPropertyDescription"/>.
    /// </summary>
    public class RoslynPropertyDescription : PropertyDescription, IDesignPropertyDescription, IRoslynBaseDescription
    {
        #region fields

        private readonly IPropertySymbol _propertySymbol;
        private readonly IRoslynDescriptionFactory _roslynDescriptionFactory;
        private readonly Lazy<ITypeDescription> _typeDescription;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynPropertyDescription"/> class.
        /// </summary>
        /// <param name="propertySymbol"></param>
        /// <param name="baseTypeService"></param>
        /// <param name="roslynDescriptionFactory"></param>
        public RoslynPropertyDescription(
            IPropertySymbol propertySymbol,
            IBaseTypeService baseTypeService,
            IRoslynDescriptionFactory roslynDescriptionFactory)
            : base(baseTypeService)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(propertySymbol, nameof(propertySymbol))
                .Parameter(baseTypeService, nameof(baseTypeService))
                .Parameter(roslynDescriptionFactory, nameof(roslynDescriptionFactory))
                .ThrowWhenNull();

            this._propertySymbol = propertySymbol;
            this._roslynDescriptionFactory = roslynDescriptionFactory;
            this.Name = propertySymbol.Name;
            this.TypeFullName = propertySymbol.Type.GetTypeFullName();
            this.AccessModifier = propertySymbol.GetAccessModifier();
            this.CanRead = propertySymbol.GetMethod != null;
            this.CanWrite = propertySymbol.SetMethod != null;
            this.IsStatic = propertySymbol.IsStatic;
            this.OverrideKind = OverrideKind.Create(propertySymbol.IsVirtual, propertySymbol.IsSealed);

            this.IsClass = propertySymbol.Type.TypeKind == TypeKind.Class;
            this.IsInterface = propertySymbol.Type.TypeKind == TypeKind.Interface;
            this.IsEnum = propertySymbol.Type.TypeKind == TypeKind.Enum;
            this.IsStruct = propertySymbol.Type.TypeKind == TypeKind.Struct;
            this.IsArray = propertySymbol.Type.TypeKind == TypeKind.Array;
            this.ArrayDimension = propertySymbol.Type.GetArrayDimension().ToImmutableArray();
            this.IsTypeParameter = propertySymbol.Type is ITypeParameterSymbol;

            this.SetMethod = ToMaybe(propertySymbol.SetMethod)
                .Map(symbol => new RoslynMethodDescription(symbol, false, baseTypeService, roslynDescriptionFactory))
                .Map(description => (IMethodDescription)description);

            this.GetMethod = ToMaybe(propertySymbol.GetMethod)
                .Map(symbol => new RoslynMethodDescription(symbol, false, baseTypeService, roslynDescriptionFactory))
                .Map(description => (IMethodDescription)description);

            this._typeDescription = new Lazy<ITypeDescription>(() =>
                roslynDescriptionFactory.CreateDescription(this._propertySymbol.Type));

            this.IsAutoProperty = propertySymbol.ContainingType
                .GetMembers($"<{propertySymbol.Name}>k__BackingField")
                .OfType<IFieldSymbol>()
                .Any();
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public override ITypeDescription DeclaredDescription =>
            this._roslynDescriptionFactory.CreateDescription(this._propertySymbol.ContainingType);

        /// <inheritdoc />
        public override bool IsStatic { get; }

        /// <inheritdoc />
        public bool IsAutoProperty { get; }

        /// <inheritdoc />
        public override Maybe<IMethodDescription> SetMethod { get; }

        /// <inheritdoc />
        public override Maybe<IMethodDescription> GetMethod { get; }

        /// <inheritdoc />
        public override OverrideKind OverrideKind { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public override ITypeDescription GetReturnTypeDescription() => this._typeDescription.Value;

        #endregion

        #region Implementation of IRoslynBaseDescription

        /// <inheritdoc />
        public ISymbol GetSymbol() => this._propertySymbol;

        #endregion
    }
}