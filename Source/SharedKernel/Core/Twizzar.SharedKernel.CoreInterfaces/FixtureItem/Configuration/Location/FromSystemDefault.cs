namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location
{
    /// <summary>
    /// Represents the system default location.
    /// </summary>
    public record FromSystemDefault : IConfigurationSource
    {
        #region members

        /// <inheritdoc />
        public override string ToString() =>
            "FromSystemDefault";

        #endregion
    }
}