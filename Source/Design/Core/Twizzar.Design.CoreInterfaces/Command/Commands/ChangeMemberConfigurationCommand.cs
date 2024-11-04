using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;

namespace Twizzar.Design.CoreInterfaces.Command.Commands
{
    /// <summary>
    /// Command to request a change of a member configuration.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ChangeMemberConfigurationCommand : ValueObject, ICommand<ChangeMemberConfigurationCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeMemberConfigurationCommand"/> class.
        /// </summary>
        /// <param name="fixtureItemId">The fixture item id.</param>
        /// <param name="memberConfiguration">The new member configuration.</param>
        public ChangeMemberConfigurationCommand(FixtureItemId fixtureItemId, IMemberConfiguration memberConfiguration)
        {
            this.FixtureItemId = fixtureItemId ?? throw new ArgumentNullException(nameof(fixtureItemId));
            this.MemberConfiguration = memberConfiguration ?? throw new ArgumentNullException(nameof(memberConfiguration));
        }

        /// <summary>
        /// Gets the fixture item id.
        /// </summary>
        public FixtureItemId FixtureItemId { get; }

        /// <summary>
        /// Gets the new member configuration.
        /// </summary>
        public IMemberConfiguration MemberConfiguration { get; }

        #region Overrides of ValueObject

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.FixtureItemId;
            yield return this.MemberConfiguration;
        }

        #endregion
    }
}
