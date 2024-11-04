using System;
using System.Collections.Generic;

using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.SharedKernel.Core.FixtureItem.Description.Services
{
    /// <summary>
    /// Implements the <see cref="IBaseTypeService"/> interface.
    /// </summary>
    public class BaseTypeService : IBaseTypeService
    {
        #region fields

        private Dictionary<string, BaseTypeKind> _baseTypes;
        private HashSet<Type> _baseTypeSet;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTypeService"/> class.
        /// </summary>
        public BaseTypeService()
        {
            this.InitializeBaseTypes();
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public IEnumerable<Type> BaseTypes => this._baseTypeSet;

        #endregion

        #region members

        /// <inheritdoc />
        public bool IsBaseType(ITypeFullName typeFullName)
        {
            this.EnsureParameter(typeFullName, nameof(typeFullName)).ThrowWhenNull();

            return this._baseTypes.ContainsKey(typeFullName.FullName);
        }

        /// <inheritdoc />
        public bool IsNullableBaseType(ITypeFullName typeFullName)
        {
            this.EnsureParameter(typeFullName, nameof(typeFullName)).ThrowWhenNull();

            return typeFullName.NullableGetUnderlyingType().AsMaybeValue() is SomeValue<ITypeFullName> underlyingType &&
                   this.IsBaseType(underlyingType.Value);
        }

        /// <inheritdoc />
        public BaseTypeKind GetKind(IBaseDescription baseDescription)
        {
            if (baseDescription.GetReturnTypeDescription().IsEnum)
            {
                return BaseTypeKind.Enum;
            }

            return baseDescription.TypeFullName.NullableGetUnderlyingType().AsMaybeValue() is SomeValue<ITypeFullName> underlyingType
                ? this._baseTypes.GetMaybe(underlyingType.Value.FullName).SomeOrProvided(BaseTypeKind.Complex)
                : this._baseTypes.GetMaybe(baseDescription.TypeFullName.FullName).SomeOrProvided(BaseTypeKind.Complex);
        }

        private void InitializeBaseTypes()
        {
            this._baseTypes = new Dictionary<string, BaseTypeKind>();
            this._baseTypeSet = new HashSet<Type>();

            // numeric base types
            this.InitializeBaseType<int>(BaseTypeKind.Number);
            this.InitializeBaseType<uint>(BaseTypeKind.Number);
            this.InitializeBaseType<long>(BaseTypeKind.Number);
            this.InitializeBaseType<ulong>(BaseTypeKind.Number);
            this.InitializeBaseType<short>(BaseTypeKind.Number);
            this.InitializeBaseType<ushort>(BaseTypeKind.Number);
            this.InitializeBaseType<byte>(BaseTypeKind.Number);
            this.InitializeBaseType<sbyte>(BaseTypeKind.Number);

            this.InitializeBaseType<decimal>(BaseTypeKind.Number);
            this.InitializeBaseType<float>(BaseTypeKind.Number);
            this.InitializeBaseType<double>(BaseTypeKind.Number);

            // others
            this.InitializeBaseType<char>(BaseTypeKind.Char);
            this.InitializeBaseType<string>(BaseTypeKind.String);
            this.InitializeBaseType<bool>(BaseTypeKind.Boolean);
        }

        private void InitializeBaseType<T>(BaseTypeKind kind)
        {
            this._baseTypes.Add(typeof(T).FullName, kind);
            this._baseTypeSet.Add(typeof(T));
        }

        #endregion
    }
}