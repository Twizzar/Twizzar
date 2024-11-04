using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Twizzar.Fixture.Verifier
{
    /// <summary>
    /// Error occurred when verifying ctor.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Serializable]
    public sealed class CtorVerifierException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CtorVerifierException"/> class.
        /// </summary>
        /// <param name="message">The message describing the error.</param>
        public CtorVerifierException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CtorVerifierException"/> class.
        /// </summary>
        /// <param name="message">The message describing the error.</param>
        /// <param name="innerException">Inner exceptions.</param>
        public CtorVerifierException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CtorVerifierException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        private CtorVerifierException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
