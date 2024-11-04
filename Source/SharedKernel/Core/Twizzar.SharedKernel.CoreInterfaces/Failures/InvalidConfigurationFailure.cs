using System;
using System.Diagnostics.CodeAnalysis;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.Failures
{
    /// <summary>
    /// Describes a failure occurred because the config is invalid.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class InvalidConfigurationFailure : Failure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationFailure"/> class.
        /// </summary>
        /// <param name="configurationItem">The invalid configuration item.</param>
        /// <param name="message">Message which describes the failure.</param>
        public InvalidConfigurationFailure(IConfigurationItem configurationItem, string message)
            : base(message)
        {
            this.ConfigurationItem = configurationItem ?? throw new ArgumentNullException(nameof(configurationItem));
        }

        /// <summary>
        /// Gets the invalid configuration.
        /// </summary>
        public IConfigurationItem ConfigurationItem { get; }

        /// <inheritdoc />
        public override string ToString() =>
            $"Failure: {this.Message} in {this.ConfigurationItem}";
    }
}
