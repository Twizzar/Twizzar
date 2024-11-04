namespace Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals
{
    /// <summary>
    /// Token for marking a char.
    /// </summary>
    public interface IViCharToken : ILiteralToken
    {
        #region properties

        /// <summary>
        /// Gets the character.
        /// </summary>
        public char Character { get; }

        #endregion
    }
}