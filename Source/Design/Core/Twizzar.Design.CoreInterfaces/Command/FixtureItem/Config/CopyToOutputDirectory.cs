namespace Twizzar.Design.CoreInterfaces.Command.FixtureItem.Config
{
    /// <summary>
    /// Options for the Copy to Output Directory property.
    /// </summary>
    public enum CopyToOutputDirectory : uint
    {
        /// <summary>
        /// Do not copy.
        /// </summary>
        DoNotCopy = 0U,

        /// <summary>
        /// Copy always.
        /// </summary>
        CopyAlways = 1U,

        /// <summary>
        /// Copy if newer.
        /// </summary>
        CopyIfNewer = 2u,
    }
}
