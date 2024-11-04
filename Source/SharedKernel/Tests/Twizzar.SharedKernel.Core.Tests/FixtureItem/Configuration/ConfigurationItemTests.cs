using System.Collections.Immutable;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;
using Twizzar.SharedKernel.Core.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;

namespace Twizzar.SharedKernel.Core.Tests.FixtureItem.Configuration
{
    [Category("TwizzarInternal")]
    public partial class ConfigurationItemTests
    {
        private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();

        [Test]
        public void Callbacks_can_be_added_and_immutable_state_is_preserved()
        {
            var key = TestHelper.RandomString();
            var callback = () => 5;

            var config = new ItemBuilder<ConfigurationItem>()
                .With(p => p.Ctor.callbacks.Value(ImmutableDictionary.Create<string, IImmutableList<object>>()))
                .Build();

            var newConfig = config.AddCallbacks(key, new []{ callback });

            config.Callbacks.Should().HaveCount(0);
            newConfig.Callbacks.Should().HaveCount(1);

            newConfig.Callbacks[key].Should().Contain(callback);
        }

        [Test]
        public void CtorParameter_get_merged_correctly()
        {
            // arrange
            var aParametersNames = new ItemBuilder<string>().BuildMany(3).ToArray();
            var bParametersNames = new[] { aParametersNames[1] };

            var aParameters = aParametersNames.Select(
                    x => Mock.Of<IMemberConfiguration>(configuration => configuration.Name == x))
                .ToImmutableArray();

            var bParameters = bParametersNames.Select(
                    x => Mock.Of<IMemberConfiguration>(configuration => configuration.Name == x))
                .ToImmutableArray();

            var ctorA = new CtorMemberConfiguration(
                aParameters, ImmutableArray<ITypeFullName>.Empty, Source);

            var ctorB = new CtorMemberConfiguration(
                bParameters, ImmutableArray<ITypeFullName>.Empty, Source);

            var id = TestHelper.RandomNamedFixtureItemId();
            var emptyCallbacks = ImmutableDictionary.Create<string, IImmutableList<object>>();

            var configItemA = new ItemBuilder<ConfigurationItem>()
                .With(p => p.Ctor.memberConfigurations.Value(
                    ImmutableDictionary.Create<string, IMemberConfiguration>()
                        .Add(ConfigurationConstants.CtorMemberName, ctorA)))
                .With(p => p.Ctor.id.Value(id))
                .With(p => p.Ctor.fixtureConfigurations.Value(
                    ImmutableDictionary<string, IFixtureConfiguration>.Empty))
                .With(p => p.Ctor.callbacks.Value(emptyCallbacks))
                .Build();


            var configItemB = new ItemBuilder<ConfigurationItem>()
                .With(p => p.Ctor.memberConfigurations.Value(
                    ImmutableDictionary<string, IMemberConfiguration>.Empty
                        .Add(ConfigurationConstants.CtorMemberName, ctorB)))
                .With(p => p.Ctor.id.Value(id))
                .With(p => p.Ctor.fixtureConfigurations.Value(
                    ImmutableDictionary<string, IFixtureConfiguration>.Empty))
                .With(p => p.Ctor.callbacks.Value(emptyCallbacks))
                .Build();

            // act
            var result = configItemA.Merge((IConfigurationItem)configItemB);

            configItemA.Id.TypeFullName.Equals(configItemB.Id.TypeFullName).Should().BeTrue();


            // assert
            var config = TestHelper.AssertAndUnwrapSuccess(result);
            var parameters = config.OnlyCtorParameterMemberConfigurations;
            parameters.Should().HaveCount(3);
            parameters[aParametersNames[0]].Should().Be(aParameters[0]);
            parameters[aParametersNames[1]].Should().Be(bParameters[0]);
            parameters[aParametersNames[2]].Should().Be(aParameters[2]);
        }
    }
}