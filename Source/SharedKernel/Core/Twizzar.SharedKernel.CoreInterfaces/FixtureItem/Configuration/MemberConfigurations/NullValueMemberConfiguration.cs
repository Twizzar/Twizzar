using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations
{
    /// <summary>
    /// Member configuration for configure a member as explicitly set to null.
    /// </summary>
    /// <seealso cref="ValueObject" />
    /// <seealso cref="IMemberConfiguration" />
    [ExcludeFromCodeCoverage] // This is a data holder class.
    public sealed record NullValueMemberConfiguration : MemberConfiguration<NullValueMemberConfiguration>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="NullValueMemberConfiguration"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="source"></param>
        public NullValueMemberConfiguration(string name, IConfigurationSource source)
            : base(name, source)
        {
            this.Name = name;
            this.Source = source;
        }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetAdditionalEqualityComponents() =>
            Enumerable.Empty<object>();

        #endregion
    }
}