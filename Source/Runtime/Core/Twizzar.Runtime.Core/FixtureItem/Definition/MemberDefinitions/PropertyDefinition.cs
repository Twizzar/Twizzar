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
    /// Defines a property of the fixture item.
    /// </summary>
    public class PropertyDefinition : MemberDefinition, IPropertyDefinition, IHasEnsureHelper, IHasLogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDefinition"/> class.
        /// </summary>
        /// <param name="configuration">The member configuration.</param>
        /// <param name="propertyDescription">Property description for the property.</param>
        public PropertyDefinition(
            IMemberConfiguration configuration,
            IRuntimePropertyDescription propertyDescription)
        {
            this.EnsureMany()
                .Parameter(configuration, nameof(configuration))
                .Parameter(propertyDescription, nameof(propertyDescription))
                .ThrowOnFailure();

            this.Name = configuration.Name;
            this.ValueDefinition = CreateValueDefinition(configuration, propertyDescription.Type);
            this.PropertyDescription = propertyDescription;
        }

        #region Properties

        /// <inheritdoc />
        public IRuntimePropertyDescription PropertyDescription { get; }

        /// <summary>
        /// Gets the name of the property.
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
            yield return this.PropertyDescription;
        }

        #endregion
    }
}
