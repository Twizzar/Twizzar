namespace Twizzar.Design.Ui.Interfaces.Enums
{
    /// <summary>
    /// The format of a segment of a fixture item value.
    /// This is needed for coloring an styling styling.
    /// </summary>
    public enum SegmentFormat
    {
        /// <summary>
        /// The default format.
        /// </summary>
        None,

        /// <summary>
        /// Used for the type segment.
        /// </summary>
        Type,

        /// <summary>
        /// Used for strings and chars.
        /// </summary>
        Letter,

        /// <summary>
        /// Used for booleans.
        /// </summary>
        Boolean,

        /// <summary>
        /// Used for number.
        /// </summary>
        Number,

        /// <summary>
        /// Used for default and undefined.
        /// </summary>
        DefaultOrUndefined,

        /// <summary>
        /// Used for the known keywords like null, default, undefined and unique.
        /// </summary>
        Keyword,

        /// <summary>
        /// used for the id segment of a fixture item value.
        /// </summary>
        Id,

        /// <summary>
        /// Used for the selected constructor.
        /// </summary>
        SelectedCtor,

        /// <summary>
        /// Used for readonly code.
        /// </summary>
        ReadonlyCode,
    }
}