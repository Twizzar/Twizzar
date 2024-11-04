using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Twizzar.SharedKernel.CoreInterfaces.Exceptions
{
    /// <summary>
    /// Represents an internal error which the user should not encounter.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class InternalException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public InternalException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">An inner exception.</param>
        public InternalException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected InternalException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
