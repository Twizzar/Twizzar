using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Twizzar.SharedKernel.NLog.Interfaces;

namespace Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators.FloatingPointNumbers
{
    /// <summary>
    /// The generator for the <see cref="float"/> type using floating point generator <see cref="FloatingPointUniqueCreator{T}"/>.
    /// </summary>
    public class FloatUniqueCreator : FloatingPointUniqueCreator<float>
    {
        #region static fields and constants

        private static byte[] _uniqueValue = new byte[sizeof(float)];
        private static int _currentPartitionIndex;
        private static BitArray[] _partitions = new BitArray[sizeof(float)];

        #endregion

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="FloatUniqueCreator"/> class.
        /// </summary>
        public FloatUniqueCreator()
            : base(23, 8, 23, 3, 4)
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
        protected override int TotalBits => sizeof(float) * BitsOfByte;

        /// <inheritdoc />
        protected override int PartitionSize => sizeof(float);

        #endregion

        #region Overrides of FloatingPointUniqueCreator<float>

        /// <inheritdoc />
        protected override float ConvertByteArrayToT()
        {
            try
            {
                var value = BitConverter.ToSingle(this.UniqueValue, 0);
                return !float.IsNaN(value) && Math.Abs(value) > float.Epsilon ?
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