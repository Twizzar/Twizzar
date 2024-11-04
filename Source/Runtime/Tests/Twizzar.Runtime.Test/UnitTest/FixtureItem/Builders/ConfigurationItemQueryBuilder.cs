using System.Collections.Immutable;
using System.Threading.Tasks;
using Moq;
using Twizzar.Runtime.Core.FixtureItem.Configuration.Services;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.Core.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon.Builder;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;
using static ViCommon.Functional.Monads.ResultMonad.Result;

namespace Twizzar.Runtime.Test.UnitTest.FixtureItem.Builders
{
    public class ConfigurationItemQueryBuilder
    {
        private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();

        private readonly FixtureItemId _id;
        private readonly string _name;
        private readonly string _type;

        private readonly Mock<IUserConfigurationQuery> _userConfigurationQueryMock;
        private readonly Mock<ISystemDefaultService> _systemDefaultServiceMock;

        public ConfigurationItemQueryBuilder(string name, string type, FixtureItemId id)
        {
            this._id = id;
            this._name = name;
            this._type = type;

            var userConfigurationQueryMock = new Mock<IUserConfigurationQuery>();
            userConfigurationQueryMock
                .Setup(query => query.GetNamedConfig(id))
                .Returns(() => Task.FromResult(None<IConfigurationItem>()));

            this._userConfigurationQueryMock = userConfigurationQueryMock;

            this._systemDefaultServiceMock = new Mock<ISystemDefaultService>();
        }

        public ConfigurationItemQueryBuilder WithNamedSimpleUserConfiguration(string value)
        {
            var namedConfigurationItem = this.GetSimpleValueConfiguration(value);

            this._userConfigurationQueryMock
                .Setup(query => query.GetNamedConfig(this._id))
                .Returns(() => Task.FromResult(Some(namedConfigurationItem)));

            return this;
        }

        public ConfigurationItemQueryBuilder WithSimpleSystemDefault(string value, ITypeDescription typeDescription)
        {
            var configurationItem = this.GetSimpleValueConfiguration(value);

            this._systemDefaultServiceMock
                .Setup(service => service.GetDefaultConfigurationItem(typeDescription, this._id.RootItemPath))
                .Returns(Success(configurationItem));

            return this;
        }

        public ConfigurationItemQueryBuilder WithSystemDefaultConfig(
            IConfigurationItem configuration,
            ITypeDescription typeDescription,
            string projectName)
        {
            this._systemDefaultServiceMock.Setup(
                    service =>
                        service.GetDefaultConfigurationItem(typeDescription, projectName))
                .Returns(() => Success(configuration));

            return this;
        }

        public IConfigurationItemQuery Build() =>
            new ConfigurationItemQuery(
                this._userConfigurationQueryMock.Object,
                this._systemDefaultServiceMock.Object);

        private IConfigurationItem GetSimpleValueConfiguration(string value)
        {
            var configItem = new ConfigurationItem(
                FixtureItemId.CreateNamed(this._name, new TypeFullNameBuilder(this._type).Build()),
                ImmutableDictionary<string, IFixtureConfiguration>.Empty,
                ImmutableDictionary<string, IMemberConfiguration>.Empty
                    .Add(
                        ConfigurationConstants.BaseTypeMemberName,
                        new ValueMemberConfiguration(this._name, value, Source)),
                ImmutableDictionary.Create<string, IImmutableList<object>>());

            return configItem;
        }
    }
}
