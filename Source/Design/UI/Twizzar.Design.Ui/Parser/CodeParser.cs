using Twizzar.Design.Ui.Interfaces.Parser;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Parser.SyntaxTree.Literals;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Ui.Parser
{
    /// <summary>
    /// Parser for values set by code.
    /// </summary>
    public class CodeParser : IParser
    {
        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public IResult<IViToken, ParseFailure> Parse(string text) =>
            Result.Success<IViToken, ParseFailure>(new ViCodeToken(0, text.Length, text));

        #endregion
    }
}
