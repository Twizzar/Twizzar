using System.Collections.Generic;

#pragma warning disable CS1591, SA1600

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// The override kind for different types and members.
    /// </summary>
    public sealed class OverrideKind : ValueObject
    {
        private OverrideKind(bool isVirtual, bool isSealed)
        {
            this.IsVirtual = isVirtual;
            this.IsSealed = isSealed;
        }

        /// <summary>
        /// Gets a value indicating whether the property is virtual.
        /// </summary>
        public bool IsVirtual { get; }

        /// <summary>
        /// Gets a value indicating whether the property is sealed.
        /// </summary>
        public bool IsSealed { get; }

        public static OverrideKind Create(
            bool isVirtual = false,
            bool isSealed = false) =>
            new(isVirtual, isSealed);

        public static OverrideKind CreateVirtual() =>
            new(true, false);

        public static OverrideKind CreateSealed() =>
            new(false, true);

        #region Overrides of ValueObject

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.IsVirtual;
            yield return this.IsSealed;
        }

        #endregion
    }
}