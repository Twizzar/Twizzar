namespace Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals
{
    /// <summary>
    /// A string token.
    /// </summary>
    public interface IViStringToken : ILiteralToken
    {
        #region properties

        /// <summary>
        /// Gets the text of the string.
        /// </summary>
        string Text { get; }

        #endregion
    }
}