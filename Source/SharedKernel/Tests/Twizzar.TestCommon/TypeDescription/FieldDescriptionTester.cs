using FluentAssertions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.TestCommon.TypeDescription
{
    public class FieldDescriptionTester<T> where T: IFieldDescription
    {
        private readonly IItemBuilder<T> _builder;

        public FieldDescriptionTester(IItemBuilder<T> builder)
        {
            this._builder = builder;
        }

        public void Name_is_set_correctly(string expectedFieldName)
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.Name.Should().Be(expectedFieldName);
        }

        public void TypeFullName_is_set_correctly(string expectedFullName)
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.TypeFullName.FullName.Should().Be(expectedFullName);
        }

        public void DeclaringType_is_set_correctly(string expectedFullName)
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.DeclaringType.FullName.Should().Be(expectedFullName);
        }

        public void IsConstant_is_set_correctly(bool isConstant)
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.IsConstant.Should().Be(isConstant);
        }

        public void ConstantValue_is_set_correctly(string value)
        {
            // act
            var sut = this._builder.Build();
            var constValue = Maybe.ToMaybe<object>(value);

            // assert
            sut.ConstantValue.Should().Be(constValue);
        }

        public void IsStatic_is_set_correctly(bool isStatic)
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.IsStatic.Should().Be(isStatic);
        }

        public void IsReadonly_is_set_correctly(bool isReadonly)
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.IsReadonly.Should().Be(isReadonly);
        }

        public void AccessModifier_is_set_correctly(object accessModifier)
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.AccessModifier.Should().Be(accessModifier);
        }

        public void Type_Booleans_for_class_are_set_correctly()
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.IsClass.Should().BeTrue();
            sut.IsInterface.Should().BeFalse();
            sut.IsStruct.Should().BeFalse();
            sut.IsEnum.Should().BeFalse();
        }

        public void Type_Booleans_for_interfaces_are_set_correctly()
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.IsClass.Should().BeFalse();
            sut.IsInterface.Should().BeTrue();
            sut.IsStruct.Should().BeFalse();
            sut.IsEnum.Should().BeFalse();
        }

        public void Type_Booleans_for_struct_are_set_correctly()
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.IsClass.Should().BeFalse();
            sut.IsInterface.Should().BeFalse();
            sut.IsStruct.Should().BeTrue();
            sut.IsEnum.Should().BeFalse();
        }

        public void Type_Booleans_for_enum_are_set_correctly()
        {
            // act
            var sut = this._builder.Build();

            // assert
            sut.IsClass.Should().BeFalse();
            sut.IsInterface.Should().BeFalse();
            sut.IsStruct.Should().BeFalse();
            sut.IsEnum.Should().BeTrue();
        }

        public class TestClass
        {
            #region static fields and constants

            public const string ConstStr = "TestStr";
            public static float StaticFloat = 2f;

            #endregion

            public TestClass(int value)
            {
                this.internalInt = value;
                this.privateInt = value;
            }

            #region fields

            public readonly int ReadonlyInt = 4;
            public int IntField = 3;
            public TestClass TestClassField = null;
            public ITestInterface TestInterfaceField = null;
            public TestStruct TestStructField;
            public TestEnum TestEnumField;
            protected int protectedInt;
            internal int internalInt;
            private int privateInt;

            public int AutoProperty { get; set; }


            #endregion
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
