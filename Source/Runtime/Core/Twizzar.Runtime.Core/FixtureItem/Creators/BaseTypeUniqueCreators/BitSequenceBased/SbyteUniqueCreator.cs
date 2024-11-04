using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators.BitSequenceBased
{
    /// <summary>
    /// The generator for the <see cref="sbyte"/> type using bit sequence based generator <see cref="BitSequenceBasedUniqueCreator{T}"/>.
    /// </summary>
    public class SbyteUniqueCreator : BitSequenceBasedUniqueCreator<sbyte>
    {
        #region static fields and constants

        private static byte[] _uniqueValue = new byte[sizeof(sbyte)];
        private static int _currentPartitionIndex;
        private static BitArray[] _partitions = new BitArray[sizeof(sbyte)];

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
        protected override int TotalBits => sizeof(sbyte) * BitsOfByte;

        /// <inheritdoc />
        protected override int PartitionSize => sizeof(sbyte);

        #endregion

        #region Overrides of BitSequenceBasedUniqueCreator<int>

        /// <inheritdoc />
        protected override sbyte ConvertByteArrayToT()
        {
            if (this.UniqueValue[0] != 0)
            {
                return unchecked((sbyte)this.UniqueValue[0]);
            }

            return this.GetNextValue();
        }

        #endregion
    }
}