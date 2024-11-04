using System.Collections.Generic;
using System.Linq;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations
{
    /// <summary>
    /// Configuration item for a member which is configured as a unique value.
    /// </summary>
    public sealed record UniqueValueMemberConfiguration(string Name, IConfigurationSource Source) : MemberConfiguration<UniqueValueMemberConfiguration>(Name, Source)
    {
        #region members

        /// <inheritdoc /> yield return this.
        protected override IEnumerable<object> GetAdditionalEqualityComponents() =>
            Enumerable.Empty<object>();

        #endregion
    }
}