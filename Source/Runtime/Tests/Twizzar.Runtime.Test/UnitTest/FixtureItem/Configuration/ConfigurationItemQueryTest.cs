using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Runtime.Test.UnitTest.FixtureItem.Builders;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using Twizzar.TestCommon.TypeDescription.Builders;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Runtime.Test.UnitTest.FixtureItem.Configuration
{
    [TestClass]
    public class ConfigurationItemQueryTest
    {
        [TestMethod]
        public async Task Query_returns_user_config_when_exists_for_fixtureItemId()
        {
            // Arrange
            var name = RandomString();
            var type = RandomTypeFullName();
            var id = FixtureItemId.CreateNamed(name, type);
            var value = RandomString();

            var typeDescription = new TypeDescriptionBuilder()
                .WithTypeFullName(type)
                .AsBaseType()
                .WithFixtureKind(FixtureKind.BaseType)
                .Build();

            var sut = new ConfigurationItemQueryBuilder(name, type.FullName, id)
                .WithSimpleSystemDefault(value, typeDescription)
                .WithNamedSimpleUserConfiguration(value)
                .Build();

            // Act
            var configuration = await sut.GetConfigurationItem(id, typeDescription);

            // Assert
            configuration.Id.Name.GetValueUnsafe().Should().Be(name);
            configuration.Id.TypeFullName.Should().Be(type);

            var memberConfiguration = configuration
                .MemberConfigurations.Should().ContainSingle().Subject.Value;

            var valueConfig = memberConfiguration.Should().BeOfType<ValueMemberConfiguration>().Subject;
            valueConfig.Value.Should().Be(value);
        }

        [TestMethod]
        public async Task Query_returns_system_default_config_when_no_user_config_for_id_and_type_exists()
        {
            // Arrange
            var name = RandomString();
            var type = RandomTypeFullName();
            var id = FixtureItemId.CreateNamed(name, type);
            var value = RandomString();

            var typeDescription = new TypeDescriptionBuilder()
                .WithTypeFullName(type)
                .WithFixtureKind(FixtureKind.BaseType)
                .Build();

            var sut = new ConfigurationItemQueryBuilder(name, type.FullName, id)
                .WithSimpleSystemDefault(value, typeDescription)
                .Build();

            // Act
            var configuration = await sut.GetConfigurationItem(id, typeDescription);

            // Assert
            configuration.Id.Name.GetValueUnsafe().Should().Be(name);
            configuration.Id.TypeFullName.Should().Be(type);

            var memberConfiguration = configuration
                .MemberConfigurations.Should().ContainSingle().Subject.Value;

            var valueConfig = memberConfiguration.Should().BeOfType<ValueMemberConfiguration>().Subject;
            valueConfig.Value.Should().Be(value);
        }
    }
}
