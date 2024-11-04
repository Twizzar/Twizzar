using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using ViCommon.Functional.Monads.MaybeMonad;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;

namespace Twizzar.SharedKernel.CoreInterfaces.Exceptions
{
    /// <summary>
    ///  Exception type when the configuration has an invalid entry.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class InvalidConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationException"/> class.
        /// </summary>
        /// <param name="configurationItem">The configuration item which is invalid.</param>
        public InvalidConfigurationException(IConfigurationItem configurationItem)
        {
            this.ConfigurationItem = Some(configurationItem);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationException"/> class.
        /// </summary>
        /// <param name="message">he error message that explains the reason for the exception.</param>
        /// <param name="memberConfiguration">The memberConfiguration item which is invalid.</param>
        public InvalidConfigurationException(string message, IMemberConfiguration memberConfiguration)
            : base(message)
        {
            this.MemberConfigurationItem = Some(memberConfiguration);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="configurationItem">The configuration item which is invalid.</param>
        public InvalidConfigurationException(string message, IConfigurationItem configurationItem)
            : base(message)
        {
            this.ConfigurationItem = Some(configurationItem);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        /// <param name="configurationItem">The configuration item which is invalid.</param>
        public InvalidConfigurationException(
            string message,
            Exception innerException,
            IConfigurationItem configurationItem)
            : base(message, innerException)
        {
            this.ConfigurationItem = Some(configurationItem);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected InvalidConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ConfigurationItem = (Maybe<IConfigurationItem>)info.GetValue(
                nameof(this.ConfigurationItem), typeof(Maybe<IConfigurationItem>));
            this.MemberConfigurationItem = (Maybe<IMemberConfiguration>)info.GetValue(
                nameof(this.MemberConfigurationItem), typeof(Maybe<IMemberConfiguration>));
        }

        /// <summary>
        /// Gets the configuration item which is invalid.
        /// </summary>
        public Maybe<IConfigurationItem> ConfigurationItem { get; }

        /// <summary>
        /// Gets the configuration item which is invalid.
        /// </summary>
        public Maybe<IMemberConfiguration> MemberConfigurationItem { get; }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(this.ConfigurationItem), this.ConfigurationItem);
            info.AddValue(nameof(this.MemberConfigurationItem), this.MemberConfigurationItem);
            base.GetObjectData(info, context);
        }
    }
}