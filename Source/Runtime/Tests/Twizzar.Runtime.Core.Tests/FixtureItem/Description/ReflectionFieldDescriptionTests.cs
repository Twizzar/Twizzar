using System.Reflection;

using FluentAssertions;

using NUnit.Framework;
using Twizzar.Runtime.Core.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.TestCommon.TypeDescription;
using TwizzarInternal.Fixture;

// ReSharper disable InconsistentNaming
// ReSharper disable UnassignedField.Global
#pragma warning disable 649
#pragma warning disable 169
#pragma warning disable S1144 // Unused private types or members should be removed
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable 0169 // The field is never used
#pragma warning disable 0649 // Is never assigned
#pragma warning disable 414

namespace Twizzar.Runtime.Core.Tests.FixtureItem.Description
{
    public partial class ReflectionFieldDescriptionTests
    {
        #region static fields and constants

        private static object[] AccessModifier_is_set_correctlyCases =
        {
            new object[] {(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.IntField)), AccessModifier.CreatePublic()},
            new object[] {"protectedInt", AccessModifier.CreateProtected()},
            new object[] {"privateInt", AccessModifier.CreatePrivate()},
            new object[] {"internalInt", AccessModifier.CreateInternal()},
        };

        private int _testField;

        #endregion

        #region members

        [Test]
        public void All_Ctor_parameter_throws_ArgumentNullException_when_null()
        {
            // arrange
            var fieldInfo = typeof(ReflectionFieldDescriptionTests).GetField(nameof(this._testField),
                BindingFlags.NonPublic | BindingFlags.Instance);

            // assert
            Verify.Ctor<ReflectionFieldDescription>()
                .SetupParameter("info", fieldInfo)
                .ShouldThrowArgumentNullException();
        }

        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.IntField))]
        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.ConstStr))]
        public void Name_is_set_correctly(string fieldName)
        {
            // arrange
            var tester = CreateTester(fieldName);

            // act & assert
            tester.Name_is_set_correctly(fieldName);
        }

        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.IntField))]
        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.ConstStr))]
        public void TypeFullName_is_set_correctly(string fieldName)
        {
            // arrange
            var fieldInfo = GetFieldInfo(fieldName);
            var tester = CreateTester(fieldName);
            
            // act
            tester.TypeFullName_is_set_correctly(fieldInfo.FieldType.FullName);
        }

        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.IntField))]
        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.ConstStr))]
        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.StaticFloat))]
        public void DeclaringType_is_set_correctly(string fieldName)
        {
            // arrange
            var tester = CreateTester(fieldName);

            // act & assert
            tester.DeclaringType_is_set_correctly(typeof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass).FullName);
        }

        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.IntField), false)]
        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.ConstStr), true)]
        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.StaticFloat), false)]
        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.ReadonlyInt), false)]
        public void IsConstant_is_set_correctly(string fieldName, bool isConstant)
        {
            // arrange
            var tester = CreateTester(fieldName);

            // act & assert
            tester.IsConstant_is_set_correctly(isConstant);
        }

        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.IntField), null)]
        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.ConstStr), "TestStr")]
        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.StaticFloat), null)]
        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.ReadonlyInt), null)]
        public void ConstantValue_is_set_correctly(string fieldName, string value)
        {
            // arrange
            var tester = CreateTester(fieldName);

            // act & assert
            tester.ConstantValue_is_set_correctly(value);
        }

        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.IntField), false)]
        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.ConstStr), true)]
        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.StaticFloat), true)]
        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.ReadonlyInt), false)]
        public void IsStatic_is_set_correctly(string fieldName, bool isStatic)
        {
            // arrange
            var tester = CreateTester(fieldName);

            // act & assert
            tester.IsStatic_is_set_correctly(isStatic);
        }

        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.IntField), false)]
        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.ConstStr), false)]
        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.StaticFloat), false)]
        [TestCase(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.ReadonlyInt), true)]
        public void IsReadonly_is_set_correctly(string fieldName, bool isReadonly)
        {
            // arrange
            var tester = CreateTester(fieldName);

            // act & assert
            tester.IsReadonly_is_set_correctly(isReadonly);
        }

        [TestCaseSource(nameof(AccessModifier_is_set_correctlyCases))]
        public void AccessModifier_is_set_correctly(string fieldName, object accessModifier)
        {
            // arrange
            var tester = CreateTester(fieldName);

            // act & assert
            tester.AccessModifier_is_set_correctly(accessModifier);
        }

        [Test]
        public void ReturnTypeDescription_is_set_correctly()
        {
            // arrange
            var baseTypeService = new ItemBuilder<IBaseTypeService>().Build();
            var fieldInfo = GetFieldInfo(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.IntField));
            var expectedTypeDescription = new ReflectionTypeDescription(typeof(int), baseTypeService);

            // act
            var sut = new ReflectionFieldDescription(fieldInfo, baseTypeService);

            // assert
            sut.GetReturnTypeDescription().Should().Be(expectedTypeDescription);
        }

        [Test]
        public void Type_Booleans_for_class_are_set_correctly()
        {
            // arrange
            var tester = CreateTester(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.TestClassField));

            // act & assert
            tester.Type_Booleans_for_class_are_set_correctly();
        }

        [Test]
        public void Type_Booleans_for_interfaces_are_set_correctly()
        {
            // arrange
            var tester = CreateTester(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.TestInterfaceField));

            // act & assert
            tester.Type_Booleans_for_interfaces_are_set_correctly();
        }

        [Test]
        public void Type_Booleans_for_struct_are_set_correctly()
        {
            // arrange
            var tester = CreateTester(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.TestStructField));

            // act & assert
            tester.Type_Booleans_for_struct_are_set_correctly();
        }

        [Test]
        public void Type_Booleans_for_enum_are_set_correctly()
        {
            // arrange
            var tester = CreateTester(nameof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass.TestEnumField));

            // act & assert
            tester.Type_Booleans_for_enum_are_set_correctly();
        }

        private static FieldInfo GetFieldInfo(string name) =>
            typeof(FieldDescriptionTester<ReflectionFieldDescription>.TestClass).GetField(name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        private static FieldDescriptionTester<ReflectionFieldDescription> CreateTester(string fieldName) =>
            new(
                new ItemBuilder<ReflectionFieldDescription>()
                .With(p => p.Ctor.info.Value(GetFieldInfo(fieldName))));

        #endregion
    }
}