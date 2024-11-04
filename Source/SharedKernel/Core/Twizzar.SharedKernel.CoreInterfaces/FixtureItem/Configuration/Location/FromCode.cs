namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location
{
    /// <summary>
    /// Represents a configuration which was configured by the user over the code.
    /// </summary>
    /// <param name="DocumentFilePath"></param>
    /// <param name="DocumentLocation"></param>
    public record FromCode(string DocumentFilePath, IDocumentLocation DocumentLocation) : IConfigurationSource
    {
        #region members

        /// <inheritdoc />
        public override string ToString() =>
            $"{this.DocumentFilePath} at Line: {this.DocumentLocation.Row + 1} Column: {this.DocumentLocation.Column + 1}";

        #endregion
    }
}