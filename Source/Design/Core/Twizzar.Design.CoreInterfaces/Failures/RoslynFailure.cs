using System.Diagnostics.CodeAnalysis;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.CoreInterfaces.Failures
{
    /// <summary>
    /// Data object for roslyn failures.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RoslynFailure : Failure, IHasEnsureHelper
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynFailure"/> class.
        /// </summary>
        /// <param name="message"></param>
        public RoslynFailure(string message)
            : base(message)
        {
            this.EnsureParameter(message, nameof(message)).ThrowWhenNull();
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion
    }
}