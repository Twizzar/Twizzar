namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location
{
    /// <summary>
    /// Used when the document location is unknown.
    /// </summary>
    public record UnknownDocumentLocation : IDocumentLocation
    {
        #region properties

        /// <inheritdoc/>
        public int Column => -1;

        /// <inheritdoc/>
        public int Row => -1;

        #endregion

        #region members

        /// <inheritdoc />
        public override string ToString() =>
            "Location unknown";

        #endregion
    }
}