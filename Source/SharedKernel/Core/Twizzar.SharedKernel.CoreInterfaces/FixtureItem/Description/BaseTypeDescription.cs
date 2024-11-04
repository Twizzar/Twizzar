using System.Collections.Immutable;
using System.Linq;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description
{
    /// <inheritdoc cref="IBaseDescription" />
    public abstract class BaseTypeDescription : ValueObject, IBaseDescription
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTypeDescription"/> class.
        /// </summary>
        /// <param name="baseTypeService"></param>
        protected BaseTypeDescription(IBaseTypeService baseTypeService)
        {
            EnsureHelper.GetDefault.Parameter(baseTypeService, nameof(baseTypeService)).ThrowWhenNull();
            this.BaseTypeService = baseTypeService;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ITypeFullName TypeFullName { get; protected set; }

        /// <inheritdoc />
        public AccessModifier AccessModifier { get; protected set; }

        /// <inheritdoc />
        public bool IsClass { get; protected set; }

        /// <inheritdoc />
        public bool IsInterface { get; protected set; }

        /// <inheritdoc />
        public bool IsEnum { get; protected set; }

        /// <inheritdoc />
        public bool IsStruct { get; protected set; }

        /// <inheritdoc />
        public bool IsArray { get; protected set; }

        /// <inheritdoc />
        public ImmutableArray<int> ArrayDimension { get; protected set; }

        /// <inheritdoc />
        public bool IsBaseType =>
            this.IsEnum || this.BaseTypeService.IsBaseType(this.TypeFullName);

        /// <inheritdoc />
        public bool IsNullableBaseType =>
            this.BaseTypeService.IsNullableBaseType(this.TypeFullName) || this.IsNullableEnum;

        /// <inheritdoc />
        public bool IsTypeParameter { get; init; }

        /// <summary>
        /// Gets the baseType service.
        /// </summary>
        protected IBaseTypeService BaseTypeService { get; }

        private bool IsNullableEnum =>
            this.TypeFullName.IsNullable() &&
            this.TypeFullName.GenericTypeArguments().Length == 1 &&
            this.GetReturnTypeDescription().GetGenericTypeArgumentDescription(0).IsEnum;

        #endregion

        #region members

        /// <inheritdoc />
        public abstract ITypeDescription GetReturnTypeDescription();

        /// <inheritdoc />
        public Maybe<string[]> GetEnumNames() =>
            this.IsEnum
                ? Maybe.Some(
                    this.GetReturnTypeDescription()
                    .GetDeclaredFields()
                    .Select(description => description.Name)
                    .OrderBy(s => s)
                    .ToArray())
                : Maybe.None();

        /// <inheritdoc />
        public string GetFriendlyReturnTypeFullName() =>
            this.GetReturnTypeDescription().TypeFullName.GetFriendlyCSharpTypeFullName();

        #endregion
    }
}