using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Query.Services.ReadModel
{
    /// <summary>
    /// Model for a member of a mock or class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FixtureItemMemberModel : ValueObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemMemberModel"/> class.
        /// </summary>
        /// <param name="configuration">The member configuration.</param>
        /// <param name="description">The member description.</param>
        public FixtureItemMemberModel(IMemberConfiguration configuration, IMemberDescription description)
        {
            this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        /// <summary>
        /// Gets the configuration of the member.
        /// </summary>
        public IMemberConfiguration Configuration { get; }

        /// <summary>
        /// Gets the description of the member.
        /// </summary>
        public IMemberDescription Description { get; }

        /// <summary>
        /// Construct a new <see cref="FixtureItemMemberModel"/> form this.
        /// </summary>
        /// <param name="configuration">The new configuration.</param>
        /// <param name="description">The new description.</param>
        /// <returns>A new instance uses all somes declared as parameter else uses the properties of this.</returns>
        public FixtureItemMemberModel With(
            Maybe<IMemberConfiguration> configuration = default,
            Maybe<IMemberDescription> description = default) =>
                new(
                    configuration.SomeOrProvided(this.Configuration),
                    description.SomeOrProvided(this.Description));

        #region Overrides of ValueObject

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Configuration;
            yield return this.Description;
        }

        #endregion
    }
}
