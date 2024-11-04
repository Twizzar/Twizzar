using System;
using System.Linq;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Twizzar.Runtime.Core.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.Runtime.Test.UnitTest.FixtureItem.Builders;
using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.TestCommon.Configuration.Builders;
using ViCommon.Functional.Monads.MaybeMonad;
using static Twizzar.TestCommon.TestHelper;

// ReSharper disable UseDeconstruction
#pragma warning disable IDE0042 // Deconstruct variable declaration

namespace Twizzar.Runtime.Test.UnitTest.FixtureItem.Definition
{
    [TestClass]
    [TestCategory("Obsolete")]
    public class FixtureItemDefinitionNodeCreationServiceTest
    {
        private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();

        public enum FixtureDefinitionNodeFactoryMethod
        {
            CreateBaseType,
            CreateInterfaceNode,
            CreateClassNode,
        }

        [TestMethod]
        [DataRow(FixtureDefinitionNodeFactoryMethod.CreateBaseType)]
        [DataRow(FixtureDefinitionNodeFactoryMethod.CreateInterfaceNode)]
        [DataRow(FixtureDefinitionNodeFactoryMethod.CreateClassNode)]
        public void All_arguments_are_null_checked(FixtureDefinitionNodeFactoryMethod methodType)
        {
            // arrange
            var sut = new FixtureItemDefinitionNodeCreationServiceBuilder().Build();
            var typeDescription = new Mock<IRuntimeTypeDescription>().Object;
            var id = RandomNamelessFixtureItemId();
            var configItem = new Mock<IConfigurationItem>().Object;

            Action<IRuntimeTypeDescription, FixtureItemId, IConfigurationItem> sutMethod = methodType switch
            {
                FixtureDefinitionNodeFactoryMethod.CreateBaseType =>
                    (description, itemId, config) =>
                        sut.CreateBaseType(null, id, configItem),
                FixtureDefinitionNodeFactoryMethod.CreateInterfaceNode =>
                    (description, itemId, config) =>
                        sut.CreateInterfaceNode(null, id, configItem),
                FixtureDefinitionNodeFactoryMethod.CreateClassNode =>
                    (description, itemId, config) =>
                        sut.CreateClassNode(null, id, configItem),
                _ => throw new ArgumentOutOfRangeException(nameof(methodType), methodType, null)
            };

            // act
            Action typeDescriptionIsNull = () => sutMethod(null, id, configItem);
            Action fixtureItemIdIsNull = () => sutMethod(typeDescription, null, configItem);
            Action configurationItemIsNull = () => sutMethod(typeDescription, id, null);

            // assert
            typeDescriptionIsNull.Should().Throw<ArgumentNullException>();
            fixtureItemIdIsNull.Should().Throw<ArgumentNullException>();
            configurationItemIsNull.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CreateBaseType_description_of_baseTypeKind_with_uniqueMemberConfig_maps_to_uniqueDefinition()
        {
            // arrange
            var sut = new FixtureItemDefinitionNodeCreationServiceBuilder().Build();
            var typeDescription = new RuntimeTypeDescriptionBuilder()
                .AsBaseType()
                .WithIsNullableBaseType(true)
                .Build();
            var id = RandomNamelessFixtureItemId();
            var configItem = new ConfigurationItemBuilder()
                .AsUniqueBaseType()
                .Build();

            // act
            var baseTypeNodeResult = sut.CreateBaseType(typeDescription, id, configItem);

            // assert
            var baseTypeNode = AssertAndUnwrapSuccess(baseTypeNodeResult);
            baseTypeNode.IsNullable.Should().BeTrue();
            baseTypeNode.TypeDescription.Should().BeEquivalentTo(typeDescription);
            baseTypeNode.ValueDefinition.Should().BeAssignableTo<UniqueDefinition>();
        }

        [TestMethod]
        public void CreateBaseType_description_of_BaseTypeKind_with_valueMemberConfig_maps_to_uniqueDefinition()
        {
            // arrange
            var sut = new FixtureItemDefinitionNodeCreationServiceBuilder().Build();
            var typeDescription = new RuntimeTypeDescriptionBuilder()
                .AsBaseType()
                .WithIsNullableBaseType(true)
                .Build();
            var id = RandomNamelessFixtureItemId();
            var value = RandomString();
            var configItem = new ConfigurationItemBuilder()
                .AsValueBaseType(value)
                .Build();

            // act
            var baseTypeNodeResult = sut.CreateBaseType(typeDescription, id, configItem);

            // assert
            var baseTypeNode = AssertAndUnwrapSuccess(baseTypeNodeResult);
            baseTypeNode.IsNullable.Should().BeTrue();
            baseTypeNode.TypeDescription.Should().BeEquivalentTo(typeDescription);
            var definition = baseTypeNode.ValueDefinition.Should().BeAssignableTo<RawValueDefinition>().Subject;
            definition.Value.Should().Be(value);
        }

        [TestMethod]
        public void CreateBaseType_Non_baseTypeDescription_throws_argumentException()
        {
            // arrange
            var sut = new FixtureItemDefinitionNodeCreationServiceBuilder().Build();
            var typeDescription = new RuntimeTypeDescriptionBuilder()
                .AsClass()
                .Build();
            var id = RandomNamelessFixtureItemId();
            var configItem = new ConfigurationItemBuilder()
                .Build();

            // act
            Action a= () => sut.CreateBaseType(typeDescription, id, configItem);

            // assert
            a.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void CreateBaseType_More_than_one_MemberConfiguration_returns_InvalidConfigurationFailure()
        {
            // arrange
            var sut = new FixtureItemDefinitionNodeCreationServiceBuilder().Build();
            var typeDescription = new RuntimeTypeDescriptionBuilder()
                .AsBaseType()
                .Build();
            var id = RandomNamelessFixtureItemId();
            var configItem = new ConfigurationItemBuilder()
                .WithMemberConfiguration(new []
                {
                    new UniqueValueMemberConfiguration(RandomString(), Source),
                    new UniqueValueMemberConfiguration(RandomString(), Source),
                })
                .Build();

            // act
            var result = sut.CreateBaseType(typeDescription, id, configItem);

            // assert
            result.IsFailure.Should().BeTrue();
            var failure = result.GetFailureUnsafe().Should().BeOfType<InvalidConfigurationFailure>().Subject;
            failure.ConfigurationItem.Should().Be(configItem);
        }

        [TestMethod]
        public void CreateClassNode_ctor_parameter_are_set_in_classNode()
        {
            // arrange
            var builder = new FixtureItemDefinitionNodeCreationServiceBuilder();
            var ctorSelector = builder.CtorSelectorMock;
            var sut = builder.Build();

            var prop1 = (name: RandomString(), value: RandomString());
            var prop2 = (name: RandomString(), value: RandomString());

            var memberCtorParameters = new[]
            {
                new ValueMemberConfiguration(prop1.name, prop1.value, Source),
                new ValueMemberConfiguration(prop2.name, prop2.value, Source),
            };

            var typeDescription = new RuntimeTypeDescriptionBuilder()
                .AsClass()
                .WithDeclaredConstructorsParams(new []{ prop1.name, prop2.name})
                .Build();

            ctorSelector.Setup(s => s.FindCtor(It.IsAny<IConfigurationItem>(), typeDescription))
                .Returns(Maybe.Some(typeDescription.GetDeclaredConstructors().FirstOrDefault()));

            var id = RandomNamelessFixtureItemId();
            var configItem = new ConfigurationItemBuilder()
                .WithCtorParameters(memberCtorParameters)
                .Build();

            // act
            var result = sut.CreateClassNode(typeDescription, id, configItem);

            // assert
            var node = AssertAndUnwrapSuccess(result);
            node.ConstructorParameters.Should().HaveCount(2);
            node.ConstructorParameters.Should().Contain(definition => definition.Name == prop1.name);
            node.ConstructorParameters.Should().Contain(definition => definition.Name == prop2.name);
        }
    }
}