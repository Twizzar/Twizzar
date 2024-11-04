namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Describes a member of a interface or class.
    /// </summary>
    public interface IMemberDescription : IBaseDescription, IValueObject
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the declared type description.
        /// </summary>
        public ITypeDescription DeclaredDescription { get; }
    }
}
