namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums
{
    /// <summary>
    /// The kind of the base type.
    /// </summary>
    public enum BaseTypeKind
    {
        /// <summary>
        /// A number like int, double, etc.
        /// </summary>
        Number,

        /// <summary>
        /// A char.
        /// </summary>
        Char,

        /// <summary>
        /// A string.
        /// </summary>
        String,

        /// <summary>
        /// A bool.
        /// </summary>
        Boolean,

        /// <summary>
        /// A enum.
        /// </summary>
        Enum,

        /// <summary>
        /// Not a base type.
        /// </summary>
        Complex,
    }
}