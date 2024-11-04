using System.Diagnostics.CodeAnalysis;
using System.Threading;

using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Analyzer
{
    /// <summary>
    /// Failure for describing an operation which was canceled by a <see cref="CancellationToken"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class OperationCanceledFailure : Failure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationCanceledFailure"/> class.
        /// </summary>
        public OperationCanceledFailure()
            : base("Operation Canceled")
        {
        }
    }
}
