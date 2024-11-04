namespace Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree
{
    /// <summary>
    /// A token.
    /// </summary>
    public interface IViToken
    {
        #region properties

        /// <summary>
        /// Gets the start of the token.
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// Gets the length of the token.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Gets the full containing text with whitespace. (For strings this would be with double quotes).
        /// </summary>
        public string ContainingText { get; }

        #endregion

        /// <summary>
        /// Create a new token with a new start, length and containingText.
        /// </summary>
        /// <param name="start">The new start.</param>
        /// <param name="length">The new length.</param>
        /// <param name="containingText">The new containing text.</param>
        /// <returns>A new instance of <see cref="IViToken"/>.</returns>
        public IViToken With(int start, int length, string containingText);
    }
}