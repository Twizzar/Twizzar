using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.Core.Query.Services;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Core.Tests.Query.Services;

[TestFixture]
public class ShortTypesConverterTests
{
    [Test]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<ShortTypesConverter>()
            .ShouldThrowArgumentNullException();
    }

    [TestCase("int")]
    [TestCase("uint")]
    [TestCase("long")]
    [TestCase("ulong")]
    [TestCase("short")]
    [TestCase("ushort")]
    [TestCase("byte")]
    [TestCase("sbyte")]
    [TestCase("decimal")]
    [TestCase("float")]
    [TestCase("double")]
    [TestCase("char")]
    [TestCase("string")]
    [TestCase("bool")]
    public void IsShortTypeReturnsTrueForShortType(string shortType)
    {
        // arrange
        var sut = new ItemBuilder<ShortTypesConverter>()
            .Build();

        // act
        var result = sut.IsShortType(shortType);

        // assert
        result.Should().BeTrue();
    }

    [TestCase("int", "Int32")]
    [TestCase("uint", "UInt32")]
    [TestCase("long", "Int64")]
    [TestCase("ulong", "UInt64")]
    [TestCase("short", "Int16")]
    [TestCase("ushort", "UInt16")]
    [TestCase("byte", "Byte")]
    [TestCase("sbyte", "SByte")]
    [TestCase("decimal", "Decimal")]
    [TestCase("float", "Single")]
    [TestCase("double", "Double")]
    [TestCase("char", "Char")]
    [TestCase("string", "String")]
    [TestCase("bool", "Boolean")]
    public void ShortTypeToTypeFullNameWorksCorrectly(string shortType, string fullname)
    {
        // arrange
        var sut = new ItemBuilder<ShortTypesConverter>()
            .Build();

        // act
        var result = sut.ConvertToTypeFullName(shortType);

        // assert
        result.Should().Be($"System.{fullname}");
    }

    [TestCase("int", "Int32")]
    [TestCase("uint", "UInt32")]
    [TestCase("long", "Int64")]
    [TestCase("ulong", "UInt64")]
    [TestCase("short", "Int16")]
    [TestCase("ushort", "UInt16")]
    [TestCase("byte", "Byte")]
    [TestCase("sbyte", "SByte")]
    [TestCase("decimal", "Decimal")]
    [TestCase("float", "Single")]
    [TestCase("double", "Double")]
    [TestCase("char", "Char")]
    [TestCase("string", "String")]
    [TestCase("bool", "Boolean")]
    public void ConvertToShortWorksCorrectly(string shortType, string fullname)
    {
        // arrange
        var sut = new ItemBuilder<ShortTypesConverter>()
            .Build();

        // act
        var result = sut.ConvertToShort(TypeFullName.Create($"System.{fullname}"), string.Empty);

        // assert
        result.Should().Be(shortType);
    }
}