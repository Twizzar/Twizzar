using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;

using Moq;

using NUnit.Framework;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;

namespace Twizzar.SharedKernel.CoreInterfaces.Tests.FixtureItem.Configuration.MemberConfigurations;

[TestFixture]
public class MemberConfigurationEqualityComparerTests
{

    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<MemberConfigurationEqualityComparer>()
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void MemberConfiguration_with_same_content_but_different_type_are_not_equal()
    {
        // arrange
        var name = RandomString();
        var source = Build.New<IConfigurationSource>();
        var m1 = new NullValueMemberConfiguration(name, source);
        var m2 = new UniqueValueMemberConfiguration(name, source);

        // act
        var sut = new MemberConfigurationEqualityComparer(RandomBool());

        // assert
        sut.Equals(m1, m2).Should().BeFalse();
    }

    [Test]
    public void ValueMember_is_compared_Correctly()
    {
        var a = Build.New<ValueMemberConfiguration>();
        var allEquals = new ValueMemberConfiguration(a.Name, a.Value, a.Source);
        var allExceptSourceEquals = a with { Source = Build.New<IConfigurationSource>() };
        var notEquals = Build.New<ValueMemberConfiguration>();
        TestMemberConfig(a, allEquals, allExceptSourceEquals, notEquals);
    }

    [Test]
    public void CodeValueMemberConfiguration_is_compared_Correctly()
    {
        var a = Build.New<CodeValueMemberConfiguration>();
        var allEquals = new CodeValueMemberConfiguration(a.Name, a.SourceCode, a.Source);
        var allExceptSourceEquals = a with { Source = Build.New<IConfigurationSource>() };
        var notEquals = Build.New<CodeValueMemberConfiguration>();
        TestMemberConfig(a, allEquals, allExceptSourceEquals, notEquals);
    }

    [Test]
    public void CtorMemberConfiguration_is_compared_Correctly()
    {
        // arrange

        var memberConfigs = Enumerable.Empty<IMemberConfiguration>()
            .Append(Mock.Of<IMemberConfiguration>(configuration => configuration.Name == "TestMember"));

        var a = new CtorMemberConfiguration(
            memberConfigs,
            ImmutableArray<ITypeFullName>.Empty,
            Build.New<IConfigurationSource>());
        var allEquals = new CtorMemberConfiguration(a.ConstructorParameters, a.ConstructorSignature, a.Source);
        var allExceptSourceEquals = a with { Source = Build.New<IConfigurationSource>() };
        var notEquals = new CtorMemberConfiguration(
            memberConfigs,
            ImmutableArray<ITypeFullName>.Empty,
            Build.New<IConfigurationSource>());

        var sut = new MemberConfigurationEqualityComparer(false);
        var ctorParamsA = a with { ConstructorParameters = a.ConstructorParameters.Add(RandomString(), Build.New<IMemberConfiguration>()) };
        var ctorParamsB = a with { ConstructorParameters = a.ConstructorParameters.Add(RandomString(), Build.New<IMemberConfiguration>()) };

        var ctorSignatureA = a with { ConstructorSignature = ImmutableArray<ITypeFullName>.Empty.Add(Build.New<ITypeFullName>()) };
        var ctorSignatureB = a with { ConstructorSignature = ImmutableArray<ITypeFullName>.Empty.Add(Build.New<ITypeFullName>()) };

        // act & assert
        TestMemberConfig(a, allEquals, allExceptSourceEquals, notEquals);
        sut.Equals(ctorParamsA, ctorParamsB).Should().BeFalse();
        sut.Equals(ctorSignatureA, ctorSignatureB).Should().BeFalse();
    }

    [Test]
    public void LinkMemberConfiguration_is_compared_Correctly()
    {
        var a = Build.New<LinkMemberConfiguration>();
        var allEquals = new LinkMemberConfiguration(a.Name, a.ConfigurationLink, a.Source);
        var allExceptSourceEquals = a with { Source = Build.New<IConfigurationSource>() };
        var notEquals = Build.New<LinkMemberConfiguration>();
        TestMemberConfig(a, allEquals, allExceptSourceEquals, notEquals);
    }

    [Test]
    public void MethodConfiguration_is_compared_Correctly()
    {
        var a = Build.New<MethodConfiguration>();
        var allEquals = a with { };
        var allExceptSourceEquals = a with { Source = Build.New<IConfigurationSource>() };
        var notEquals = Build.New<MethodConfiguration>();
        TestMemberConfig(a, allEquals, allExceptSourceEquals, notEquals);
    }

    [Test]
    public void NullValueMemberConfiguration_is_compared_Correctly()
    {
        var a = Build.New<NullValueMemberConfiguration>();
        var allEquals = new NullValueMemberConfiguration(a.Name, a.Source);
        var allExceptSourceEquals = a with { Source = Build.New<IConfigurationSource>() };
        var notEquals = Build.New<NullValueMemberConfiguration>();
        TestMemberConfig(a, allEquals, allExceptSourceEquals, notEquals);
    }

    [Test]
    public void UniqueValueMemberConfiguration_is_compared_Correctly()
    {
        var a = Build.New<UniqueValueMemberConfiguration>();
        var allEquals = new UniqueValueMemberConfiguration(a.Name, a.Source);
        var allExceptSourceEquals = a with { Source = Build.New<IConfigurationSource>() };
        var notEquals = Build.New<UniqueValueMemberConfiguration>();
        TestMemberConfig(a, allEquals, allExceptSourceEquals, notEquals);
    }

    private static void TestMemberConfig(
        IMemberConfiguration a, 
        IMemberConfiguration allEquals,
        IMemberConfiguration allExceptSourceEquals,
        IMemberConfiguration notEquals)
    {
        var sut = new MemberConfigurationEqualityComparer(false);
        var sutIgnoreSource = new MemberConfigurationEqualityComparer(true);

        sut.Equals(a, allEquals).Should().BeTrue();
        sutIgnoreSource.Equals(a, allEquals).Should().BeTrue();
        sutIgnoreSource.Equals(a, allExceptSourceEquals).Should().BeTrue();

        sut.Equals(a, notEquals).Should().BeFalse();
        sut.Equals(a, allExceptSourceEquals).Should().BeFalse();
        sut.Equals(a, notEquals).Should().BeFalse();
        sutIgnoreSource.Equals(a, notEquals).Should().BeFalse();
    }
}