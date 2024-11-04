using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals
{
    /// <summary>
    /// Token for marking an enum.
    /// </summary>
    public interface IViEnumToken : IViToken
    {
        /// <summary>
        /// Gets the enum name.
        /// </summary>
        string EnumName { get; }

        /// <summary>
        /// Gets the enum type.
        /// </summary>
        Maybe<string> EnumType { get; }

        /// <summary>
        /// Gets the declaring type.
        /// </summary>
        Maybe<string> DeclaringType { get; }
    }
}