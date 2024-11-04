using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Discovers the attributes of a field and provides access to field metadata.
    /// </summary>
    public interface IFieldDescription : IMemberDescription
    {
        /// <summary>
        /// Gets the declaring type of this Field.
        /// </summary>
        public ITypeFullName DeclaringType { get; }

        /// <summary>
        /// Gets a value indicating whether the field is static.
        /// </summary>
        public bool IsStatic { get; }

        /// <summary>
        /// Gets a value indicating whether the field is constant.
        /// </summary>
        public bool IsConstant { get; }

        /// <summary>
        /// Gets a value indicating whether the field is readonly.
        /// </summary>
        public bool IsReadonly { get; }

        /// <summary>
        /// Gets the  constant value of the field if <see cref="IsConstant"/> else None.
        /// </summary>
        Maybe<object> ConstantValue { get; }
    }
}