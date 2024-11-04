using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;

namespace Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals
{
    /// <summary>
    /// Token for numbers.
    /// </summary>
    public interface IViNumericToken : ILiteralToken
    {
        #region properties

        /// <summary>
        /// Gets the numeric with suffix.
        /// </summary>
        NumericWithSuffix NumericWithSuffix { get; }

        #endregion

        #region members

        /// <summary>
        /// Construct a new <see cref="IViNumericToken"/> from this.
        /// </summary>
        /// <param name="numericWithSuffix">The new number with suffix.</param>
        /// <returns></returns>
        IViNumericToken With(NumericWithSuffix numericWithSuffix);

        #endregion
    }
}