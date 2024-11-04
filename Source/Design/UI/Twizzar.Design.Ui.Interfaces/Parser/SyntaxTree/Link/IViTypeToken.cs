using Twizzar.Design.Shared.CoreInterfaces.Name;

namespace Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Link
{
    /// <summary>
    /// Token for a type.
    /// </summary>
    public interface IViTypeToken : IViToken
    {
        #region properties

        /// <summary>
        /// Gets the type full name token.
        /// </summary>
        public ITypeFullNameToken TypeFullNameToken { get; }

        /// <summary>
        /// Create a new <see cref="IViToken"/> with a new type full name.
        /// </summary>
        /// <param name="typeFullNameToken"></param>
        /// <returns>A new instance of <see cref="IViTypeToken"/>.</returns>
        public IViTypeToken WithToken(ITypeFullNameToken typeFullNameToken);

        #endregion
    }
}