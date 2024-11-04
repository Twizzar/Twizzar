using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;

namespace Twizzar.Design.Ui.Parser
{
    /// <summary>
    /// Abstract BaseType parser for all base types.
    /// </summary>
    public abstract class BaseTypeParser : BaseParser
    {
        #region properties

        /// <summary>
        /// Gets a Parser which parses a base type value.
        /// </summary>
        protected abstract ParserExtensions.Parser<IViToken> ValueParser { get; }

        /// <inheritdoc />
        protected override ParserExtensions.Parser<IViToken> Parser =>
            ConsumeToken.UniqueToken // Consume Unique Token
                .Or(ConsumeToken.NullToken) // Or a NullToken
                .Or(this.ValueParser); // Or a value

        #endregion
    }
}