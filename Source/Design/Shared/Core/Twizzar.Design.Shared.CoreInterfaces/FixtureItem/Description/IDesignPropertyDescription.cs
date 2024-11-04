using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.Shared.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Property description interface for design domain.
    /// </summary>
    public interface IDesignPropertyDescription : IPropertyDescription
    {
        /// <summary>
        /// Gets a value indicating whether the property is an auto property.
        /// </summary>
        public bool IsAutoProperty { get; }
    }
}
