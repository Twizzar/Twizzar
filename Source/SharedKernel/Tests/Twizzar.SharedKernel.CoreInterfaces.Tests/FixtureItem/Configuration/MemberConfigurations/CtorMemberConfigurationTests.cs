using System;
using System.Collections.Immutable;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using TwizzarInternal.Fixture;

namespace Twizzar.SharedKernel.CoreInterfaces.Tests.FixtureItem.Configuration.MemberConfigurations;

[Category("TwizzarInternal")]
public class CtorMemberConfigurationTests
{
    private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();

    [Test]
    public void WithParameter_change_on_valid_position_returns_a_correct_new_config()
    {
        var name = "MyCtorParameter";
        var paramConfigs = ImmutableArray.Create<IMemberConfiguration>()
            .Add(new ValueMemberConfiguration(name, RandomString(), Source))
            .Add(new ItemBuilder<ValueMemberConfiguration>().Build());

        var sut = new CtorMemberConfiguration(paramConfigs, ImmutableArray<ITypeFullName>.Empty, Source);

        // act
        var newConfig = sut.WithParameter(name, new UniqueValueMemberConfiguration(name, Source));

        // assert
        newConfig.ConstructorParameters.Should().HaveCount(2);
        newConfig.ConstructorParameters[paramConfigs[1].Name].Should().Be(sut.ConstructorParameters[paramConfigs[1].Name]);
        newConfig.ConstructorParameters[paramConfigs[0].Name].Should().BeOfType<UniqueValueMemberConfiguration>();
        newConfig.ConstructorParameters[paramConfigs[0].Name].Name.Should().Be(name);
        newConfig.Should().NotBeEquivalentTo(sut);
    }

    [Test]
    public void WithParameter_when_new_parameterConfig_is_null_throws_ArgumentNullException()
    {
        // arrange
        var sut = new CtorMemberConfiguration(
            new ItemBuilder<IImmutableDictionary<string, IMemberConfiguration>>().Build(),
            new ItemBuilder<ImmutableArray<ITypeFullName>>().Build(),
            Source);

        // act
        Action action = () => sut.WithParameter(string.Empty, null);

        // assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void WithParameter_when_parameterName_does_not_exists_throws_ArgumentException()
    {
        // arrange
        var name = RandomString();

        var ctorParameters = ImmutableArray.Create<IMemberConfiguration>()
            .AddRange(new ItemBuilder<UniqueValueMemberConfiguration>().BuildMany(2));

        var sut = new CtorMemberConfiguration(ctorParameters, ImmutableArray<ITypeFullName>.Empty, Source);

        // act
        Action action = () => sut.WithParameter(name, new ValueMemberConfiguration(RandomString(), RandomString(), Source));

        // assert
        var exp = action.Should().Throw<ArgumentException>().Subject.First();
        exp.ParamName.Should().Be("name");
    }
}