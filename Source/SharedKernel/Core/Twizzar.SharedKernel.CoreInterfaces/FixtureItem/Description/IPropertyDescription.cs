using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Discovers the attributes of a property and provides access to property metadata.
    /// </summary>
    public interface IPropertyDescription : IMemberDescription
    {
        #region description

        /// <summary>
        /// Gets a value indicating whether the property can be read.
        /// </summary>
        public bool CanRead { get; }

        /// <summary>
        /// Gets a value indicating whether the property can be written to.
        /// </summary>
        public bool CanWrite { get; }

        /// <summary>
        /// Gets a value indicating whether the property is static.
        /// </summary>
        public bool IsStatic { get; }

        #region methods

        /// <summary>
        /// Gets the property setter.
        /// </summary>
        public Maybe<IMethodDescription> SetMethod { get; }

        /// <summary>
        /// Gets the property getter.
        /// </summary>
        public Maybe<IMethodDescription> GetMethod { get; }

        #endregion

        /// <summary>
        /// Gets the override kind of the property.
        /// </summary>
        public OverrideKind OverrideKind { get; }

        #endregion
    }
}