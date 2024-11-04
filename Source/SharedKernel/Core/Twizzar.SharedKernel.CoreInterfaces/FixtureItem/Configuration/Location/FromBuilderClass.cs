namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location
{
    /// <summary>
    /// Represents the configuration class location.
    /// </summary>
    /// <param name="DocumentFilePath">The file path to the configuration class document.</param>
    /// <param name="DocumentLocation">The location in the document.</param>
    public record FromBuilderClass(string DocumentFilePath, IDocumentLocation DocumentLocation) : IConfigurationSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FromBuilderClass"/> class.
        /// </summary>
        /// <param name="fromeCode"></param>
        public FromBuilderClass(FromCode fromeCode)
            : this(fromeCode.DocumentFilePath, fromeCode.DocumentLocation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FromBuilderClass"/> class.
        /// </summary>
        /// <param name="documentFilePath"></param>
        public FromBuilderClass(string documentFilePath)
            : this(documentFilePath, new UnknownDocumentLocation())
        {
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{this.DocumentFilePath} at Line: {this.DocumentLocation.Row + 1} Column: {this.DocumentLocation.Column + 1}";
    }
}