namespace Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Link
{
    /// <summary>
    /// Token for the name.
    /// </summary>
    public interface IViLinkNameToken : IViToken
    {
        #region properties

        /// <summary>
        /// Gets the link name.
        /// </summary>
        public string Name { get; }

        #endregion
    }
}