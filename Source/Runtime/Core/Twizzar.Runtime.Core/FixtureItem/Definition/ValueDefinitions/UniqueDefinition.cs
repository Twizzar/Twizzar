using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;

namespace Twizzar.Runtime.Core.FixtureItem.Definition.ValueDefinitions
{
    /// <summary>
    /// The value will be an unique value.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class UniqueDefinition : ValueObject, IUniqueDefinition, IHasLogger, IHasEnsureHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueDefinition"/> class.
        /// </summary>
        public UniqueDefinition()
        {
        }

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
        protected override IEnumerable<object> GetEqualityComponents() =>
            Enumerable.Empty<object>();

        #endregion
    }
}
