using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Runtime.Core.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;

// ReSharper disable PublicConstructorInAbstractClass
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedTypeParameter
#pragma warning disable S2326 // Unused type parameters should be removed
#pragma warning disable S3442 // "abstract" classes should not have "public" constructors
#pragma warning disable S1144 // Unused private types or members should be removed
#pragma warning disable IDE0060 

namespace Twizzar.Runtime.Core.Tests.FixtureItem.Description
{
    public class ReflectionMethodDescriptionTests
    {
        #region static fields and constants

        private static readonly object[] AccessModifier_is_set_correctlyCases =
        {
            new object[] {(nameof(TestClass.PublicVoidMethod)), AccessModifier.CreatePublic()},
            new object[] {"ProtectedMethod", AccessModifier.CreateProtected()},
            new object[] {"PrivateMethod", AccessModifier.CreatePrivate()},
            new object[] {"InternalMethod", AccessModifier.CreateInternal()},
        };

        private static readonly object[] OverrideKind_is_set_correctlyCases =
        {
            new object[] {GetInfo(nameof(TestClass.PublicVoidMethod)), OverrideKind.Create()},
            new object[] {GetInfo(nameof(TestClass.AbstractMethod)), OverrideKind.CreateVirtual()},
            new object[]
                {GetInfo(nameof(TestClass2.AbstractMethod), typeof(TestClass2)), OverrideKind.Create(true, true)},
        };

        #endregion


        #region members

        [TestCase(nameof(TestClass.PublicVoidMethod))]
        [TestCase(nameof(TestClass.AbstractMethod))]
        public void Name_is_set_correctly(string methodName)
        {
            // arrange
            var info = GetInfo(methodName);

            // act
            var sut = this.GetSut(info);

            // assert
            sut.Name.Should().Be(methodName);
        }

        [TestCase(nameof(TestClass.PublicVoidMethod))]
        [TestCase(nameof(TestClass.AbstractMethod))]
        public void TypeFullName_is_set_correctly(string methodName)
        {
            // arrange
            var info = GetInfo(methodName);

            // act
            var sut = this.GetSut(info);

            // assert
            sut.TypeFullName.FullName.Should().Be(info.ReturnType.FullName);
        }

        [Test]
        public void For_ctors_IsConstructor_is_true()
        {
            // arrange
            var infos = typeof(TestClass).GetConstructors();
            var returnType = typeof(TestClass);

            // act
            var descriptions = infos.Select(info => this.GetSut(info, returnType));

            // assert
            descriptions
                .Select(description => description.IsConstructor)
                .Should()
                .AllBeEquivalentTo(true);
        }

        [TestCase(nameof(TestClass.PublicVoidMethod))]
        public void For_methods_IsConstructor_is_false(string methodName)
        {
            // arrange
            var info = GetInfo(methodName);

            // act
            var sut = this.GetSut(info);

            // assert
            sut.IsConstructor.Should().BeFalse();
        }

        [TestCase(nameof(TestClass.PublicVoidMethod), false)]
        [TestCase(nameof(TestClass.AbstractMethod), true)]
        public void IsAbstract_is_set_correctly(string methodName, bool isAbstract)
        {
            // arrange
            var info = GetInfo(methodName);

            // act
            var sut = this.GetSut(info);

            // assert
            sut.IsAbstract.Should().Be(isAbstract);
        }

        [TestCase(nameof(TestClass.PublicVoidMethod), false)]
        [TestCase(nameof(TestClass.AbstractMethod), false)]
        [TestCase(nameof(TestClass.StaticMethod), true)]
        public void IsStatic_is_set_correctly(string methodName, bool isStatic)
        {
            // arrange
            var info = GetInfo(methodName);

            // act
            var sut = this.GetSut(info);

            // assert
            sut.IsStatic.Should().Be(isStatic);
        }

        [TestCase(nameof(TestClass.PublicVoidMethod), false)]
        [TestCase(nameof(TestClass.AbstractMethod), false)]
        [TestCase(nameof(TestClass.GenericMethod), true)]
        public void IsGeneric_is_set_correctly(string methodName, bool isGeneric)
        {
            // arrange
            var info = GetInfo(methodName);

            // act
            var sut = this.GetSut(info);

            // assert
            sut.IsGeneric.Should().Be(isGeneric);
        }

        [Test]
        public void GenericTypeArguments_is_set_correctly()
        {
            // arrange
            var genericInfo = GetInfo(nameof(TestClass.GenericMethod));
            var info = genericInfo.MakeGenericMethod(typeof(int), typeof(string));

            // act
            var sut = this.GetSut(info);
            var sutGeneric = this.GetSut(genericInfo);

            // assert
            sut.GenericTypeArguments.Should().HaveCount(2);
            sutGeneric.GenericTypeArguments.Should().HaveCount(2);
            var arg1 = sut.GenericTypeArguments.Values.First();
            var arg2 = sut.GenericTypeArguments.Values.Last();

            arg1.TypeFullName.IsSome.Should().BeTrue();
            arg2.TypeFullName.IsSome.Should().BeTrue();
            arg1.Name.Should().Be("T1");
            arg2.Name.Should().Be("T2");
            arg1.TypeFullName.GetValueUnsafe().FullName.Should().Be(typeof(int).FullName);
            arg2.TypeFullName.GetValueUnsafe().FullName.Should().Be(typeof(string).FullName);
            arg1.IsClosedConstructed.Should().BeTrue();
            arg2.IsClosedConstructed.Should().BeTrue();

            var genericParameterType1 = sutGeneric.GenericTypeArguments.Values.First();
            var genericParameterType2 = sutGeneric.GenericTypeArguments.Values.Last();

            genericParameterType1.TypeFullName.IsSome.Should().BeFalse();
            genericParameterType2.TypeFullName.IsSome.Should().BeFalse();
            genericParameterType1.Name.Should().Be("T1");
            genericParameterType2.Name.Should().Be("T2");
            genericParameterType1.IsClosedConstructed.Should().BeFalse();
            genericParameterType2.IsClosedConstructed.Should().BeFalse();
        }

