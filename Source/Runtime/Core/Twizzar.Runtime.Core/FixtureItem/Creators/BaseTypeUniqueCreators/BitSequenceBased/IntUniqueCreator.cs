using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Twizzar.SharedKernel.NLog.Interfaces;

namespace Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators.BitSequenceBased
{
    /// <summary>
    /// The generator for the <see cref="int"/> type using bit sequence based generator <see cref="BitSequenceBasedUniqueCreator{T}"/>.
    /// </summary>
    public class IntUniqueCreator : BitSequenceBasedUniqueCreator<int>
    {
        #region static fields and constants

        private static byte[] _uniqueValue = new byte[sizeof(int)];
        private static int _currentPartitionIndex;
        private static BitArray[] _partitions = new BitArray[sizeof(int)];

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
        protected override int TotalBits => sizeof(int) * BitsOfByte;

        /// <inheritdoc />
        protected override int PartitionSize => sizeof(int);

        #endregion

        #region members

        /// <inheritdoc />
        protected override int ConvertByteArrayToT()
        {
            try
            {
                var result = BitConverter.ToInt32(this.UniqueValue, 0);
                return result == 0 ? this.GetNextValue() : result;
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