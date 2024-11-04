using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.CoreInterfaces.Query.Services.ReadModel
{
    /// <inheritdoc cref="IFixtureItemModel" />
    [ExcludeFromCodeCoverage]
    public class BaseTypeFixtureItemModel : ValueObject, IFixtureItemModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTypeFixtureItemModel"/> class.
        /// </summary>
        /// <param name="id">The fixture item id.</param>
        /// <param name="fixtureConfigurations"><see cref="IFixtureConfiguration"/> used for general fixture item configurations.</param>
        /// <param name="value">The value.</param>
        /// <param name="typeDescription">The type description.</param>
        public BaseTypeFixtureItemModel(
            FixtureItemId id,
            IImmutableDictionary<string, IFixtureConfiguration> fixtureConfigurations,
            IMemberConfiguration value,
            ITypeDescription typeDescription)
        {
            this.Id = id ?? throw new NullReferenceException(nameof(id));
            this.FixtureConfigurations = fixtureConfigurations ?? throw new NullReferenceException(nameof(fixtureConfigurations));
            this.Value = value ?? throw new NullReferenceException(nameof(value));
            this.Description = typeDescription ?? throw new NullReferenceException(nameof(typeDescription));
        }

        #region Implementation of IFixtureItemModel

        /// <inheritdoc />
        public FixtureItemId Id { get; }

        /// <inheritdoc />
        public IImmutableDictionary<string, IFixtureConfiguration> FixtureConfigurations { get; }

        /// <inheritdoc />
        public ITypeDescription Description { get; }

        #endregion

        /// <summary>
        /// Gets the configured value.
        /// </summary>
        public IMemberConfiguration Value { get; }

        /// <summary>
        /// Construct a new <see cref="BaseTypeFixtureItemModel"/> form this.
        /// </summary>
        /// <param name="id">New id.</param>
        /// <param name="fixtureConfigurations">New fixtureConfigurations.</param>
        /// <param name="value">New value.</param>
        /// <returns>A new instance uses all somes declared as parameter else uses the properties of this.</returns>
        public BaseTypeFixtureItemModel With(
            Maybe<FixtureItemId> id = default,
            Maybe<IImmutableDictionary<string, IFixtureConfiguration>> fixtureConfigurations = default,
            Maybe<IMemberConfiguration> value = default) =>
                new(
                    id.SomeOrProvided(this.Id),
                    fixtureConfigurations.SomeOrProvided(this.FixtureConfigurations),
                    value.SomeOrProvided(this.Value),
                    this.Description);

        #region Overrides of ValueObject

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Id;
            yield return this.FixtureConfigurations;
            yield return this.Value;
        }

        #endregion
    }
}
