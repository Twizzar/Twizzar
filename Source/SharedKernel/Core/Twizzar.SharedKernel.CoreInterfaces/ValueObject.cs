using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Twizzar.SharedKernel.CoreInterfaces.Extensions;

namespace Twizzar.SharedKernel.CoreInterfaces
{
    /// <inheritdoc cref="IValueObject" />
    public abstract class ValueObject
    {
        /// <summary>
        /// Check if value object a is equals value object b.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns>True if equals.</returns>
        [SuppressMessage("Blocker Code Smell", "S3875:\"operator==\" should not be overloaded on reference types", Justification = "For value objects compare values note only reference.")]
        public static bool operator ==(ValueObject a, ValueObject b) =>
            a?.Equals(b) ?? b is null;

        /// <summary>
        /// Check if value object a is not equals value object b.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns>False if equals.</returns>
        public static bool operator !=(ValueObject a, ValueObject b) =>
            !(a == b);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;

            return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        /// <summary>
        /// Generates hashcode for each equality component by taking the bitwise logical exclusive OR
        /// for for each value.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() =>
            this.GetEqualityComponents()
                .GetHashCodeOfElements();

        /// <inheritdoc />
        public override string ToString() =>
            this.GetType().Name + "(" + string.Join(",", this.GetEqualityComponents()) + ")";

        /// <summary>s
        /// Operator for comparing two value objects.
        /// </summary>
        /// <param name="left">The first value object.</param>
        /// <param name="right">The second value object.</param>
        /// <returns>True when they are equal.</returns>
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (left is null ^ right is null)
            {
                return false;
            }

            return left is null || left.Equals(right);
        }

        /// <summary>
        /// Operator for comparing two value objects.
        /// </summary>
        /// <param name="left">The first value object.</param>
        /// <param name="right">The second value object.</param>
        /// <returns>True when they are not equal.</returns>
        protected static bool NotEqualOperator(ValueObject left, ValueObject right) =>
            !EqualOperator(left, right);

        /// <summary>
        /// Get all properties which should be check for equality.
        /// </summary>
        /// <returns>A sequence of object.</returns>
        [ExcludeFromCodeCoverage]
        protected abstract IEnumerable<object> GetEqualityComponents();
    }
}