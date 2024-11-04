using System;
using System.Collections.Immutable;
using System.Reflection;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;

namespace Twizzar.Runtime.Core.FixtureItem.Description
{
    /// <summary>
    /// The field description build with reflection. For more infos see <see cref="IFieldDescription"/>.
    /// </summary>
    public sealed class ReflectionFieldDescription : FieldDescription, IRuntimeFieldDescription
    {
        #region fields

        private readonly FieldInfo _info;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionFieldDescription"/> class.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="baseTypeService"></param>
        public ReflectionFieldDescription(FieldInfo info, IBaseTypeService baseTypeService)
            : base(baseTypeService)
        {
            EnsureHelper.GetDefault.Parameter(info, nameof(info))
                .ThrowWhenNull();

            this._info = info;
            this.TypeFullName = CoreInterfaces.FixtureItem.Name.TypeFullName.Create(info.FieldType);
            this.DeclaringType = CoreInterfaces.FixtureItem.Name.TypeFullName.Create(info.DeclaringType);
            this.IsConstant = info.IsLiteral && !info.IsInitOnly;

            this.ConstantValue = this.IsConstant
                ? Some(info.GetRawConstantValue())
                : None();

            this.Name = info.Name;
            this.Type = info.FieldType;
            this.IsStatic = info.IsStatic;

            this.Type = info.FieldType;
            this.IsClass = this.Type.IsClass;
            this.IsInterface = this.Type.IsInterface;
            this.IsEnum = this.Type.IsEnum;
            this.IsStruct = this.Type.IsStruct();
            this.IsArray = this.Type.IsArray;
            this.ArrayDimension = this.Type.GetArrayDimension().ToImmutableArray();

            this.AccessModifier = new AccessModifier(info.IsPrivate, info.IsPublic, info.IsFamily, info.IsAssembly);
            this.IsReadonly = !info.IsLiteral && info.IsInitOnly;
            this.IsTypeParameter = info.FieldType.IsGenericParameter;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public Type Type { get; }

        /// <inheritdoc />
        public override ITypeDescription DeclaredDescription =>
            new ReflectionTypeDescription(this._info.DeclaringType, this.BaseTypeService);

        #endregion

        #region members

        /// <inheritdoc />
        public override ITypeDescription GetReturnTypeDescription() =>
            new ReflectionTypeDescription(this._info.FieldType, this.BaseTypeService);

        #endregion
    }
}