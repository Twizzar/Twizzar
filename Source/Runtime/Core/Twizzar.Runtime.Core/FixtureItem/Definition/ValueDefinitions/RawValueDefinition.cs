using System.Collections.Generic;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Runtime.Core.FixtureItem.Definition.ValueDefinitions
{
    /// <summary>
    /// The value is a 'raw value' which means it was set in the configuration and is stored in the definition.
    /// </summary>
    public sealed class RawValueDefinition : ValueObject, IRawValueDefinition, IHasLogger, IHasEnsureHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawValueDefinition"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public RawValueDefinition(object value)
        {
            this.EnsureParameter(value, nameof(value))
                .ThrowWhenNull();
            this.Value = value;
        }

        #region Properties

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value { get; }

        #endregion

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region Overrides of ValueObject

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Value;
        }

        #endregion
    }
}
