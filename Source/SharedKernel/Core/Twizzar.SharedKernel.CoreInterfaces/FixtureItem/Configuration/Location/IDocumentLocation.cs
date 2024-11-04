namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location
{
    /// <summary>
    /// The location in the document.
    /// </summary>
    public interface IDocumentLocation
    {
        /// <summary>
        /// Gets the column or character of the location.
        /// <remarks>This will start at zero. For displaying use this + 1.</remarks>
        /// </summary>
        int Column { get; }

        /// <summary>
        /// Gets the row or line of the location.
        /// <remarks>This will start at zero. For displaying use this + 1.</remarks>
        /// </summary>
        int Row { get; }
    }
}