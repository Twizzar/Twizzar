using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Twizzar.Runtime.CoreInterfaces.Exceptions
{
    /// <summary>
    /// Error occurred when resolving the definition to an instance.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Serializable]
    public sealed class ResolveTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveTypeException"/> class.
        /// </summary>
        /// <param name="message">The message describing the error.</param>
        public ResolveTypeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveTypeException"/> class.
        /// </summary>
        /// <param name="message">The message describing the error.</param>
        /// <param name="innerException">Inner exceptions.</param>
        public ResolveTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveTypeException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        private ResolveTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
