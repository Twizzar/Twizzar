using System;
using System.Collections.Immutable;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.SharedKernel.Core.FixtureItem.Configuration;
using Twizzar.SharedKernel.Core.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.ResultMonad;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.SharedKernel.Core.Tests.FixtureItem.Configuration.Services
{
    [Category("TwizzarInternal")]
    public partial class SystemDefaultServiceTests
    {
        #region members

        [Test]
        public void
            GetDefaultConfigurationItem_from_basetype_TypeDescriptionNode_returns_a_default_configurationItem_of_type_UniqueValueConfiguration()
        {
            // Arrange
            var factory = SetupFactory();

            var sut = new ItemBuilder<SystemDefaultService>()
                .With(p => p.Ctor.configurationItemFactory.Value(factory.Object))
                .Build();

            var typeDescriptionNode = new BaseTypeDescriptionBuilder()
                .With(p => p.TypeFullName.Value(RandomTypeFullName()))
                .Build();

            // Act
            var result = sut.GetDefaultConfigurationItem(typeDescriptionNode, Guid.NewGuid().ToString());

            // Assert
            var config = AssertAndUnwrapSuccess(result);
            config.MemberConfigurations.Should().HaveCount(1);
            config.MemberConfigurations.Keys.Should().Contain(ConfigurationConstants.BaseTypeMemberName);

            config.MemberConfigurations[ConfigurationConstants.BaseTypeMemberName]
                .Should()
                .BeOfType<UniqueValueMemberConfiguration>();
        }

        [Test]
        public void
            Default_configuration_for_a_class_constructor_baseType_parameters_are_UniqueValueConfiguration_and_other_are_LinkMemberConfiguration()
        {
            // Arrange
            var parameterDescriptions = ImmutableArray.Create<IParameterDescription>()
                .Add(new ItemBuilder<IParameterDescription>()
                    .With(p => p.Name.Value("basetype"))
                    .With(p => p.IsBaseType.Value(true))
                    .With(p => p.TypeFullName.Value(RandomTypeFullName()))
                    .Build())
                .Add(new ItemBuilder<IParameterDescription>()
                    .With(p => p.Name.Value("notbaseType"))
                    .With(p => p.IsBaseType.Value(false))
                    .With(p => p.TypeFullName.Value(RandomTypeFullName()))
                    .Build());

            var ctor = new CtorMethodDescriptionBuilder()
                .With(p => p.DeclaredParameters.Value(parameterDescriptions))
                .Build();

            var ctorSelector = new ItemBuilder<ICtorSelector>()
                .With(p => p.GetCtorDescription
                    .Value(Result.Success(ctor)))
                .Build();

            var typeDescriptionNode = new ClassITypeDescriptionBuilder()
                .With(p => p.GetDeclaredConstructors.Value(ImmutableArray<IMethodDescription>.Empty.Add(ctor)))
                .With(p => p.TypeFullName.Value(RandomTypeFullName()))
                .Build();

            var factory = SetupFactory();

            var sut = new ItemBuilder<SystemDefaultService>()
                .With(p => p.Ctor.configurationItemFactory.Value(factory.Object))
                .With(p => p.Ctor.ctorSelector.Value(ctorSelector))
                .Build();

            // Act
            var result = sut.GetDefaultConfigurationItem(typeDescriptionNode, RandomString());

            // Assert
            var config = AssertAndUnwrapSuccess(result);
            config.OnlyCtorParameterMemberConfigurations.Should().HaveCount(parameterDescriptions.Length);

            config.OnlyCtorParameterMemberConfigurations["basetype"]
                .Should()
                .BeOfType<UniqueValueMemberConfiguration>();

            config.OnlyCtorParameterMemberConfigurations["notbaseType"].Should().BeOfType<LinkMemberConfiguration>();
        }

        private static Mock<IConfigurationItemFactory> SetupFactory()
        {
            var factory = new Mock<IConfigurationItemFactory>();

            factory.Setup(factory => factory.CreateConfigurationItem(
                    It.IsAny<FixtureItemId>(),
                    It.IsAny<IImmutableDictionary<string, IFixtureConfiguration>>(),
                    It.IsAny<IImmutableDictionary<string, IMemberConfiguration>>(),
                    It.IsAny<IImmutableDictionary<string, IImmutableList<object>>>())
                )
                .Returns<FixtureItemId, IImmutableDictionary<string, IFixtureConfiguration>,
                    IImmutableDictionary<string, IMemberConfiguration>,
                    IImmutableDictionary<string, IImmutableList<object>>>(
                    (id, configurations, memberConfigurations, callbacks) =>
                        new ConfigurationItem(id, configurations, memberConfigurations, callbacks));

            return factory;
        }

        #endregion
    }
}