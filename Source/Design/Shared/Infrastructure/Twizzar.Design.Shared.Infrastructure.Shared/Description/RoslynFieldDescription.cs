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
using ViCommon.Functional.Monads.MaybeMonad;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;

namespace Twizzar.Design.Shared.Infrastructure.Description
{
    /// <summary>
    /// The field description for roslyn for more information see <see cref="IFieldDescription"/>.
    /// </summary>
    public class RoslynFieldDescription : FieldDescription, IDesignFieldDescription, IRoslynBaseDescription
    {
        #region fields

        private readonly IFieldSymbol _fieldSymbol;
        private readonly IRoslynDescriptionFactory _roslynDescriptionFactory;
        private readonly Lazy<ITypeDescription> _typeDescription;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynFieldDescription"/> class.
        /// </summary>
        /// <param name="fieldSymbol"></param>
        /// <param name="baseTypeService"></param>
        /// <param name="roslynDescriptionFactory"></param>
        public RoslynFieldDescription(
            IFieldSymbol fieldSymbol,
            IBaseTypeService baseTypeService,
            IRoslynDescriptionFactory roslynDescriptionFactory)
            : base(baseTypeService)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(fieldSymbol, nameof(fieldSymbol))
                .Parameter(baseTypeService, nameof(baseTypeService))
                .Parameter(roslynDescriptionFactory, nameof(roslynDescriptionFactory))
                .ThrowWhenNull();

            this._fieldSymbol = fieldSymbol;
            this._roslynDescriptionFactory = roslynDescriptionFactory;
            this.Name = fieldSymbol.Name;
            this.TypeFullName = fieldSymbol.Type.GetTypeFullName();

            this.DeclaringType = fieldSymbol.ContainingType.GetTypeFullName();
            this.AccessModifier = fieldSymbol.GetAccessModifier();
            this.IsStatic = fieldSymbol.IsStatic;
            this.IsConstant = fieldSymbol.IsConst;
            this.ConstantValue = fieldSymbol.IsConst ? Some(fieldSymbol.ConstantValue) : None<object>();
            this.IsReadonly = fieldSymbol.IsReadOnly;
            this.IsClass = fieldSymbol.Type.TypeKind == TypeKind.Class;
            this.IsInterface = fieldSymbol.Type.TypeKind == TypeKind.Interface;
            this.IsEnum = fieldSymbol.Type.TypeKind == TypeKind.Enum;
            this.IsStruct = fieldSymbol.Type.TypeKind == TypeKind.Struct;
            this.IsArray = fieldSymbol.Type.TypeKind == TypeKind.Array;
            this.ArrayDimension = fieldSymbol.Type.GetArrayDimension().ToImmutableArray();

            this._typeDescription = new Lazy<ITypeDescription>(() =>
                roslynDescriptionFactory.CreateDescription(this._fieldSymbol.Type));

            this.IsBackingField = fieldSymbol.Name.EndsWith(">k_BackingField");

            this.BackingFieldProperty =
                this.IsBackingField
                    ? fieldSymbol.ContainingType
                        .GetMembers(fieldSymbol.Name
                            .Replace("<", string.Empty)
                            .Replace(">k__BackingField", string.Empty))
                        .OfType<IPropertySymbol>()
                        .Select(symbol => (IPropertyDescription)new RoslynPropertyDescription(
                            symbol,
                            baseTypeService,
                            roslynDescriptionFactory))
                        .FirstOrNone()
                    : None();

            this.IsTypeParameter = this._fieldSymbol.Type is ITypeParameterSymbol;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public bool IsBackingField { get; }

        /// <inheritdoc />
        public Maybe<IPropertyDescription> BackingFieldProperty { get; }

        /// <inheritdoc />
        public override ITypeDescription DeclaredDescription =>
            this._roslynDescriptionFactory.CreateDescription(this._fieldSymbol.ContainingType);

        #endregion

        #region members

        /// <inheritdoc />
        public override ITypeDescription GetReturnTypeDescription() => this._typeDescription.Value;

        #endregion

        #region Implementation of IRoslynBaseDescription

        /// <inheritdoc />
        public ISymbol GetSymbol() => this._fieldSymbol;

        #endregion
    }
}