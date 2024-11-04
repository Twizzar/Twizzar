using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Design.Ui.Helpers;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.FixtureInformations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon;
using Twizzar.TestCommon.Builder;

using TwizzarInternal.Fixture;


namespace Twizzar.Design.Ui.Tests.ViewModels.FixtureItem.Nodes.FixtureInformations;

[TestFixture]
public partial class FixtureItemInformationTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<FixtureItemInformation>()
            .SetupParameter("fixtureDescription", Mock.Of<IMemberDescription>(description => description.Name == ""))
            .ShouldThrowArgumentNullException();
    }

    [TestCase(
        typeof(UndefinedMemberConfiguration),
        KeyWords.Undefined)]
    [TestCase(
        typeof(UniqueValueMemberConfiguration),
        KeyWords.Unique)]
    [TestCase(
        typeof(NullValueMemberConfiguration),
        KeyWords.Null)]
    public void DisplayValue_for_simple_keywords_is_set_correctly(Type memberType, string expectedDisplayValue)
    {
        // arrange
        var method = typeof(Build)
            .GetMethod(nameof(Build.New), new Type[0])
            .MakeGenericMethod(memberType);

        var memberConfig = (IMemberConfiguration)method.Invoke(null, Array.Empty<object>());

        var sut = new EmptyFixtureItemInformationBuild()
            .With(p => p.Ctor.memberConfiguration.Value(memberConfig))
            .Build();

        // act
        var displayValue = sut.DisplayValue;

        // assert
        displayValue.Should().Be(expectedDisplayValue);
    }

    [Test]
    public void ValueMemberConfiguration_returns_correct_DisplayValue()
    {
        // arrange
        var valueConfig = Build.New<ValueMemberConfiguration>();

        var sut = new EmptyFixtureItemInformationBuild()
            .With(p => p.Ctor.memberConfiguration.Value(valueConfig))
            .Build();

        // act
        var displayValue = sut.DisplayValue;

        // assert
        displayValue.Should().Be(valueConfig.Value.ToString());
    }

    [TestCase(true)]
    [TestCase(false)]
    public void IsDefaultIsSetCorrectlyForMethodOverloads(bool expectedIsDefault)
    {
        // assert
        var source = expectedIsDefault 
            ? new FromSystemDefault()
            : Mock.Of<IConfigurationSource>();

        var methodConfiguration = MethodConfiguration.Create(
            "TestMethod__Int32",
            "TestMethod",
            source,
            Build.New<ValueMemberConfiguration>(),
            typeof(int),
            nameof(Int32));

        var description = Mock.Of<ITypeDescription>(typeDescription =>
            typeDescription.TypeFullName == new TypeFullNameBuilder(typeof(int)).Build());

        var sut = new EmptyFixtureItemInformationBuild()
            .With(p => p.Ctor.memberConfiguration.Value(methodConfiguration))
            .With(p => p.Ctor.fixtureDescription.Value(description))
            .Build();

        // act
        var isDefault = sut.IsDefault;

        // assert
        isDefault.Should().Be(expectedIsDefault);
    }
}