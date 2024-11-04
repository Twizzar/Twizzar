using System;
using System.Collections;
using Twizzar.Runtime.CoreInterfaces.Resources;

namespace Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators.FloatingPointNumbers
{
    /// <summary>
    /// Abstract base class for floating point byte based generators (float and double).
    /// The generator will add one to the mantissa and exponent in every run.
    /// </summary>
    /// <typeparam name="T">The type parameter of the generator base type.</typeparam>
    public abstract class FloatingPointUniqueCreator<T> : BaseByteArrayUniqueCreator<T>
    {
        private readonly int _lsbExponent;
        private readonly int _msbExponent;
        private readonly int _lsbMantissa;
        private readonly int _msbMantissa;

        /// <summary>
        /// Initializes a new instance of the <see cref="FloatingPointUniqueCreator{T}"/> class.
        /// </summary>
        /// <param name="mantissaSize">The whole size of the mantissa.</param>
        /// <param name="exponentSize">The whole size of the exponent.</param>
        /// <param name="mantissaUsedSize">The size of the mantissa used to generate values.</param>
        /// <param name="exponentUsedSize">The size of the exponent used to generate values.</param>
        /// <param name="partitionCount">The number of partitions for positives numbers and for negative numbers.</param>
        protected FloatingPointUniqueCreator(int mantissaSize, int exponentSize, int mantissaUsedSize, int exponentUsedSize, int partitionCount)
        {
            // float is a 32 bit type (23 bits of mantissa, 8 bits of exponent, last bit of sign),
            // and double is a 64 bit type (52 bits of mantissa, 11 bits of exponent, last bit of sign).
            if (mantissaUsedSize > mantissaSize)
            {
                throw new ArgumentException(ErrorMessagesRuntime.FloatingPointCreator_InvalidMantissa);
            }

            if (exponentUsedSize > exponentSize)
            {
                throw new ArgumentException(ErrorMessagesRuntime.FloatingPointCreator_InvalidExponent);
            }

            if (mantissaSize <= 0 || exponentSize <= 0 || mantissaUsedSize <= 0 || exponentUsedSize <= 0 ||
                                 partitionCount <= 0)
            {
                throw new ArgumentException(ErrorMessagesRuntime.FloatingPointCreator_BiggerZero);
            }

            // calculate the indices of exponent and mantissa.
            // last index ist always the sign bit.
            this._msbExponent = this.TotalBits - 2;
            this._lsbExponent = this.TotalBits - 1 - exponentUsedSize;

            this._msbMantissa = this.TotalBits - 2 - exponentSize;
            this._lsbMantissa = this.TotalBits - 1 - exponentSize - mantissaUsedSize;
        }

        /// <inheritdoc />
        protected override void InitializePartitionAndDetermineFirstValue()
        {
            var partition = new BitArray(this.TotalBits);

            if (this.CurrentPartitionIndex > 0)
            {
                // set partition bits of exponent and mantissa all to true
                for (var index = 0; index < this.TotalBits; index++)
                {
                    if ((index <= this._msbExponent && index >= this._lsbExponent) ||
                        (index <= this._msbMantissa && index >= this._lsbMantissa))
                    {
                        partition.Set(index, true);
                    }
                }

                // Looping from right to left of the parts and set
                // bits to false  again (according to the partition index)
                for (var y = this.CurrentPartitionIndex % (this.PartitionSize / 2); y >= 0; y--)
                {
                    partition.Set(this._msbExponent - y, false);
                    partition.Set(this._msbMantissa - y, false);
                }

                // set sign bit
                if (this.CurrentPartitionIndex >= (this.PartitionSize / 2))
                {
                    partition.Set(this.TotalBits - 1, true);
                }
            }

            this.SetCurrentPartition(partition);
            this.CurrentPartition.CopyTo(this.UniqueValue, 0);
        }

        /// <inheritdoc />
        protected override void DetermineNextValue()
        {
            // add one to exponent:
            // Looping from right to left
            for (var bitIndex = this._lsbExponent; bitIndex <= this._msbExponent; bitIndex++)
            {
                if (!this.CurrentPartition.Get(bitIndex))
                {
                    this.CurrentPartition.Set(bitIndex, true);
                    break;
                }

                this.CurrentPartition.Set(bitIndex, false);
            }

            // add one to mantissa:
            // Looping from right to left
            for (var bitIndex = this._lsbMantissa; bitIndex <= this._msbMantissa; bitIndex++)
            {
                if (!this.CurrentPartition.Get(bitIndex))
                {
                    this.CurrentPartition.Set(bitIndex, true);
                    this.CurrentPartition.CopyTo(this.UniqueValue, 0);
                    return;
                }

                this.CurrentPartition.Set(bitIndex, false);
            }

            this.CurrentPartition.CopyTo(this.UniqueValue, 0);
        }
    }
}
