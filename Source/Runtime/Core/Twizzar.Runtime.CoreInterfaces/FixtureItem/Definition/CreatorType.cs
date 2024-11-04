namespace Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition
{
    /// <summary>
    /// Creator type of the definition. Used for mapping to an creator.
    /// </summary>
    public enum CreatorType
    {
        /// <summary>
        /// For the Moq Creator.
        /// </summary>
        Moq,

        /// <summary>
        /// For the ConcreteTypeCreator.
        /// </summary>
        ConcreteType,

        /// <summary>
        /// For the BaseTypeCreator.
        /// </summary>
        BaseType,
    }
}
