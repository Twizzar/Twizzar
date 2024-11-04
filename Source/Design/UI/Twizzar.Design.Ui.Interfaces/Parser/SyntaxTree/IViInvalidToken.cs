namespace Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree
{
    /// <summary>
    /// A token marking text as invalid.
    /// </summary>
    public interface IViInvalidToken : IViToken
    {
        #region properties

        /// <summary>
        /// Gets the message why this is invalid.
        /// </summary>
        public string Message { get; }

        #endregion
    }
}