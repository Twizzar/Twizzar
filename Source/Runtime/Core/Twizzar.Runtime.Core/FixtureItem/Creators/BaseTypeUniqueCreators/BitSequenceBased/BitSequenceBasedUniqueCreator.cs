using System.Collections;

namespace Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators.BitSequenceBased
{
    /// <summary>
    /// Abstract base class for bit sequence based byte array generator.
    /// The bit sequence can be used directly to generated the value of T in
    /// a meaningful way. Types are int, long, byte etc.
    /// </summary>
    /// <typeparam name="T">The type parameter of the generator base type.</typeparam>
    public abstract class BitSequenceBasedUniqueCreator<T> : BaseByteArrayUniqueCreator<T>
        where T : struct
    {
        /// <inheritdoc />
        protected override void InitializePartitionAndDetermineFirstValue()
        {
            var partition = new BitArray(this.TotalBits);

            if (this.CurrentPartitionIndex > 0)
            {
                // set all bits to true
                partition.SetAll(true);

                // Looping from right to left and set
                // bits to false  again (according to the partition index)
                for (var y = this.CurrentPartitionIndex; y > 0; y--)
                {
                    partition.Set(BitsOfByte - y, false);
                }
            }

            // set partition and UniqueValue value.
            this.SetCurrentPartition(partition);
            this.CurrentPartition.CopyTo(this.UniqueValue, 0);
        }

        /// <inheritdoc />
        protected override void DetermineNextValue()
        {
            // Looping byte vice from right to left
            // for two bytes we loop from index 8...15 and then from 0..7
            for (var byteIndex = this.PartitionSize; byteIndex > 0; byteIndex--)
            {
                for (var bitIndex = BitsOfByte; bitIndex > 0; bitIndex--)
                {
                    var index = (byteIndex * BitsOfByte) - bitIndex;
                    if (!this.CurrentPartition.Get(index))
                    {
                        // found digit of 0, set it to 1 and return
                        this.CurrentPartition.Set(index, true);
                        this.CurrentPartition.CopyTo(this.UniqueValue, 0);
                        return;
                    }

                    // The digit is 1, change it to 0 and look for further digit with 0
                    this.CurrentPartition.Set(index, false);
                }
            }

            // no digits of 0 found and all have been reset, next value is 0
            this.CurrentPartition.CopyTo(this.UniqueValue, 0);
        }
    }
}
