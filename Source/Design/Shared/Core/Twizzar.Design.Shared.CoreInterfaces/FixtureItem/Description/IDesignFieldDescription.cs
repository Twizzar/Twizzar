using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Shared.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Field description interface for design domain.
    /// </summary>
    public interface IDesignFieldDescription : IFieldDescription
    {
        /// <summary>
        /// Gets a value indicating whether the field is a backing field used by a property.
        /// </summary>
        public bool IsBackingField { get; }

        /// <summary>
        /// Gets a maybe value of the linked property.
        /// Property is None, when field is no backing field.
        /// </summary>
        public Maybe<IPropertyDescription> BackingFieldProperty { get; }
    }
}