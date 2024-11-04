using System;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Encapsulation of a parameter default value.
    /// This is useful to distinguish between no Default value and null as an default value, because both will return null when checked with reflection.
    /// </summary>
    public readonly struct ParameterDefaultValue : IEquatable<ParameterDefaultValue>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterDefaultValue"/> struct.
        /// </summary>
        /// <param name="value"></param>
        public ParameterDefaultValue(object value)
        {
            this.Value = value;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the default parameter value.
        /// </summary>
        public object Value { get; }

        #endregion

        #region members

        /// <summary>
        /// Equals operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>True when equal.</returns>
        public static bool operator ==(ParameterDefaultValue left, ParameterDefaultValue right) => left.Equals(right);

        /// <summary>
        /// Not Equals operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>True when not equal.</returns>
        public static bool operator !=(ParameterDefaultValue left, ParameterDefaultValue right) => !left.Equals(right);

        /// <inheritdoc />
        public bool Equals(ParameterDefaultValue other) => Equals(this.Value, other.Value);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is ParameterDefaultValue other && this.Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => this.Value != null ? this.Value.GetHashCode() : 0;

        #region Overrides of ValueType

        /// <inheritdoc />
        public override string ToString()
        {
            var strValue = this.Value?.ToString() ?? "null";
            return $"{nameof(ParameterDefaultValue)}({strValue})";
        }

        #endregion

        #endregion
    }
}