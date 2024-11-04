using System;
using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.Infrastructure.Extension;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Shared.Infrastructure.Description
{
    /// <summary>
    /// The parameter description for roslyn for more information see <see cref="IParameterDescription"/>.
    /// </summary>
    public class RoslynParameterDescription : ParameterDescription, IRoslynBaseDescription
    {
        private readonly IParameterSymbol _parameterSymbol;
        private readonly Lazy<ITypeDescription> _typeDescription;

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynParameterDescription"/> class.
        /// </summary>
        /// <param name="parameterSymbol"></param>
        /// <param name="baseTypeService"></param>
        /// <param name="roslynDescriptionFactory"></param>
        public RoslynParameterDescription(IParameterSymbol parameterSymbol, IBaseTypeService baseTypeService, IRoslynDescriptionFactory roslynDescriptionFactory)
            : base(baseTypeService)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(parameterSymbol, nameof(parameterSymbol))
                .Parameter(baseTypeService, nameof(baseTypeService))
                .Parameter(roslynDescriptionFactory, nameof(roslynDescriptionFactory))
                .ThrowWhenNull();

            this._parameterSymbol = parameterSymbol;
            this.Name = parameterSymbol.Name;
            this.TypeFullName = parameterSymbol.Type.GetTypeFullName();
            this.DefaultValue = parameterSymbol.HasExplicitDefaultValue
                ? new ParameterDefaultValue(parameterSymbol.ExplicitDefaultValue)
                : Maybe.None<ParameterDefaultValue>();
            this.IsIn = parameterSymbol.RefKind == RefKind.In;
            this.IsOut = parameterSymbol.RefKind == RefKind.Out;
            this.Position = parameterSymbol.Ordinal;
            this.IsOptional = parameterSymbol.IsOptional;
            this.AccessModifier = new AccessModifier(false, false, false, false);
            this._typeDescription = new Lazy<ITypeDescription>(
                () => roslynDescriptionFactory.CreateDescription(this._parameterSymbol.Type));

            this.IsClass = parameterSymbol.Type.TypeKind == TypeKind.Class;
            this.IsInterface = parameterSymbol.Type.TypeKind == TypeKind.Interface;
            this.IsEnum = parameterSymbol.Type.TypeKind == TypeKind.Enum;
            this.IsStruct = parameterSymbol.Type.TypeKind == TypeKind.Struct;
            this.IsArray = parameterSymbol.Type.TypeKind == TypeKind.Array;
            this.IsTypeParameter = parameterSymbol.Type is ITypeParameterSymbol;
        }

        #endregion

        #region members

        /// <inheritdoc />
        public override ITypeDescription GetReturnTypeDescription() =>
            this._typeDescription.Value;
        #endregion

        #region Implementation of IRoslynBaseDescription

        /// <inheritdoc />
        public ISymbol GetSymbol() => this._parameterSymbol;

        #endregion
    }
}