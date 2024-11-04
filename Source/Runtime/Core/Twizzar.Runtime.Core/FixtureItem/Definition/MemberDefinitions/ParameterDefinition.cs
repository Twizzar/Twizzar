using System.Collections.Generic;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Runtime.Core.FixtureItem.Definition.MemberDefinitions
{
    /// <summary>
    /// Defines a parameter of the fixture type.
    /// </summary>
    public sealed class ParameterDefinition : MemberDefinition, IParameterDefinition, IHasLogger, IHasEnsureHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterDefinition"/> class.
        /// </summary>
        /// <param name="memberConfiguration">The member configuration.</param>
        /// <param name="parameterDescription">Parameter description for the parameter.</param>
        public ParameterDefinition(
            IMemberConfiguration memberConfiguration,
            IRuntimeParameterDescription parameterDescription)
        {
            this.EnsureMany()
                .Parameter(memberConfiguration, nameof(memberConfiguration))
                .Parameter(parameterDescription, nameof(parameterDescription))
                .ThrowWhenNull();

            this.Name = memberConfiguration.Name;
            this.ValueDefinition = CreateValueDefinition(memberConfiguration, parameterDescription.Type);
            this.ParameterDescription = parameterDescription;
        }

        #region Properties

        /// <inheritdoc />
        public IRuntimeParameterDescription ParameterDescription { get; }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc />
        public override IValueDefinition ValueDefinition { get; }

        #region IHasLogger and IHasEnsureHelper implementations

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #endregion

        #region Overrides of ValueObject

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Name;
            yield return this.ValueDefinition;
            yield return this.ParameterDescription;
        }

        #endregion
    }
}
