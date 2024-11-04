namespace Twizzar.Design.Ui.Interfaces.Enums
{
    /// <summary>
    /// needed for the icon later.
    /// </summary>
    public enum AutoCompleteFormat
    {
        /// <summary>
        /// None.
        /// </summary>
        None,

        /// <summary>
        /// Keyword format like null, unique.
        /// </summary>
        Keyword,

        /// <summary>
        /// The type format like ExampleCode.SuperCar.
        /// </summary>
        Type,

        /// <summary>
        /// The type combined with possible fixture ids saved in the yaml file.
        /// </summary>
        TypeAndId,
    }
}