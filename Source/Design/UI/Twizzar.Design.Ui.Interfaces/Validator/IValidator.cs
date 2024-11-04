using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twizzar.Design.Ui.Interfaces.Parser;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Ui.Interfaces.Validator
{
    /// <summary>
    /// ValidateAsync the syntax tree generate by <see cref="IParser"/>.
    /// </summary>
    public interface IValidator : IService
    {
        #region events

        /// <summary>
        /// Event triggered when the validator is initialized.
        /// </summary>
        event Action OnInitialized;

        /// <summary>
        /// Event triggered when the valid input change.
        /// </summary>
        event Action<IEnumerable<string>> OnValidInputsChanged;

        #endregion

        #region properties

        /// <summary>
        /// Gets a sequence with all valid inputs for this validator.
        /// </summary>
        IEnumerable<string> ValidInput { get; }

        /// <summary>
        /// Gets the default value of the validator.
        /// </summary>
        string DefaultValue { get; }

        /// <summary>
        /// Gets the tooltip of the validator.
        /// </summary>
        string Tooltip { get; }

        /// <summary>
        /// Gets the adorner text.
        /// </summary>
        string AdornerText { get; }

        /// <summary>
        /// Gets the type description of the validator.
        /// </summary>
        IBaseDescription TypeDescription { get; }

        #endregion

        #region members

        /// <summary>
        /// ValidateAsync the syntax tree / token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>A new root token.</returns>
        Task<IViToken> ValidateAsync(IResult<IViToken, ParseFailure> token);

        /// <summary>
        /// When loading or commiting tokens from ui to view model or vice versa, this method
        /// should be applied. It will inject some information used for the saving the config or
        /// it will neglect some information for better viewing in the ui.
        /// </summary>
        /// <param name="token">The input token.</param>
        /// <returns>The output token.</returns>
        IViToken Prettify(IViToken token);

        /// <summary>
        /// Initialize the validator.
        /// </summary>
        /// <returns></returns>
        Task InitializeAsync();

        #endregion
    }
}