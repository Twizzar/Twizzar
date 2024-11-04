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
    /// The model for parameters of a method or constructor.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FixtureItemParameterModel : ValueObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemParameterModel"/> class.
        /// </summary>
        /// <param name="configuration">The configuration of the parameter.</param>
        /// <param name="description">The description of the parameter.</param>
        public FixtureItemParameterModel(IMemberConfiguration configuration, IParameterDescription description)
        {
            this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        /// <summary>
        /// Gets the configuration of the parameter.
        /// </summary>
        public IMemberConfiguration Configuration { get; }

        /// <summary>
        /// Gets the description of the parameter.
        /// </summary>
        public IParameterDescription Description { get; }

        /// <summary>
        /// Construct a new <see cref="BaseTypeFixtureItemModel"/> form this.
        /// </summary>
        /// <param name="configuration">The new configuration.</param>
        /// <param name="description">The new description.</param>
        /// <returns>A new instance uses all somes declared as parameter else uses the properties of this.</returns>
        public FixtureItemParameterModel With(
            Maybe<IMemberConfiguration> configuration = default,
            Maybe<IParameterDescription> description = default) =>
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
