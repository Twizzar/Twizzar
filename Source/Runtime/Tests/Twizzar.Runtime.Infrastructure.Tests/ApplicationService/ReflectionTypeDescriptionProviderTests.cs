using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Runtime.Infrastructure.ApplicationService;
using Twizzar.Runtime.Infrastructure.Tests.Builder;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using TwizzarInternal.Fixture;

namespace Twizzar.Runtime.Infrastructure.Tests.ApplicationService
{
    [Category("TwizzarInternal")]
    public partial class ReflectionTypeDescriptionProviderTests
    {
        [Test]
        public void All_ctor_parameter_throw_ArgumentNullException_when_null()
        {
            // assert
            Verify.Ctor<ReflectionTypeDescriptionProvider>()
                .ShouldThrowArgumentNullException();
        }

        [Test]
        [TestCase(typeof(int))]
        [TestCase(typeof((int, string)))]
        [TestCase(typeof(List<int>))]
        [TestCase(typeof(IEnumerable<int>))]
        [TestCase(typeof(int?))]
        public void TypeFullName_of_description_is_equal_to_input_type_fullname(Type type)
        {
            // arrange
            var sut = CreateSut();

            // act
            var description = sut.GetTypeDescription(type);

            // assert
            description.TypeFullName.FullName.Should().Be(type.FullName);
        }

        [Test]
        public void Interface_contains_members_of_implementing_interfaces()
        {
            // arrange
            var sut = CreateSut();

            // act
            var description = sut.GetTypeDescription(typeof(ITypeDescription));

            description.GetDeclaredProperties().Select(propertyDescription => propertyDescription.Name)
                .Should()
                .Contain("TypeFullName");
        }

        [Test]
        public void ReflectionDescriptionProvider_GetTypeDescription_Success()
        {
            // arrange
            var sut = CreateSut();
            var someValidType = typeof(ReflectionTypeDescriptionProvider);

            // act
            var result = sut.GetTypeDescription(someValidType);

            // assert
            result.Should().NotBeNull();
            result.TypeFullName.FullName.Should().Be(someValidType.FullName);

            result.IsClass.Should().BeTrue();
            result.AccessModifier.IsPublic.Should().BeTrue();

            result.IsInterface.Should().BeFalse();
            result.IsAbstract.Should().BeFalse();
        }

        [Test]
        public void Description_of_enum_has_all_enumValues_as_const_fields()
        {
            // arrange
            var sut = CreateSut();

            // act
            var description = sut.GetTypeDescription(typeof(OneTwoThree));

            // assert
            description.Should().NotBeNull();

            description.GetDeclaredFields()
                .Where(fieldDescription => fieldDescription.DeclaringType.Equals(description.TypeFullName))
                .Should().HaveCount(4);

            var constFields = description.GetDeclaredFields()
                .Where(fieldDescription => fieldDescription.DeclaringType.Equals(description.TypeFullName))
                .Where(fieldDescription => fieldDescription.IsConstant)
                .ToList();

            constFields.Should().HaveCount(3);

            constFields.Should().Contain(
                fieldDescription =>
                    fieldDescription.Name == "One" &&
                    fieldDescription.ConstantValue.IsSome &&
                    (int)fieldDescription.ConstantValue.GetValueUnsafe() == 0);

            constFields.Should().Contain(
                fieldDescription =>
                    fieldDescription.Name == "Two" &&
                    fieldDescription.ConstantValue.IsSome &&
                    (int)fieldDescription.ConstantValue.GetValueUnsafe() == 1);

            constFields.Should().Contain(
                fieldDescription =>
                    fieldDescription.Name == "Three" &&
                    fieldDescription.ConstantValue.IsSome &&
                    (int)fieldDescription.ConstantValue.GetValueUnsafe() == 2);
        }

        private static ReflectionTypeDescriptionProvider CreateSut() =>
            new ItemBuilder<ReflectionTypeDescriptionProvider>()
                .With(p => p.Ctor.descriptionFactory.Value(new ReflectionDescriptionFactoryBuilder().Build()))
                .Build();
    }
}