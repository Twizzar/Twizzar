using System;
using Twizzar.Runtime.Core.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.Resources;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Runtime.Core.FixtureItem.Definition.MemberDefinitions
{
    /// <summary>
    /// Defines a member of the fixture item.
    /// </summary>
    public abstract class MemberDefinition : ValueObject, IMemberDefinition
    {
        #region Properties

        /// <summary>
        /// Gets the value definition which describes how the value is constructed.
        /// </summary>
        public abstract IValueDefinition ValueDefinition { get; }

        #endregion

        #region static methods

        /// <summary>
        /// Match the member configuration to a value definition.
        /// </summary>
        /// <param name="memberConfiguration">A member configuration.</param>
        /// <param name="type">The type of the member.</param>
        /// <returns>A new value definition.</returns>
        protected static IValueDefinition CreateValueDefinition(
            IMemberConfiguration memberConfiguration,
            Type type)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(memberConfiguration, nameof(memberConfiguration))
                .Parameter(type, nameof(type))
                .ThrowWhenNull();

            if (!type.CanBeNull() && memberConfiguration is NullValueMemberConfiguration)
            {
                throw new InvalidConfigurationException(
                    string.Format(ErrorMessagesRuntime.The_type__0__is_not_nullable_, type.FullName),
                    memberConfiguration);
            }

            return memberConfiguration switch
            {
                LinkMemberConfiguration x => new LinkDefinition(
                    x.ConfigurationLink),

                UndefinedMemberConfiguration _ =>
                    new UndefinedDefinition(),

                ValueMemberConfiguration x =>
                    new RawValueDefinition(x.Value),

                UniqueValueMemberConfiguration _ =>
                    new UniqueDefinition(),

                NullValueMemberConfiguration _ =>
                    new NullValueDefinition(),

                DelegateValueMemberConfiguration x =>
                    new DelegateValueDefinition(x.ValueDelegate),

                _ => throw new ArgumentException(
                    $"configuration type is not supported by {nameof(MemberDefinition)}",
                    nameof(memberConfiguration)),
            };
        }

        #endregion
    }
}
