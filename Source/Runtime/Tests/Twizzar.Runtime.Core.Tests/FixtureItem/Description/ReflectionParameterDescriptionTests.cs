using System;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Runtime.Core.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;

#pragma warning disable IDE0060 // Remove unused parameter

namespace Twizzar.Runtime.Core.Tests.FixtureItem.Description
{
    public partial class ReflectionParameterDescriptionTests
    {
        #region static fields and constants

        private static readonly object[] DefaultValue_is_set_correctlyCases =
        {
            new object[] { 0, Maybe.None<ParameterDefaultValue>() },
            new object[] { 1, Maybe.Some(new ParameterDefaultValue("testDefault")) },
            new object[] { 2, Maybe.Some(new ParameterDefaultValue(null)) },
        };

        #endregion

        #region TestEnum enum

        public enum TestEnum
        {
        }

        #endregion

        #region members

        [Test]
        public void All_Ctor_parameter_throws_ArgumentNullException_when_null()
        {
            // arrange
            var parameterInfo = GetParameterInfo();

            // assert
            Verify.Ctor<ReflectionParameterDescription>()
                .SetupParameter("info", parameterInfo)
                .ShouldThrowArgumentNullException();
        }

        [TestCase(0, typeof(int))]
        [TestCase(1, typeof(string))]
        [TestCase(2, typeof(TestClass))]
        public void TypeFullName_is_set_correctly(int parameterPosition, Type expectedParameterType)
        {
            // arrange
            // act
            var sut = CreateSut(parameterPosition: parameterPosition);

            // assert
            sut.TypeFullName.FullName.Should().Be(expectedParameterType.FullName);
        }

        [TestCase(0, "param1")]
        [TestCase(1, "param2")]
        [TestCase(2, "testClass")]
        public void Name_is_set_correctly(int parameterPosition, string expectedName)
        {
            // arrange
            var sut = CreateSut(parameterPosition: parameterPosition);

            // assert
            sut.Name.Should().Be(expectedName);
        }

        [TestCaseSource(nameof(DefaultValue_is_set_correctlyCases))]
        public void DefaultValue_is_set_correctly(
            int parameterPosition,
            object expectedDefaultValue)
        {
            // arrange
            var sut = CreateSut(parameterPosition: parameterPosition);

            // assert
            sut.DefaultValue.Should().Be(expectedDefaultValue);
        }

        [TestCase(0, true, false)]
        [TestCase(1, false, true)]
        public void IsIn_and_IsOut_is_set_correctly(int parameterPosition, bool expectedIsIn, bool expectedIsOut)
        {
            // arrange
            var sut = CreateSut(((InOutDelegate)TestClass.MethodWithInAndOut).Method, parameterPosition);

            // assert
            sut.IsIn.Should().Be(expectedIsIn);
            sut.IsOut.Should().Be(expectedIsOut);
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        public void IsOptional_is_set_correctly(int parameterPosition, bool expectedIsOptional)
        {
            // arrange
            // act
            var sut = CreateSut(parameterPosition: parameterPosition);

            // assert
            sut.IsOptional.Should().Be(expectedIsOptional);
        }

        [TestCase(0, typeof(int))]
        [TestCase(1, typeof(string))]
        [TestCase(2, typeof(TestClass))]
        public void GetReturnTypeDescription_returns_correct_description(int parameterPosition, Type expectedType)
        {
            // arrange
            var expected = CreateSut(parameterPosition: parameterPosition);

            // act
            var sut = CreateSut(parameterPosition: parameterPosition);

            // assert
            sut.Type.Should().Be(expectedType);
        }

        [Test]
        public void Type_Booleans_for_class_are_set_correctly()
        {
            // arrange
            var info = ((Action<TestClass>)TestClass.MethodWithClass).Method.GetParameters()[0];

            // act
            var sut = new ReflectionParameterDescription(info, new ItemBuilder<IBaseTypeService>().Build());

            // assert
            sut.IsClass.Should().BeTrue();
            sut.IsInterface.Should().BeFalse();
            sut.IsStruct.Should().BeFalse();
            sut.IsEnum.Should().BeFalse();
        }

        [Test]
        public void Type_Booleans_for_interface_are_set_correctly()
        {
            // arrange
            var info = ((Action<ITestInterface>)TestClass.MethodWithInterface).Method.GetParameters()[0];

            // act
            var sut = new ReflectionParameterDescription(info, new ItemBuilder<IBaseTypeService>().Build());

            // assert
            sut.IsClass.Should().BeFalse();
            sut.IsInterface.Should().BeTrue();
            sut.IsStruct.Should().BeFalse();
            sut.IsEnum.Should().BeFalse();
        }

        [Test]
        public void Type_Booleans_for_struct_are_set_correctly()
        {
            // arrange
            var info = ((Action<TestStruct>)TestClass.MethodWithStruct).Method.GetParameters()[0];

            // act
            var sut = new ReflectionParameterDescription(info, new ItemBuilder<IBaseTypeService>().Build());

            // assert
            sut.IsClass.Should().BeFalse();
            sut.IsInterface.Should().BeFalse();
            sut.IsStruct.Should().BeTrue();
            sut.IsEnum.Should().BeFalse();
        }

        [Test]
        public void Type_Booleans_for_enum_are_set_correctly()
        {
            // arrange
            var info = ((Action<TestEnum>)TestClass.MethodWithEnum).Method.GetParameters()[0];

            // act
            var sut = new ReflectionParameterDescription(info, new ItemBuilder<IBaseTypeService>().Build());

            // assert
            sut.IsClass.Should().BeFalse();
            sut.IsInterface.Should().BeFalse();
            sut.IsStruct.Should().BeFalse();
            sut.IsEnum.Should().BeTrue();
        }

        private static ReflectionParameterDescription CreateSut(
            Maybe<MethodBase> maybeMethodBase = default,
            int parameterPosition = 0) =>
            new ItemBuilder<ReflectionParameterDescription>()
                .With(p => p.Ctor.info.Value(
                    GetParameterInfo(maybeMethodBase, parameterPosition: parameterPosition)))
                .Build();

        private static ParameterInfo GetParameterInfo(
            Maybe<MethodBase> maybeMethodBase = default,
            int parameterPosition = 0)
        {
            var methodBase = maybeMethodBase.SomeOrProvided(((Action<int, string, TestClass>)TestClass.Method).Method);

            return methodBase.GetParameters()[parameterPosition];
        }

        #endregion

        #region Nested type: InOutDelegate

        private delegate void InOutDelegate(in int param1, out int param2);

        #endregion

        #region Nested type: ITestInterface

        public interface ITestInterface
        {
        }

        #endregion

        #region Nested type: TestClass

        public class TestClass
        {
            #region members

            public static void Method(int param1, string param2 = "testDefault", TestClass testClass = null) =>
                throw new NotImplementedException();

            public static void MethodWithInAndOut(in int param1, out int param2) => throw new NotImplementedException();

            public static void MethodWithClass(TestClass p1) => throw new NotImplementedException();

            public static void MethodWithInterface(ITestInterface p1) => throw new NotImplementedException();

            public static void MethodWithEnum(TestEnum p1) => throw new NotImplementedException();

            public static void MethodWithStruct(TestStruct p1) => throw new NotImplementedException();

            #endregion
        }

        #endregion

        #region Nested type: TestStruct

        public struct TestStruct
        {
        }

        #endregion
    }
}