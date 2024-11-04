using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.SharedKernel.Core.FixtureItem.Description.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using Twizzar.TestCommon;
using Twizzar.TestCommon.Builder;
using TwizzarInternal.Fixture;

namespace Twizzar.SharedKernel.Core.Tests.FixtureItem.Description.Services
{
    [TestFixture]
    public partial class BaseTypeServiceTests
    {
        [Test]
        public void Ctor_parameters_throw_ArgumentNullException_when_null()
        {
            // assert
            Verify.Ctor<BaseTypeService>()
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public void When_ReturnTypeDescription_is_enum_BaseTypeKind_is_enum()
        {
            // arrange
            var sut = new ItemBuilder<BaseTypeService>().Build();
            var baseDescription = new EnumIBaseDescriptionBuilder().Build();

            // act
            var baseTypeKind = sut.GetKind(baseDescription);

            // assert
            baseTypeKind.Should().Be(BaseTypeKind.Enum);
        }

        [TestCase(typeof(int), BaseTypeKind.Number)]
        [TestCase(typeof(int?), BaseTypeKind.Number)]
        [TestCase(typeof(uint), BaseTypeKind.Number)]
        [TestCase(typeof(uint?), BaseTypeKind.Number)]
        [TestCase(typeof(long), BaseTypeKind.Number)]
        [TestCase(typeof(long?), BaseTypeKind.Number)]
        [TestCase(typeof(ulong), BaseTypeKind.Number)]
        [TestCase(typeof(ulong?), BaseTypeKind.Number)]
        [TestCase(typeof(short), BaseTypeKind.Number)]
        [TestCase(typeof(short?), BaseTypeKind.Number)]
        [TestCase(typeof(ushort), BaseTypeKind.Number)]
        [TestCase(typeof(ushort?), BaseTypeKind.Number)]
        [TestCase(typeof(byte), BaseTypeKind.Number)]
        [TestCase(typeof(byte?), BaseTypeKind.Number)]
        [TestCase(typeof(sbyte), BaseTypeKind.Number)]
        [TestCase(typeof(sbyte?), BaseTypeKind.Number)]
        [TestCase(typeof(decimal), BaseTypeKind.Number)]
        [TestCase(typeof(decimal?), BaseTypeKind.Number)]
        [TestCase(typeof(float), BaseTypeKind.Number)]
        [TestCase(typeof(float?), BaseTypeKind.Number)]
        [TestCase(typeof(double), BaseTypeKind.Number)]
        [TestCase(typeof(double?), BaseTypeKind.Number)]
        [TestCase(typeof(bool), BaseTypeKind.Boolean)]
        [TestCase(typeof(bool?), BaseTypeKind.Boolean)]
        [TestCase(typeof(string), BaseTypeKind.String)]
        [TestCase(typeof(char), BaseTypeKind.Char)]
        [TestCase(typeof(char?), BaseTypeKind.Char)]
        public void Kind_for_basetype_is_correctly_set(Type type, BaseTypeKind expected)
        {
            // arrange
            var sut = new ItemBuilder<BaseTypeService>().Build();
            var notEnumDescription = new ItemBuilder<ITypeDescription>().Build();
            var typeFullName = new TypeFullNameBuilder(type).Build();

            var baseDescription = Mock.Of<IBaseDescription>(
                description =>
                    description.GetReturnTypeDescription() == notEnumDescription &&
                    description.TypeFullName == typeFullName);

            // act
            var baseTypeKind = sut.GetKind(baseDescription);

            // assert
            baseTypeKind.Should().Be(expected);
        }

        [Test]
        public void All_not_basetype_Fullnames_are_of_kind_complex()
        {
            // arrange
            var sut = new ItemBuilder<BaseTypeService>().Build();
            var notEnumDescription = new ItemBuilder<ITypeDescription>().Build();
            var typeFullName = TestHelper.RandomTypeFullName();

            var baseDescription = Mock.Of<IBaseDescription>(
                description =>
                    description.GetReturnTypeDescription() == notEnumDescription &&
                    description.TypeFullName == typeFullName);

            // act
            var baseTypeKind = sut.GetKind(baseDescription);

            // assert
            baseTypeKind.Should().Be(BaseTypeKind.Complex);
        }
    }
}