        [TestCaseSource(nameof(AccessModifier_is_set_correctlyCases))]
        public void AccessModifier_is_set_correctly(string methodName, object accessModifier)
        {
            // arrange
            var info = GetInfo(methodName);

            // act
            var sut = this.GetSut(info);

            // assert
            sut.AccessModifier.Should().Be(accessModifier);
        }

        [TestCaseSource(nameof(OverrideKind_is_set_correctlyCases))]
        public void OverrideKind_is_set_correctly(MethodInfo methodInfo, object overrideKind)
        {
            // act
            var sut = this.GetSut(methodInfo);

            // assert
            sut.OverrideKind.Should().Be(overrideKind);
        }

        [TestCase(nameof(TestClass.IntMethod), typeof(int))]
        [TestCase(nameof(TestClass.StringMethod), typeof(string))]
        [TestCase(nameof(TestClass.PublicVoidMethod), typeof(void))]
        public void Type_is_set_correctly(string methodName, Type expectedType)
        {
            // arrange
            var info = GetInfo(methodName);

            // act
            var sut = this.GetSut(info, expectedType);

            // assert
            sut.Type.Should().Be(expectedType);
        }

        [TestCase(nameof(TestClass.PublicVoidMethodWithParameters))]
        [TestCase(nameof(TestClass.PublicIntMethodWithParameters))]
        [TestCase(nameof(TestClass.IntMethod))]
        public void Type_is_set_correctly2(string methodName)
        {
            // arrange
            var info = GetInfo(methodName);

            // act
            var sut = this.GetSut(info);
            var expectedParameters = info.GetParameters().Select(p => p.Name).ToCommaSeparated();
            var expectedParameterFullTypes = info.GetParameters().Select(p => p.ParameterType.FullName).ToCommaSeparated();

            // assert
            sut.DeclaredParameters.Length.Should().Be(info.GetParameters().Length);
            sut.Parameters.Should().Be(expectedParameters);
            sut.FriendlyParameterFullTypes.Should().Be(expectedParameterFullTypes);
        }

        [TestCase(nameof(TestClass.IntMethod), typeof(int))]
        [TestCase(nameof(TestClass.StringMethod), typeof(string))]
        [TestCase(nameof(TestClass.PublicVoidMethod), typeof(void))]
        public void GetReturnTypeDescription_returns_the_correct_description(string methodName, Type expectedType)
        {
            // arrange
            var info = GetInfo(methodName);

            var expected =
                new ReflectionTypeDescription(expectedType, new ItemBuilder<IBaseTypeService>().Build());

            // act
            var sut = this.GetSut(info, expectedType);

            // assert
            sut.GetReturnTypeDescription().Should().Be(expected);
        }


        [Test]
        public void Type_Booleans_for_method_are_all_false()
        {

            // arrange
            var methodInfo = GetInfo(nameof(TestClass.PublicVoidMethodWithParameters));

            // act
            var sut = this.GetSut(methodInfo);

            // assert
            sut.IsClass.Should().BeFalse();
            sut.IsInterface.Should().BeFalse();
            sut.IsStruct.Should().BeFalse();
            sut.IsEnum.Should().BeFalse();
        }

        private ReflectionMethodDescription GetSut(MethodBase methodBase, Maybe<Type> returnType = default) =>
            new(methodBase,
                returnType.SomeOrProvided(typeof(void)),
                new ItemBuilder<IBaseTypeService>().Build());

        private static MethodInfo GetInfo(string name, Type type = null) =>
            (type ?? typeof(TestClass)).GetMethod(
                name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        #endregion

        #region Nested type: TestClass

        private abstract class TestClass
        {
            #region ctors

            public TestClass()
            {
            }

            protected TestClass(int a)
            {
            }

            #endregion

            #region members

            public void PublicVoidMethod() => throw new NotImplementedException();

            public void PublicVoidMethodWithParameters
                (int p1, string p2, ReflectionMethodDescriptionTests p3) => throw new NotImplementedException();

            public int PublicIntMethodWithParameters
                (float name1, char p2, AccessModifier modifier) => throw new NotImplementedException();

            public abstract void AbstractMethod();

            public static void StaticMethod() => throw new NotImplementedException();

            public void GenericMethod<T1, T2>() => throw new NotImplementedException();

            public abstract int IntMethod();

            public abstract string StringMethod();

            protected abstract void ProtectedMethod();

            internal void InternalMethod() => throw new NotImplementedException();

#pragma warning disable IDE0051 // Remove unused private members
            private void PrivateMethod() => throw new NotImplementedException();
#pragma warning restore IDE0051 // Remove unused private members

            #endregion
        }

        #endregion

        #region Nested type: TestClass2

        private class TestClass2 : TestClass
        {
            #region members

            /// <inheritdoc />
            public sealed override void AbstractMethod() => throw new NotImplementedException();

            /// <inheritdoc />
            public override int IntMethod() => throw new NotImplementedException();

            /// <inheritdoc />
            public override string StringMethod() => throw new NotImplementedException();

            /// <inheritdoc />
            protected override void ProtectedMethod() => throw new NotImplementedException();

            #endregion
        }

        #endregion
    }
}