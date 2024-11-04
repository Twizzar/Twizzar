using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;

namespace Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators
{
    /// <inheritdoc cref="IUniqueEnumCreator{T}"/>
    public class UniqueEnumCreator<T> : IUniqueEnumCreator<T>
    {
        #region fields

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2743:Static fields should not be used in generic types", Justification = "Ok for static Cache in EnumUniqueCreators.")]
        private static int _currentIndex;
        private readonly T[] _enumValues;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueEnumCreator{T}"/> class.
        /// </summary>
        public UniqueEnumCreator()
        {
            var type = typeof(T);
            this._enumValues = Enum.GetValues(type).Cast<T>().ToArray();
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        [SuppressMessage("Critical Code Smell", "S2696:Instance members should not write to \"static\" fields", Justification = "Ok for static Cache in UniqueCreators")]
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Ok for static Cache in UniqueCreators")]
        private int CurrentIndex
        {
            get => _currentIndex;
            set => _currentIndex = value;
        }

        #endregion

        #region members

        /// <inheritdoc />
        public T GetNextValue() => this._enumValues[this.AdvanceIndexAndReturnCurrent()];

        private int AdvanceIndexAndReturnCurrent()
        {
            var tmp = this.CurrentIndex;
            this.CurrentIndex = (this.CurrentIndex + 1) % this._enumValues.Length;
            return tmp;
        }

        #endregion
    }
}