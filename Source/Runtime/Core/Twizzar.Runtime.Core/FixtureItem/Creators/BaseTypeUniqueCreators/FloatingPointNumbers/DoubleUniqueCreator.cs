using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Twizzar.SharedKernel.NLog.Interfaces;

namespace Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators.FloatingPointNumbers
{
    /// <summary>
    /// The generator for the <see cref="double"/> type using floating point generator <see cref="FloatingPointUniqueCreator{T}"/>.
    /// </summary>
    public class DoubleUniqueCreator : FloatingPointUniqueCreator<double>
    {
        #region static fields and constants

        private static byte[] _uniqueValue = new byte[sizeof(double)];
        private static int _currentPartitionIndex;
        private static BitArray[] _partitions = new BitArray[sizeof(double)];

        #endregion

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleUniqueCreator"/> class.
        /// </summary>
        public DoubleUniqueCreator()
            : base(52, 11, 25, 4, 3)
        {
        }

        #endregion

        #region properties

        /// <inheritdoc />
        [SuppressMessage("Critical Code Smell", "S2696:Instance members should not write to \"static\" fields", Justification = "Ok for static Cache in UniqueCreators")]
        protected override BitArray[] Partitions
        {
            get => _partitions;
            set => _partitions = value;
        }

        /// <inheritdoc />
        [SuppressMessage("Critical Code Smell", "S2696:Instance members should not write to \"static\" fields", Justification = "Ok for static Cache in UniqueCreators")]
        protected override byte[] UniqueValue
        {
            get => _uniqueValue;
            set => _uniqueValue = value;
        }

        /// <inheritdoc />
        [SuppressMessage("Critical Code Smell", "S2696:Instance members should not write to \"static\" fields", Justification = "Ok for static Cache in UniqueCreators")]
        protected override int CurrentPartitionIndex
        {
            get => _currentPartitionIndex;
            set => _currentPartitionIndex = value;
        }

        /// <inheritdoc />
        protected override BitArray CurrentPartition => this.Partitions[this.CurrentPartitionIndex];

        /// <inheritdoc />
        protected override int TotalBits => sizeof(double) * BitsOfByte;

        /// <inheritdoc />
        protected override int PartitionSize => sizeof(double);

        #endregion

        #region Overrides of FloatingPointUniqueCreator<double>

        /// <inheritdoc />
        protected override double ConvertByteArrayToT()
        {
            try
            {
                var value = BitConverter.ToDouble(this.UniqueValue, 0);

                return !double.IsNaN(value) && Math.Abs(value) > double.Epsilon ?
                    value :
                    this.GetNextValue();
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