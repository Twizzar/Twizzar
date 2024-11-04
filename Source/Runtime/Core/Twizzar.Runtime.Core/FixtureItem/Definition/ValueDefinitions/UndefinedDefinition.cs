using System.Collections.Generic;
using System.Linq;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Runtime.Core.FixtureItem.Definition.ValueDefinitions
{
    /// <summary>
    /// Indicates that the value is undefined and will be ignored by the creator.
    /// </summary>
    public sealed class UndefinedDefinition : ValueObject, IUndefinedDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UndefinedDefinition"/> class.
        /// </summary>
        public UndefinedDefinition()
        {
        }

        #region Overrides of ValueObject

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents() =>
            Enumerable.Empty<object>();

        #endregion
    }
}
