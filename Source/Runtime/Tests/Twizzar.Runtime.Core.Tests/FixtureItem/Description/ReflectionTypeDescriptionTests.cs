using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;
using Twizzar.Runtime.Core.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon.TypeDescription;
using TwizzarInternal.Fixture;

// ReSharper disable UnusedTypeParameter
#pragma warning disable S2326 // Unused type parameters should be removed

namespace Twizzar.Runtime.Core.Tests.FixtureItem.Description
{
    public partial class ReflectionTypeDescriptionTests
    {
        #region members

        [Test]
        public void All_Ctor_parameter_throws_ArgumentNullException_when_null()
        {
            // arrange
            var type = typeof(TypeDescriptionTester<>.TestClass);

            // assert
            Verify.Ctor<ReflectionTypeDescription>()
                .SetupParameter("type", type)
                .ShouldThrowArgumentNullException();
        }

        [TestCase(typeof(TypeDescriptionTester<>.TestClass), typeof(object))]
        [TestCase(typeof(ReflectionTypeDescription), typeof(TypeDescription))]
        [TestCase(typeof(TypeDescription), typeof(BaseTypeDescription))]
        public void BaseType_is_set_correctly(Type type, Type expectedBaseType)
        {
            // arrange
            var tester = CreateTester(type);

            // act & assert
            tester
                .BaseType_is_set_correctly(expectedBaseType);
        }

        [TestCase(typeof(TypeDescriptionTester<>.TestClass), new Type[] { })]
        [TestCase(typeof(TypeDescriptionTester<>.ClassA), new[] {typeof(TypeDescriptionTester<>.IA1), typeof(TypeDescriptionTester<>.IA2), typeof(TypeDescriptionTester<>.IA3), typeof(TypeDescriptionTester<>.IC1), typeof(TypeDescriptionTester<>.IB1) })]
        public void ImplementedInterfaceNames_is_set_correctly(
            Type type,
            IEnumerable<Type> expectedImplementedInterfaceNames)
        {
            // arrange
            var tester = CreateTester(type);

            // act & assert
            tester
                .ImplementedInterfaceNames_is_set_correctly(expectedImplementedInterfaceNames);
        }

        [TestCase(typeof(TypeDescriptionTester<>.TestClass), false)]
        [TestCase(typeof(TypeDescriptionTester<>.SealedClass), true)]
        public void IsSealed_is_set_correctly(Type type, bool expectedIsSealed)
        {
            // arrange
            var tester = CreateTester(type);

            // act & assert
            tester
                .IsSealed_is_set_correctly(expectedIsSealed);
        }

        [TestCase(typeof(TypeDescriptionTester<>.TestClass), false)]
        [TestCase(typeof(TypeDescriptionTester<>.StaticClass), true)]
        public void IsStatic_is_set_correctly(Type type, bool expectedIsStatic)
        {
            // arrange
            var tester = CreateTester(type);

            // act & assert
            tester
                .IsStatic_is_set_correctly(expectedIsStatic);
        }

        [TestCase(typeof(NonGenericClass), false)]
        [TestCase(typeof(TypeDescriptionTester<>.GenericClass<,>), true)]
        public void IsGeneric_is_set_correctly(Type type, bool expectedIsGeneric)
        {
            // arrange
            var tester = CreateTester(type);

            tester
                .IsGeneric_is_set_correctly(expectedIsGeneric);
        }

        [TestCase(typeof(TypeDescriptionTester<>.TestClass), true)]
        [TestCase(typeof(ReflectionTypeDescriptionTests), false)]
        public void IsNested_is_set_correctly(Type type, bool expectedIsNested)
        {
            // arrange
            var tester = CreateTester(type);

            tester
                .IsNested_is_set_correctly(expectedIsNested);
        }

        [Test]
        public void All_properties_from_implemented_interfaces_are_found()
        {
            // arrange
            var expectedPropertyNames = new[]
            {
                nameof(TypeDescriptionTester<ReflectionTypeDescription>.IA3.IA3Prop),
                nameof(TypeDescriptionTester<ReflectionTypeDescription>.IA3.IB1Prop),
                nameof(TypeDescriptionTester<ReflectionTypeDescription>.IA3.IC1Prop),
            };

            // act
            var sut = new ItemBuilder<ReflectionTypeDescription>()
                .With(p => p.Ctor.type.Value(typeof(TypeDescriptionTester<>.IA3)))
                .Build();

            // assert
            sut.GetDeclaredProperties()
                .Select(description => description.Name)
                .Should()
                .Contain(expectedPropertyNames);
        }

        [TestCase(typeof(List<>), true)]
        [TestCase(typeof(List<string>), true)]
        [TestCase(typeof(TypeDescriptionTester<ReflectionTypeDescription>.TestCollection), true)]
        [TestCase(typeof(ICollection<>), false)]
        [TestCase(typeof(Dictionary<,>), true)]
        [TestCase(typeof(Dictionary<int,string>), true)]
        public void Is_InheritedICollection_determines_correctly(Type type, bool expectedValue)
        {
            // arrange
            var tester = CreateTester(type);

            // act & assert
            tester
                .IsInheritedFromICollection_set_correctly(expectedValue);
        }

        [TestCase(typeof(int[]), true)]
        [TestCase(typeof(string[]), true)]
        [TestCase(typeof(IPropertyDescription[]), true)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(string), false)]
        [TestCase(typeof(IPropertyDescription), false)]
        public void IsArray_is_set_correctly(Type type, bool expectedValue)
        {
            // arrange
            var tester = CreateTester(type);

            // act & assert
            tester.IsArray_is_set_correctly(expectedValue);
        }

        [TestCase(typeof(int[]), new int[] { 1 })]
        [TestCase(typeof(int[][]), new int[] { 1, 1 })]
        [TestCase(typeof(int[][][]), new int[] { 1, 1, 1 })]
        [TestCase(typeof(int[,]), new int[] { 2 })]
        [TestCase(typeof(int[,][,,]), new int[] { 3, 2 })]
        [TestCase(typeof(int[,,][]), new int[] { 1, 3 })]
        public void ArrayDimension_is_set_correctly(Type type, int[] expectedValue)
        {
            // arrange
            var tester = CreateTester(type);

            // act & assert
            tester.ArrayDimension_is_set_correctly(expectedValue);
            
        }

        #endregion

        private static TypeDescriptionTester<ReflectionTypeDescription> CreateTester(Type type) =>
            new(new ItemBuilder<ReflectionTypeDescription>()
                .With(p => p.Ctor.type.Value(type)));

        public class NonGenericClass { }

    }
}