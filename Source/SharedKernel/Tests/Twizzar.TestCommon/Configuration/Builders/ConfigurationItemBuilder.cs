using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Moq;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using static ViCommon.Functional.FunctionalCommon;

namespace Twizzar.TestCommon.Configuration.Builders
{
    /// <summary>
    /// Builder for configuration items, helpful for testing with specific behavior of configuration items.
    /// </summary>
    public class ConfigurationItemBuilder
    {
        private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();
        private readonly Mock<IConfigurationItem> _mock;
        private readonly Dictionary<string, IMemberConfiguration> _memberConfigs = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationItemBuilder"/> class.
        /// </summary>
        public ConfigurationItemBuilder()
        {
            this._mock = new Mock<IConfigurationItem>();
            this._mock.Setup(item => item.FixtureConfigurations)
                .Returns(ImmutableDictionary<string, IFixtureConfiguration>.Empty);
            this._mock.Setup(item => item.MemberConfigurations)
                .Returns(ImmutableDictionary<string, IMemberConfiguration>.Empty);
        }

        public ConfigurationItemBuilder WithId(FixtureItemId id)
        {
            this._mock.Setup(item => item.Id)
                .Returns(id);
            return this;
        }


        public ConfigurationItemBuilder AsUniqueBaseType() =>
            this.WithMemberConfiguration(new UniqueValueMemberConfiguration(ConfigurationConstants.BaseTypeMemberName, Source));

        public ConfigurationItemBuilder WithMemberConfiguration(
            params IMemberConfiguration[] memberConfigurations)
        {
            this._memberConfigs.AddRange(
                memberConfigurations
                    .Select(
                        configuration =>
                            new KeyValuePair<string, IMemberConfiguration>(configuration.Name, configuration)));

            return this;
        }

        public ConfigurationItemBuilder WithPropertyMember(IMemberConfiguration[] memberConfigurations)
        {
            this._mock.Setup(item => item.OnlyVariableMemberConfiguration)
                .Returns(memberConfigurations.ToImmutableDictionary(configuration => configuration.Name, Identity));

            this.WithMemberConfiguration(memberConfigurations);
            return this;
        }

        public ConfigurationItemBuilder AsValueBaseType(string value) =>
            this.WithMemberConfiguration(new ValueMemberConfiguration(ConfigurationConstants.BaseTypeMemberName, value, Source));

        public ConfigurationItemBuilder WithCtorParameters(IMemberConfiguration[] ctorParameters, bool createMemberConfig = true)
        {
            this._mock.Setup(item => item.OnlyCtorParameterMemberConfigurations)
                .Returns(ctorParameters.ToImmutableDictionary(configuration => configuration.Name, Identity));

            if (ctorParameters is { Length: > 0 } && createMemberConfig)
            {
                this._memberConfigs.Add(
                    ConfigurationConstants.CtorMemberName, new CtorMemberConfiguration(ctorParameters.ToImmutableArray(), ImmutableArray<ITypeFullName>.Empty, Source));
            }

            return this;
        }

        public IConfigurationItem Build()
        {
            this._mock.Setup(item => item.MemberConfigurations)
                .Returns(this._memberConfigs.ToImmutableDictionary);
            return this._mock.Object;
        }
    }
}
