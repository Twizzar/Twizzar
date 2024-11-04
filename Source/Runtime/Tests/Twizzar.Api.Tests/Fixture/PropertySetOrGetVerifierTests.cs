using System;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Fixture;
using Twizzar.Fixture.MethodVerifier;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Api.Tests.Fixture;

[TestFixture]
public class PropertySetOrGetVerifierTests
{
    [Test]
    public void Get_property_is_verified_correctly_on_called()
    {
        // arrange
        var mock = new Mock<IEngine>();
        var propInfo = typeof(IEngine).GetProperty("Cylinder");

        var sut = new PropertySetOrGetVerifier<ICylinder>(
            mock,
            typeof(IEngine),
            propInfo,
            false,
            Maybe.None());

        // act
        var _ = mock.Object.Cylinder;

        sut.CalledAtLeastOnce();

        // assert
        Assert.Pass();
    }

    [Test]
    public void Get_property_is_verified_correctly_not_called()
    {
        // arrange
        var mock = new Mock<IEngine>();
        var propInfo = typeof(IEngine).GetProperty("Cylinder");

        var sut = new PropertySetOrGetVerifier<ICylinder>(
            mock,
            typeof(IEngine),
            propInfo,
            false,
            Maybe.None());

        // act
        var a = () => sut.CalledAtLeastOnce();

        // assert
        var exp = a.Should().Throw<VerificationException>().Subject.First();
        Console.WriteLine(exp);
    }

    [TestCase(1)]
    [TestCase(2)]
    public void Get_property_is_verified_correctly_not_called_n_times(int times)
    {
        // arrange
        var mock = new Mock<IEngine>();
        var propInfo = typeof(IEngine).GetProperty("Cylinder");

        var sut = new PropertySetOrGetVerifier<ICylinder>(
            mock,
            typeof(IEngine),
            propInfo,
            false,
            Maybe.None());

        // act
        var a = () => sut.Called(times);

        // assert
        a.Should().Throw<VerificationException>();
    }

    [TestCase(1)]
    [TestCase(2)]
    public void Set_property_is_verified_correctly_on_called_n_times(int times)
    {
        // arrange
        var mock = new Mock<IEngine>();

        var propInfo = typeof(IEngine).GetProperty("EngineName");

        var sut = new PropertySetOrGetVerifier<string>(
            mock,
            typeof(IEngine),
            propInfo,
            true,
            Maybe.None());

        // act
        for (int t = 0; t < times; t++)
        {
            mock.Object.EngineName = "test";
        }
        sut.Called(times);

        // assert
        Assert.Pass();
    }

    [Test]
    public void Set_property_is_verified_correctly_on_not_called()
    {
        // arrange
        var mock = new Mock<IEngine>();

        var propInfo = typeof(IEngine).GetProperty("EngineName");

        var sut = new PropertySetOrGetVerifier<string>(
            mock,
            typeof(IEngine),
            propInfo,
            true,
            Maybe.None());

        // act
        var a = () => sut.CalledAtLeastOnce();

        // assert
        a.Should().Throw<VerificationException>();
    }

    [Test]
    public void Set_property_is_verified_correctly_when_value_specified_on_called()
    {
        // arrange
        var mock = new Mock<IEngine>();

        var propInfo = typeof(IEngine).GetProperty("EngineName");

        var sut = new PropertySetOrGetVerifier<string>(
            mock,
            typeof(IEngine),
            propInfo,
            true,
            "test");

        // act
        mock.Object.EngineName = "test";
        sut.CalledAtLeastOnce();

        // assert
        Assert.Pass();
    }

    [Test]
    public void Set_property_is_verified_correctly_when_value_specified_on_not_called()
    {
        // arrange
        var mock = new Mock<IEngine>();

        var propInfo = typeof(IEngine).GetProperty("EngineName");

        var sut = new PropertySetOrGetVerifier<string>(
            mock,
            typeof(IEngine),
            propInfo,
            true,
            "test");

        // act
        var a = () => sut.CalledAtLeastOnce();

        // assert
        a.Should().Throw<VerificationException>();
    }
}