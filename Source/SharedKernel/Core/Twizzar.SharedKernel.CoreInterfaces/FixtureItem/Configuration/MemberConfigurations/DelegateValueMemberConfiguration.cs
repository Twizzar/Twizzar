using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations
{
    /// <summary>
    /// Configuration for a delegate which calculates the return value.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public record DelegateValueMemberConfiguration
        : MemberConfiguration<DelegateValueMemberConfiguration>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateValueMemberConfiguration"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="valueDelegate"></param>
        /// <param name="source"></param>
        public DelegateValueMemberConfiguration(string name, object valueDelegate, IConfigurationSource source)
            : base(name, source)
        {
            this.ValueDelegate = valueDelegate;
        }

        #endregion

        /// <summary>
        /// Gets the delegate, this should be a Func&lt;TParam1, TParam2, ..., TParam3&gt;.
        /// </summary>
        public object ValueDelegate { get; init; }

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetAdditionalEqualityComponents()
        {
            yield return this.ValueDelegate;
        }

        #endregion
    }
}