using System;
using System.Collections.Immutable;
using System.Reflection;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.Core.FixtureItem.Description
{
    /// <summary>
    /// The property description build with reflection. For more infos see <see cref="IPropertyDescription"/>.
    /// </summary>
    public sealed class ReflectionPropertyDescription : PropertyDescription, IRuntimePropertyDescription
    {
        #region fields

        private readonly PropertyInfo _info;
        private readonly Lazy<Maybe<IMethodDescription>> _setMethod;
        private readonly Lazy<Maybe<IMethodDescription>> _getMethod;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionPropertyDescription"/> class.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="baseTypeService"></param>
        public ReflectionPropertyDescription(PropertyInfo info, IBaseTypeService baseTypeService)
            : base(baseTypeService)
        {
            EnsureHelper.GetDefault.Parameter(info, nameof(info)).ThrowWhenNull();

            this._info = info;
            this.Type = info.PropertyType;
            this.TypeFullName = info.PropertyType.ToTypeFullName();

            this._getMethod = new Lazy<Maybe<IMethodDescription>>(() =>
                Maybe.ToMaybe(info.GetMethod)
                    .Map(methodInfo =>
                        (IMethodDescription)new ReflectionMethodDescription(
                            methodInfo,
                            methodInfo.ReturnType,
                            baseTypeService)));

            this._setMethod = new Lazy<Maybe<IMethodDescription>>(() =>
                Maybe.ToMaybe(info.SetMethod)
                    .Map(methodInfo =>
                        (IMethodDescription)new ReflectionMethodDescription(
                            methodInfo,
                            methodInfo.ReturnType,
                            baseTypeService)));

            this.AccessModifier = this.GetMethod
                .BindNone(this.SetMethod)
                .Map(description => description.AccessModifier)
                .SomeOrProvided(() => throw new InternalException("Property has no get method and no set method."));

            this.Name = info.Name;
            this.CanRead = info.CanRead;
            this.CanWrite = info.CanWrite;

            this.IsClass = this.Type.IsClass;
            this.IsInterface = this.Type.IsInterface;
            this.IsEnum = this.Type.IsEnum;
            this.IsStruct = this.Type.IsStruct();
            this.IsArray = this.Type.IsArray;
            this.ArrayDimension = this.Type.GetArrayDimension().ToImmutableArray();
            this.IsTypeParameter = this.Type.IsGenericParameter;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public Type Type { get; }

        /// <inheritdoc />
        public override Maybe<IMethodDescription> SetMethod => this._setMethod.Value;

        /// <inheritdoc />
        public override Maybe<IMethodDescription> GetMethod => this._getMethod.Value;

        /// <inheritdoc />
        public override OverrideKind OverrideKind =>
            this.GetMethod
                .BindNone(this.SetMethod)
                .Map(description => description.OverrideKind)
                .SomeOrProvided(() => throw new InternalException("Property has no get method and no set method."));

        /// <inheritdoc />
        public override ITypeDescription DeclaredDescription =>
            new ReflectionTypeDescription(this._info.DeclaringType, this.BaseTypeService);

        /// <inheritdoc />
        public override bool IsStatic =>
            this.GetMethod
                .BindNone(this.SetMethod)
                .Map(description => description.IsStatic)
                .SomeOrProvided(() => throw new InternalException("Property has no get method and no set method."));

        #endregion

        #region members

        /// <inheritdoc />
        public override ITypeDescription GetReturnTypeDescription() =>
            new ReflectionTypeDescription(this._info.PropertyType, this.BaseTypeService);

        #endregion
    }
}