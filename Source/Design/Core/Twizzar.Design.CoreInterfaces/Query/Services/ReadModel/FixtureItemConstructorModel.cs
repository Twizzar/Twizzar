using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.Resources;
using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

using ILogger = Twizzar.SharedKernel.NLog.Interfaces.ILogger;

namespace Twizzar.Design.CoreInterfaces.Query.Services.ReadModel
{
    /// <summary>
    /// Describes the configured constructor.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FixtureItemConstructorModel : ValueObject, IHasEnsureHelper, IHasLogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemConstructorModel"/> class.
        /// </summary>
        /// <param name="parameters">The parameter models.</param>
        /// <param name="methodDescription">The method description.</param>
        /// <param name="configuration">The ctor member configuration.</param>
        public FixtureItemConstructorModel(
            ImmutableArray<FixtureItemParameterModel> parameters,
            IMethodDescription methodDescription,
            CtorMemberConfiguration configuration)
        {
            this.Parameters = parameters;
            this.MethodDescription = methodDescription;
            this.Configuration = configuration;
        }

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        /// <summary>
        /// Gets the models for the parameters.
        /// </summary>
        public ImmutableArray<FixtureItemParameterModel> Parameters { get; }

        /// <summary>
        /// Gets the method description.
        /// </summary>
        public IMethodDescription MethodDescription { get; }

        /// <summary>
        /// Gets the ctor member configuration.
        /// </summary>
        public CtorMemberConfiguration Configuration { get; }

        #region Overrides of ValueObject

        /// <summary>
        /// Creates a new Constructor model with the parameter at the given position replaced with the new one.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="fixtureItemParameterModel">The fixture item parameter model.</param>
        /// <returns>A new <see cref="FixtureItemConstructorModel"/>.</returns>
        public FixtureItemConstructorModel WithParameter(int position, FixtureItemParameterModel fixtureItemParameterModel)
        {
            this.EnsureParameter(position, nameof(position))
                .IsInRange(
                    0,
                    this.Parameters.Length,
                    param => new ArgumentOutOfRangeException(
                        param,
                        string.Format(ErrorMessages.Parameter_Is_not_in_range_of__0_, nameof(this.Parameters))))
                .ThrowOnFailure();
            this.EnsureParameter(fixtureItemParameterModel, nameof(fixtureItemParameterModel)).ThrowWhenNull();

            return new FixtureItemConstructorModel(
                this.Parameters.SetItem(position, fixtureItemParameterModel),
                this.MethodDescription,
                this.Configuration);
        }

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Parameters;
        }

        #endregion
    }
}
