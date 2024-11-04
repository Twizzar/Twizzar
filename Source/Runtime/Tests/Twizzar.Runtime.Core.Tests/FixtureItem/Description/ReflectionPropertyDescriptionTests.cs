using System;
using System.Reflection;

using FluentAssertions;

using NUnit.Framework;
using Twizzar.Runtime.Core.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using TwizzarInternal.Fixture;

// ReSharper disable UnusedMember.Local
#pragma warning disable S2376 // Write-only properties should not be used
#pragma warning disable S1144 // Unused private types or members should be removed
#pragma warning disable IDE0051 // Remove unused private members

namespace Twizzar.Runtime.Core.Tests.FixtureItem.Description
{
    public partial class ReflectionPropertyDescriptionTests
    {

        [Test]
        public void All_Ctor_parameter_throws_ArgumentNullException_when_null()
        {
            // arrange
            var parameterInfo = GetPropertyInfo(nameof(TestClass.IntProp));

            // assert
            Verify.Ctor<ReflectionPropertyDescription>()
                .SetupParameter("info", parameterInfo)
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public void TypeFullName_is_set_correctly()
        {
            // arrange
            var sut = GetSut(nameof(TestClass.IntProp));

            // assert
            sut.TypeFullName.FullName.Should().Be(typeof(int).FullName);
        }

        [TestCaseSource(nameof(AccessModifier_is_set_correctlyCases))]
        public void AccessModifier_is_set_correctly(string propName, object expectedAccessModifier)
        {
            // arrange
            var sut = GetSut(propName);

            // assert
            sut.AccessModifier.Should().Be(expectedAccessModifier);
        }

        [TestCase(nameof(TestClass.InternalPop))]
        [TestCase(nameof(TestClass.IntProp))]
        public void Name_is_set_correctly(string propName)
        {
            // arrange
            var sut = GetSut(propName);

            // assert
            sut.Name.Should().Be(propName);
        }

        [TestCase(nameof(TestClass.InternalPop), true, true)]
        [TestCase("PrivateProp", true, false)]
        [TestCase("ProtectedProp", false, true)]
        public void CanRead_and_CanWrite_are_set_correctly(string propName, bool expectedCanRead, bool expectedCanWrite)
        {
            // arrange
            var sut = GetSut(propName);

            // assert
            sut.CanRead.Should().Be(expectedCanRead);
            sut.CanWrite.Should().Be(expectedCanWrite);
        }

        [TestCase(nameof(TestClass.IntProp), false)]
        [TestCase(nameof(TestClass.StaticProp), true)]
        public void IsStatic_and_CanWrite_are_set_correctly(string propName, bool expectedIsStatic)
        {
            // arrange
            var sut = GetSut(propName);

            // assert
            sut.IsStatic.Should().Be(expectedIsStatic);
        }

        [TestCaseSource(nameof(OverrideKind_and_CanWrite_are_set_correctlyCases))]
        public void OverrideKind_and_CanWrite_are_set_correctly(string propName, object expectedOverrideKind)
        {
            // arrange
            var sut = GetSut(propName);

            // assert
            sut.OverrideKind.Should().Be(expectedOverrideKind);
        }

        [TestCase(nameof(TestClass.IntProp), typeof(int))]
        [TestCase(nameof(TestClass.StringProp), typeof(string))]
        [TestCase(nameof(TestClass.TestClassProp), typeof(TestClass))]
        public void Type_and_CanWrite_are_set_correctly(string propName, Type expectedType)
        {
            // arrange
            var sut = GetSut(propName);

            // assert
            sut.Type.Should().Be(expectedType);
        }

        [TestCase(nameof(TestClass.IntProp), typeof(int))]
        [TestCase(nameof(TestClass.StringProp), typeof(string))]
        [TestCase(nameof(TestClass.TestClassProp), typeof(TestClass))]
        public void GetReturnTypeDescription_returns_correct_description(string propName, Type expectedType)
        {
            // arrange
            var expectedDescription = new ItemBuilder<ReflectionTypeDescription>()
                .With(p => p.Ctor.type.Value(expectedType))
                .Build();

            // act
            var sut = GetSut(propName);

            // assert
            sut.GetReturnTypeDescription().Should().Be(expectedDescription);
        }

        [TestCase]
        public void Type_Booleans_for_class_are_set_correctly()
        {
            // arrange
            var propertyInfo = GetPropertyInfo(nameof(TestClass.TestClassProp));

            // act
            var sut = new ReflectionPropertyDescription(propertyInfo, new ItemBuilder<IBaseTypeService>().Build());

            // assert
            sut.IsClass.Should().BeTrue();
            sut.IsInterface.Should().BeFalse();
            sut.IsStruct.Should().BeFalse();
            sut.IsEnum.Should().BeFalse();
        }

        [TestCase]
        public void Type_Booleans_for_interfaces_are_set_correctly()
        {
            // arrange
            var propertyInfo = GetPropertyInfo(nameof(TestClass.TestInterfaceProp));

            // act
            var sut = new ReflectionPropertyDescription(propertyInfo, new ItemBuilder<IBaseTypeService>().Build());

            // assert
            sut.IsClass.Should().BeFalse();
            sut.IsInterface.Should().BeTrue();
            sut.IsStruct.Should().BeFalse();
            sut.IsEnum.Should().BeFalse();
        }

        [TestCase]
        public void Type_Booleans_for_struct_are_set_correctly()
        {
            // arrange
            var propertyInfo = GetPropertyInfo(nameof(TestClass.TestStructProp));

            // act
            var sut = new ReflectionPropertyDescription(propertyInfo, new ItemBuilder<IBaseTypeService>().Build());

            // assert
            sut.IsClass.Should().BeFalse();
            sut.IsInterface.Should().BeFalse();
            sut.IsStruct.Should().BeTrue();
            sut.IsEnum.Should().BeFalse();
        }

        [TestCase]
        public void Type_Booleans_for_enum_are_set_correctly()
        {
            // arrange
            var propertyInfo = GetPropertyInfo(nameof(TestClass.TestEnumProp));

            // act
            var sut = new ReflectionPropertyDescription(propertyInfo, new ItemBuilder<IBaseTypeService>().Build());

            // assert
            sut.IsClass.Should().BeFalse();
            sut.IsInterface.Should().BeFalse();
            sut.IsStruct.Should().BeFalse();
            sut.IsEnum.Should().BeTrue();
        }

        private static readonly object[] AccessModifier_is_set_correctlyCases =
        {
            new object[] {"IntProp", AccessModifier.CreatePublic()},
            new object[] {"PrivateProp", AccessModifier.CreatePrivate()},
            new object[] {"ProtectedProp", AccessModifier.CreateProtected()},
            new object[] {"InternalPop", AccessModifier.CreateInternal()},
        };

        private static readonly object[] OverrideKind_and_CanWrite_are_set_correctlyCases =
        {
            new object[] { nameof(TestClass.IntProp), OverrideKind.Create()},
            new object[] { nameof(TestClass.VirtualProp), OverrideKind.CreateVirtual()},
            new object[] { nameof(TestClass.SealedProp), OverrideKind.Create(true, true)},
        };

        private static ReflectionPropertyDescription GetSut(string propName) =>
            new ItemBuilder<ReflectionPropertyDescription>()
                .With(p => p.Ctor.info.Value(GetPropertyInfo(propName)))
                .Build();

        private static PropertyInfo GetPropertyInfo(string propName) =>
            typeof(TestClass).GetProperty(propName,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

        public class BaseTestClass
        {
            public virtual int SealedProp { get; set; }
        }

        public class TestClass : BaseTestClass
        {
            public int IntProp { get; set; }

            public string StringProp { get; set; }

            public TestClass TestClassProp { get; set; }

            private int PrivateProp { get; }

            protected int ProtectedProp
            {
                set => throw new NotImplementedException();
            }

            internal int InternalPop { get; set; }

            public static int StaticProp { get; set; }

            public virtual int VirtualProp { get; set; }

            public sealed override int SealedProp { get; set; }

            public ITestInterface TestInterfaceProp { get; set; }
            public TestStruct TestStructProp { get; set; }
            public TestEnum TestEnumProp { get; set; }
        }

        public interface ITestInterface
        {
        }

        public struct TestStruct
        {
        }

        public enum TestEnum
        {

        }
    }
}