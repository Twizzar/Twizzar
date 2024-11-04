using System.Collections.Generic;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Runtime.Core.FixtureItem.Definition.MemberDefinitions
{
    /// <summary>
    /// Defines a field of the fixture item.
    /// </summary>
    public class FieldDefinition : MemberDefinition, IFieldDefinition, IHasEnsureHelper, IHasLogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDefinition"/> class.
        /// </summary>
        /// <param name="configuration">The member configuration.</param>
        /// <param name="fieldDescription">Field description for the field.</param>
        public FieldDefinition(
            IMemberConfiguration configuration,
            IRuntimeFieldDescription fieldDescription)
        {
            this.EnsureMany()
                .Parameter(configuration, nameof(configuration))
                .Parameter(fieldDescription, nameof(fieldDescription))
                .ThrowOnFailure();

            this.Name = configuration.Name;
            this.ValueDefinition = CreateValueDefinition(configuration, fieldDescription.Type);
            this.FieldDescription = fieldDescription;
        }

        #region Properties

        /// <inheritdoc />
        public IRuntimeFieldDescription FieldDescription { get; }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc />
        public override IValueDefinition ValueDefinition { get; }

        #region IHasLogger and IHasEnsureHelper implemenation

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #endregion

        #region Overrides of ValueObject

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Name;
            yield return this.ValueDefinition;
            yield return this.FieldDescription;
        }

        #endregion
    }
}
