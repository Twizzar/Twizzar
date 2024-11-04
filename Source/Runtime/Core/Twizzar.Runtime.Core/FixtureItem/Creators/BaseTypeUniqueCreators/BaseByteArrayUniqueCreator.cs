using System;
using System.Collections;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;

namespace Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators
{
    /// <summary>
    /// Abstract base class for byte array based generator.
    /// </summary>
    /// <typeparam name="T">The type parameter of the generator base type.</typeparam>
    public abstract class BaseByteArrayUniqueCreator<T> : IUniqueCreator<T>
    {
        #region static fields and constants

        /// <summary>
        /// Number of bits in a byte.
        /// </summary>
        protected const int BitsOfByte = 8;

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <summary>
        /// Gets or sets the partitions.
        /// </summary>
        protected abstract BitArray[] Partitions { get; set; }

        /// <summary>
        /// Gets or sets the current generated unique value represented as byte array.
        /// </summary>
        protected abstract byte[] UniqueValue { get; set; }

        /// <summary>
        /// Gets the current partition.
        /// </summary>
        protected abstract BitArray CurrentPartition { get; }

        /// <summary>
        /// Gets the number of bits represented by the generator.
        /// </summary>
        protected abstract int TotalBits { get; }

        /// <summary>
        /// Gets the number of partitions used.
        /// </summary>
        protected abstract int PartitionSize { get; }

        /// <summary>
        /// Gets or sets the current partition index, which will generate the next new value.
        /// </summary>
        protected abstract int CurrentPartitionIndex { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public T GetNextValue()
        {
            this.NextValue();

            return this.ConvertByteArrayToT();
        }

        /// <summary>
        /// Convert the current element a value of T.
        /// </summary>
        /// <returns>The current byte array as a value of T.</returns>
        protected abstract T ConvertByteArrayToT();

        /// <summary>
        /// Initialize current partition and set first value to the current element.
        /// </summary>
        protected abstract void InitializePartitionAndDetermineFirstValue();

        /// <summary>
        /// Determines next value and set it to the current element.
        /// </summary>
        protected abstract void DetermineNextValue();

        /// <summary>
        /// Sets the current partition.
        /// </summary>
        /// <param name="partition">The bit array partition.</param>
        protected void SetCurrentPartition(BitArray partition)
        {
            this.Partitions[this.CurrentPartitionIndex] =
                partition ?? throw new ArgumentNullException(nameof(partition));
        }

        /// <summary>
        /// Gets the next value of the current partition by adding one to the byte array
        /// and copy this value to the current element.
        /// </summary>
        private void NextValue()
        {
            if (this.CurrentPartition is null)
            {
                this.InitializePartitionAndDetermineFirstValue();
            }
            else
            {
                this.DetermineNextValue();
            }

            this.CurrentPartitionIndex = ++this.CurrentPartitionIndex % this.PartitionSize;
        }

        #endregion
    }
}