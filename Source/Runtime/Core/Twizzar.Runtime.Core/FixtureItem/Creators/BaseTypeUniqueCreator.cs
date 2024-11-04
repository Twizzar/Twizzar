using System;
using System.Data;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Runtime.Core.FixtureItem.Creators
{
    /// <summary>
    /// Class implements <see cref="IBaseTypeUniqueCreator"/> and <see cref="IHasLogger"/>.
    /// </summary>
    public class BaseTypeUniqueCreator : IBaseTypeUniqueCreator
    {
        #region fields

        private readonly IUniqueCreatorProvider _uniqueCreatorProvider;

        private readonly DefaultDictionary<Type, object>
            _generators = new(type => null);

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTypeUniqueCreator"/> class.
        /// </summary>
        /// <param name="uniqueCreatorProvider">The base type generator factory.</param>
        public BaseTypeUniqueCreator(IUniqueCreatorProvider uniqueCreatorProvider)
        {
            this.EnsureParameter(uniqueCreatorProvider, nameof(uniqueCreatorProvider)).ThrowWhenNull();
            this._uniqueCreatorProvider = uniqueCreatorProvider;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public object GetNextValue(Type type)
        {
            type = type.IsNullableType()
                ? Nullable.GetUnderlyingType(type)
                : type;

            Func<object> genericFunc = this.GetNextValue<object>;

            return genericFunc
                .Method
                .GetGenericMethodDefinition()
                .MakeGenericMethod(type)
                .Invoke(this, null);
        }

        private T GetNextValue<T>()
        {
            var type = typeof(T);

            this._generators[type] ??= this._uniqueCreatorProvider.GetUniqueCreator<T>();

            if (!(this._generators[type] is IUniqueCreator<T> creatorT))
            {
                throw new InvalidConstraintException();
            }

            return creatorT.GetNextValue();
        }

        #endregion
    }
}