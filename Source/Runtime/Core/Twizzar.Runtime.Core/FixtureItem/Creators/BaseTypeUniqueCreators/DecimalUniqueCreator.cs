using System;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;

namespace Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators
{
    /// <summary>
    /// Implements the <see cref="IUniqueCreator{T}"/> for <see cref="decimal"/>.
    /// </summary>
    public class DecimalUniqueCreator : IUniqueCreator<decimal>
    {
        private const byte Scale = 3;
        private readonly IUniqueCreator<int> _intBaseTypeCreator;
        private bool _isNegative;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalUniqueCreator"/> class.
        /// </summary>
        public DecimalUniqueCreator()
        {
            this._intBaseTypeCreator = new BitSequenceBased.IntUniqueCreator();
        }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region Implementation of IUniqueCreator

        /// <inheritdoc />
        public decimal GetNextValue()
        {
            this._isNegative = !this._isNegative;
            var lo = this._intBaseTypeCreator.GetNextValue();

            try
            {
                return new decimal(lo, 0, 0, this._isNegative, Scale);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                this.Logger?.Log(LogLevel.Warn, exception.Message);
                return this.GetNextValue();
            }
        }

        #endregion
    }
}