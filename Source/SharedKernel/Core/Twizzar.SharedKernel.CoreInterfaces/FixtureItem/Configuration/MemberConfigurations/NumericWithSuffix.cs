using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations
{
    /// <summary>
    /// Numeric with suffix (from parsing a number).
    /// </summary>
    /// <param name="Numeric"></param>
    /// <param name="Suffix"></param>
    public record struct NumericWithSuffix(string Numeric, Maybe<char> Suffix)
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Numeric + this.Suffix.Match(c => c.ToString(), string.Empty);
        }
    }
}
