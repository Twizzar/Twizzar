#pragma warning disable SA1623 // Property summary documentation should match accessors

namespace Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals
{
    /// <summary>
    /// Token for marking a boolean.
    /// </summary>
    public interface IViBoolToken : ILiteralToken
    {
        /// <summary>
        /// Gets a the boolean value.
        /// </summary>
        public bool Boolean { get; }
    }
}