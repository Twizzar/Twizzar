using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Runtime.Core.FixtureItem.Definition.ValueDefinitions
{
    /// <summary>
    /// Indicates that the value should be set to null explicitly.
    /// </summary>
    [ExcludeFromCodeCoverage] // This is a data holder class.
    public class NullValueDefinition : ValueObject, INullValueDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullValueDefinition"/> class.
        /// </summary>
        public NullValueDefinition()
        {
        }

        #region Overrides of ValueObject

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() =>
            Enumerable.Empty<object>();

        #endregion
    }
}
