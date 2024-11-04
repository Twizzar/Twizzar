using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Twizzar.SharedKernel.CoreInterfaces.Exceptions
{
    /// <summary>
    /// Exception type when the given type description is not valid for the given context.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class InvalidTypeDescriptionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTypeDescriptionException"/> class.
        /// </summary>
        public InvalidTypeDescriptionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTypeDescriptionException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InvalidTypeDescriptionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTypeDescriptionException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public InvalidTypeDescriptionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTypeDescriptionException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected InvalidTypeDescriptionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
