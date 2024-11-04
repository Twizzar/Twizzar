using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Ui.Interfaces.Parser
{
    /// <summary>
    /// A parser which can parse text into <see cref="IViToken"/>s.
    /// </summary>
    public interface IParser : IService
    {
        #region members

        /// <summary>
        /// Parse a text into tokens.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>A sequence of valid or invalid tokens.
        /// <remarks>The parser stops on failure. So when a failure appears the last entry in the sequence is always the failure.</remarks>
        /// </returns>
        IResult<IViToken, ParseFailure> Parse(string text);

        #endregion
    }
}