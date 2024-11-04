namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums
{
    /// <summary>
    /// Describes the method kind.
    /// </summary>
    public enum MethodKind
    {
        /// <summary>
        /// The Method is a constructor.
        /// </summary>
        Constructor,

        /// <summary>
        /// The Method is a Property setter or getter.
        /// </summary>
        Property,

        /// <summary>
        /// The Method is an ordinary method.
        /// </summary>
        Ordinary,

        /// <summary>
        /// The method is not a Property, constructor or ordinary method.
        /// </summary>
        Other,
    }
}