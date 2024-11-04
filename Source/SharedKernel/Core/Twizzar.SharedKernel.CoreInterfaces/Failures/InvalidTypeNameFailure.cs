using System.Diagnostics.CodeAnalysis;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.Failures
{
    /// <summary>
    /// Exception type the given string cannot be a type.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class InvalidTypeNameFailure : Failure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTypeNameFailure"/> class.
        /// </summary>
        /// <param name="message">Failure message.</param>
        public InvalidTypeNameFailure(string message)
            : base(message)
        {
        }

        #region Overrides of Failure

        /// <inheritdoc />
        public override string ToString() => "Failure: " + this.Message;

        #endregion
    }
}
