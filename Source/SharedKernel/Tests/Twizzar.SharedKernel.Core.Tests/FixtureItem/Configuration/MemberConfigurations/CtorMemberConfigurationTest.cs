using System;
using System.Collections.Immutable;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.SharedKernel.Core.Tests.FixtureItem.Configuration.MemberConfigurations
{
    [TestFixture]
    public class CtorMemberConfigurationTest
    {
        private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();

        [Test]
        public void WithParameter_change_on_valid_position_returns_a_correct_new_config()
        {
            // arrange
            var name2 = RandomString();

            var paramConfigs = ImmutableArray.Create<IMemberConfiguration>()
                .Add(new ValueMemberConfiguration(RandomString(), RandomString(), Source))
                .Add(new ValueMemberConfiguration(name2, RandomString(), Source));

            var sut = new CtorMemberConfiguration(paramConfigs, ImmutableArray<ITypeFullName>.Empty, Source);

            // act
            var newConfig = sut.WithParameter(name2, new UniqueValueMemberConfiguration(name2, Source));

            // assert
            newConfig.ConstructorParameters.Should().HaveCount(2);
            newConfig.ConstructorParameters[paramConfigs[0].Name].Should().Be(sut.ConstructorParameters[paramConfigs[0].Name]);
            newConfig.ConstructorParameters[paramConfigs[1].Name].Should().BeOfType<UniqueValueMemberConfiguration>();
            newConfig.ConstructorParameters[paramConfigs[1].Name].Name.Should().Be(name2);
            newConfig.Should().NotBeEquivalentTo(sut);
        }

        [Test]
        public void WithParameter_when_new_parameterConfig_is_null_throws_ArgumentNullException()
        {
            // arrange
            var sut = new CtorMemberConfiguration(
                ImmutableArray.Create<IMemberConfiguration>()
                    .Add(new UniqueValueMemberConfiguration(RandomString(), Source)),
                ImmutableArray<ITypeFullName>.Empty,
                Source);

            // act
            Action action = () => sut.WithParameter(string.Empty, null);

            // assert
            action.Should().Throw<ArgumentNullException>();
        }
    }
}