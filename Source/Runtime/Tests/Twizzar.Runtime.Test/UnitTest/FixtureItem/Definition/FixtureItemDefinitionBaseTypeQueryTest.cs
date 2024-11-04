using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Name;
using Twizzar.Runtime.Test.UnitTest.FixtureItem.Builders;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Runtime.Test.UnitTest.FixtureItem.Definition
{
    [TestClass]
    public class FixtureItemDefinitionBaseTypeQueryTest
    {
        private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();

        [TestMethod]
        [DataRow(BaseTypeConfigurationKind.RawValue)]
        [DataRow(BaseTypeConfigurationKind.UniqueValue)]
        public async Task BaseType_configuration_resolve_in_the_correct_definition_node(
            BaseTypeConfigurationKind baseTypeConfigurationKind)
        {
            // Arrange
            var type = typeof(int);
            var name = "name" + RandomString();
            var value = "value" + RandomString();

            var fixtureItemId = FixtureItemId.CreateNamed(name, type.ToTypeFullName());

            var configurationItem = this.GetConfig(baseTypeConfigurationKind, value);

            var typeDesc = new RuntimeTypeDescriptionBuilder()
                .WithTypeFullName(type.ToTypeFullName())
                .AsBaseType()
                .Build();

            var sut = new FixtureItemDefinitionQueryBuilder()
                .WithConfigurationItem(fixtureItemId, configurationItem)
                .WithTypeDescriptionNode(type, typeDesc)
                .Build();

            // Act
            var node = await sut.GetDefinitionNode(fixtureItemId);

            // Assert
            var baseTypeNode = node
                .Should()
                .BeAssignableTo<IBaseTypeNode>()
                .Subject;
            baseTypeNode.FixtureItemId.Should().Be(fixtureItemId);
            baseTypeNode.TypeDescription.Should().Be(typeDesc);

            switch (baseTypeConfigurationKind)
            {
                case BaseTypeConfigurationKind.RawValue:
                    var valueDef = baseTypeNode.ValueDefinition.Should()
                        .BeAssignableTo<IRawValueDefinition>().Subject;
                    valueDef.Value.Should().Be(value);
                    break;
                case BaseTypeConfigurationKind.UniqueValue:
                    baseTypeNode.ValueDefinition.Should()
                        .BeAssignableTo<IUniqueDefinition>();
                    break;
            }
        }

        public enum BaseTypeConfigurationKind
        {
            RawValue,
            UniqueValue,
        }

        private IConfigurationItem GetConfig(
            BaseTypeConfigurationKind kind,
            string value)
        {
            var configurationItemMock = new Mock<IConfigurationItem>();

            switch (kind)
            {
                case BaseTypeConfigurationKind.RawValue:
                    configurationItemMock
                        .Setup(item => item.MemberConfigurations)
                        .Returns(
                            ImmutableDictionary<string, IMemberConfiguration>.Empty.Add(
                                "Value",
                                new ValueMemberConfiguration("Value", value, Source)));
                    break;
                case BaseTypeConfigurationKind.UniqueValue:
                    configurationItemMock
                        .Setup(item => item.MemberConfigurations)
                        .Returns(
                            ImmutableDictionary<string, IMemberConfiguration>.Empty.Add(
                                "Value",
                                new UniqueValueMemberConfiguration("Value", Source)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }

            return configurationItemMock.Object;
        }
    }
}
