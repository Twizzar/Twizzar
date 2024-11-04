using System.Diagnostics.CodeAnalysis;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.Failures
{
    /// <summary>
    /// Write or read to the system failure.
    /// </summary>
    /// <seealso cref="ViCommon.Functional.Monads.ResultMonad.Failure" />
    [ExcludeFromCodeCoverage] // Data holder for a failure.
    public class IoFailure : Failure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IoFailure"/> class.
        /// </summary>
        /// <param name="message">The failure message.</param>
        public IoFailure(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IoFailure"/> class.
        /// </summary>
        /// <param name="message">The failure message.</param>
        /// <param name="path">The path where the error occurred.</param>
        public IoFailure(string message, string path)
            : base(message)
        {
            this.Path = path;
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        public string Path { get; }
    }
}
