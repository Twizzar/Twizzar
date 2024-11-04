using System.Collections.Generic;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations
{
    /// <summary>
    /// Member configuration when a code value was set.
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="SourceCode"></param>
    /// <param name="Source"></param>
    public record CodeValueMemberConfiguration(string Name, string SourceCode, IConfigurationSource Source) : MemberConfiguration<CodeValueMemberConfiguration>(
        Name,
        Source)
    {
        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetAdditionalEqualityComponents()
        {
            yield return this.SourceCode;
        }

        #endregion
    }
}