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
    /// Defines a method of the fixture item.
    /// </summary>
    public class MethodDefinition : MemberDefinition, IMethodDefinition, IHasEnsureHelper, IHasLogger
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodDefinition"/> class.
        /// </summary>
        /// <param name="methodConfiguration">The member configuration.</param>
        /// <param name="methodDescription">Property description for the property.</param>
        /// <param name="callbacks"></param>
        public MethodDefinition(
            MethodConfiguration methodConfiguration,
            IRuntimeMethodDescription methodDescription,
            IEnumerable<object> callbacks)
        {
            this.EnsureMany()
                .Parameter(methodConfiguration, nameof(methodConfiguration))
                .Parameter(methodDescription, nameof(methodDescription))
                .ThrowWhenNull();

            this.EnsureParameter(methodConfiguration, nameof(methodConfiguration))
                .IsTrue(
                    configuration => configuration.ReturnType == typeof(object).FullName ||
                                     configuration.ReturnType == methodDescription.TypeFullName.FullName ||
                                     methodDescription.IsGeneric,
                    "The MethodConfiguration return type must be equal to the MethodDescription TypeFullName")
                .ThrowOnFailure();

            this.ValueDefinition = CreateValueDefinition(
                methodConfiguration.ReturnValue,
                methodDescription.Type);

            this.MethodDescription = methodDescription;
            this.Callbacks = callbacks;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public IRuntimeMethodDescription MethodDescription { get; }

        /// <inheritdoc />
        public IEnumerable<object> Callbacks { get; }

        /// <inheritdoc />
        public override IValueDefinition ValueDefinition { get; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.MethodDescription;
            yield return this.ValueDefinition;
        }

        #endregion
    }
}