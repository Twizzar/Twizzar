using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;

namespace Twizzar.Design.Ui.Parser.SyntaxTree.Literals
{
    /// <inheritdoc cref="IViNumericToken" />
    public class ViNumericToken : ViToken, IViNumericToken
    {
        #region ctors

        private ViNumericToken(int start, int length, string containingText, NumericWithSuffix numericWithSuffix)
            : base(start, length, containingText)
        {
            this.NumericWithSuffix = numericWithSuffix;
        }

        #endregion

        #region properties

        /// <inheritdoc/>
        public NumericWithSuffix NumericWithSuffix { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public override IViToken With(int start, int length, string containingText) =>
            new ViNumericToken(start, length, containingText, this.NumericWithSuffix);

        /// <inheritdoc/>
        public IViNumericToken With(NumericWithSuffix numericWithSuffix)
        {
            var newText = numericWithSuffix.ToString();
            return new ViNumericToken(0, newText.Length, newText, numericWithSuffix);
        }

        #endregion

        /// <summary>
        /// Create a new token.
        /// </summary>
        /// <param name="span">The span containing the number.</param>
        /// <param name="numericWithSuffix">The number with suffix.</param>
        /// <returns>A new instance of <see cref="ViNumericToken"/>.</returns>
        public static ViNumericToken Create(ParserSpan span, NumericWithSuffix numericWithSuffix) =>
            new(
                span.Start,
                span.Length,
                span.Content,
                numericWithSuffix);
    }
}