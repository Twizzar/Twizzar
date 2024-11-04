using System.Collections.Generic;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations
{
    /// <summary>
    /// Configuration item which describes a link to another named user configuration.
    /// </summary>
    public sealed record LinkMemberConfiguration : MemberConfiguration<LinkMemberConfiguration>,
        IHasLogger,
        IHasEnsureHelper
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkMemberConfiguration"/> class.
        /// </summary>
        /// <param name="name">The member name.</param>
        /// <param name="configurationLink">The link to another configuration.</param>
        /// <param name="source"></param>
        public LinkMemberConfiguration(string name, FixtureItemId configurationLink, IConfigurationSource source)
            : base(name, source)
        {
            this.EnsureParameter(configurationLink, nameof(configurationLink)).ThrowWhenNull();
            this.ConfigurationLink = configurationLink;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets a link to another configuration.
        /// </summary>
        public FixtureItemId ConfigurationLink { get; init; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public override string ToString() =>
            $"Link({this.Name} -> {this.ConfigurationLink})";

        /// <inheritdoc />
        protected override IEnumerable<object> GetAdditionalEqualityComponents()
        {
            yield return this.ConfigurationLink;
        }

        #endregion
    }
}