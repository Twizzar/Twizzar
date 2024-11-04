namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations
{
    /// <summary>
    /// Literal value (numbers, bool) represented as a string.
    /// </summary>
    /// <param name="Value"></param>
    public record struct SimpleLiteralValue(string Value)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleLiteralValue"/> class.
        /// </summary>
        /// <param name="value"></param>
        public SimpleLiteralValue(bool value)
            : this(value.ToString().ToLowerInvariant())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleLiteralValue"/> class.
        /// </summary>
        /// <param name="value"></param>
        public SimpleLiteralValue(NumericWithSuffix value)
            : this(value.ToString())
        {
        }

        /// <inheritdoc/>
        public override string ToString() => this.Value;
    }
}
