using System;
using System.Collections.Immutable;
using System.Reflection;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.Core.FixtureItem.Description
{
    /// <summary>
    /// The parameter description build with reflection. For more infos see <see cref="IParameterDescription"/>.
    /// </summary>
    public sealed class ReflectionParameterDescription : ParameterDescription, IRuntimeParameterDescription
    {
        #region fields

        private readonly ParameterInfo _info;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionParameterDescription"/> class.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="baseTypeService"></param>
        public ReflectionParameterDescription(ParameterInfo info, IBaseTypeService baseTypeService)
            : base(baseTypeService)
        {
            EnsureHelper.GetDefault.Parameter(info, nameof(info)).ThrowWhenNull();

            this._info = info;
            this.TypeFullName = info.ParameterType.ToTypeFullName();
            this.AccessModifier = new AccessModifier(false, false, false, false);
            this.Name = info.Name;

            this.Type = info.ParameterType;
            this.IsClass = this.Type.IsClass;
            this.IsInterface = this.Type.IsInterface;
            this.IsEnum = this.Type.IsEnum;
            this.IsStruct = this.Type.IsStruct();
            this.IsArray = this.Type.IsArray;
            this.ArrayDimension = this.Type.GetArrayDimension().ToImmutableArray();

            this.DefaultValue = info.HasDefaultValue
                ? Maybe.Some(new ParameterDefaultValue(info.DefaultValue))
                : Maybe.None();
            this.IsIn = info.IsIn;
            this.IsOptional = info.IsOptional;
            this.IsOut = info.IsOut;
            this.Position = info.Position;
            this.IsTypeParameter = info.ParameterType.IsGenericParameter;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public Type Type { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public override ITypeDescription GetReturnTypeDescription() =>
            new ReflectionTypeDescription(this._info.ParameterType, this.BaseTypeService);

        #endregion
    }
}