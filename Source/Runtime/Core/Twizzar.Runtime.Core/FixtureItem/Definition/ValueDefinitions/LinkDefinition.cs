using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Runtime.Core.FixtureItem.Definition.ValueDefinitions
{
    /// <summary>
    /// The value is defined in another FixtureItemDefinitionNode.
    /// </summary>
    public record LinkDefinition : ILinkDefinition, IHasLogger, IHasEnsureHelper
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkDefinition"/> class.
        /// </summary>
        /// <param name="link">ItemId to another Fixture Item.</param>
        public LinkDefinition(FixtureItemId link)
        {
            this.EnsureParameter(link, nameof(link)).ThrowWhenNull();

            this.Link = link;
        }

        #endregion

        #region properties

        /// <inheritdoc/>
        public FixtureItemId Link { get; init; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public ILinkDefinition WithLink(FixtureItemId id) => this with { Link = id };

        #endregion
    }
}