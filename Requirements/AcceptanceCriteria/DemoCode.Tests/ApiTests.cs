using System;
using System.Collections.Generic;

using DemoCode.ExampleCode;

using FluentAssertions;

using NUnit.Framework;
using Twizzar.Fixture;

namespace DemoCode.Tests
{
    public partial class ApiTests
    {
        public class MyClass
        {
            public int MyProperty { get; }
        }

        [Test]
        public void Test()
        {
            var myClass = new ItemBuilder<MyClass>()
                .With(p => p.MyPropertyk__BackingField.Value(2))
                .Build();

            myClass.MyProperty.Should().Be(2);
        }


        [Test]
        public void ctor_verifier_ignores_parameters()
        {
            // act
            Verify.Ctor<Car.Car>()
                .IgnoreParameter("wheelCount", (byte)3)
                .IgnoreParameter("model", 'c')
                .IgnoreParameter("hasRadio", true)
                .IgnoreParameter("serialNumber", "abc")
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public void Get_empty_struct_from_Fixture()
        {
            // act
            var value = new ItemBuilder<EmptyStruct>().Build();

            // assert
            value.Should().NotBe(null);
        }

        [Test]
        public void Get_simple_struct_from_Fixture_has_correct_value()
        {
            // act
            var x = new ItemBuilder<double>().Build();
            var y = new ItemBuilder<double>().Build();
            var z = new ItemBuilder<double>().Build();

            // assert
            var simpleStruct = new ItemBuilder<TestStruct>()
                .With(p => p.Xk__BackingField.Value(x))
                .With(p => p.Y.Value(y))
                .Build();
            
            simpleStruct.X.Should().Be(x);
            simpleStruct.Y.Should().Be(y);
            simpleStruct.Z.Should().Be(4);
        }

        [Test]
        public void Get_complex_struct_from_Fixture_has_correct_value()
        {
            // act
            var x = new ItemBuilder<double>().Build();
            var y = new ItemBuilder<double>().Build();

            // assert
            var complexStruct = new ItemBuilder<AnotherTestStruct>()
                .With(p => p.Left.Xk__BackingField.Value(x))
                .With(p => p.Left.Y.Value(y))
                .Build();

            complexStruct.Left.X.Should().Be(x);
            complexStruct.Left.Y.Should().Be(y);
            complexStruct.Left.Z.Should().Be(4);

            complexStruct.Right.X.Should().NotBe(default);
            complexStruct.Right.Y.Should().NotBe(default);
            complexStruct.Right.Z.Should().NotBe(default);
        }

        [Test]
        public void Nullable_baseType_behave_like_a_baseType()
        {
            // act
            var instance1 = new ItemBuilder<int?>().Build();
            var instance2 = new ItemBuilder<int?>().Build();

            // assert
            instance1.HasValue.Should().BeTrue();
            instance2.HasValue.Should().BeTrue();
            instance1.Should().NotBe(instance2);
        }

        [TestCase(typeof(IList<int>))]
        [TestCase(typeof(Tuple<int, string>))]
        [TestCase(typeof(IEnumerable<Tuple<int, string>>))]
        [TestCase(typeof(int?))]
        public void Generic_types_get_instantiated(Type type)
        {
            // act
            static object Create<T>() =>
                new ItemBuilder<T>().Build();

            var methodInfo = ((Func<object>)Create<object>).Method.GetGenericMethodDefinition();
            var genericMethodInfo = methodInfo.MakeGenericMethod(type);
            var instance = genericMethodInfo.Invoke(null, new object[] { });

            // assert
            instance.Should().NotBeNull();
        }

        [TestCase(-155)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(254)]
        public void Public_field_is_set_to_value_in_yaml_file(int expected)
        {
            var instance = new ItemBuilder<TestClass>()
                .With(p => p.PublicInt.Value(expected))
                .Build();

            instance.PublicInt.Should().Be(expected);
        }

        [TestCase(-155)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(254)]
        public void Internal_field_is_set_to_value_in_yaml_file(int expected)
        {
            var instance = new ItemBuilder<TestClass>()
                .With(p => p.InternalInt.Value(expected))
                .Build();

            instance.InternalInt.Should().Be(expected);
        }

        [TestCase(-155)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(254)]
        public void ReadOnly_field_is_set_to_value_in_yaml_file(int expected)
        {
            var instance = new ItemBuilder<TestClass>()
                .With(p => p.ReadonlyInt.Value(expected))
                .Build();

            instance.ReadonlyInt.Should().Be(expected);
        }

        [Test]
        public void Not_set_does_not_touch_the_field()
        {
            static void TestGeneric<T>()
            {
                var instance = new ItemBuilder<GenericTestClass<T>>().Build();
                instance.PublicField.Should().Be(default(T));
            }

            TestGeneric<sbyte>();
            TestGeneric<byte>();
            TestGeneric<ushort>();
            TestGeneric<int>();
            TestGeneric<uint>();
            TestGeneric<long>();
            TestGeneric<ulong>();

            TestGeneric<decimal>();
            TestGeneric<double>();
            TestGeneric<float>();

            TestGeneric<string>();
            TestGeneric<char>();

            TestGeneric<object>();
        }

        [Test]
        public void LongPathTest()
        {
            var instance = new LongPathClassDBuilder().Build();
            instance.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.A1.Value.Should().Be(42);
            instance.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.A1.Value.Should().Be(42);
        }


        [Test]
        public void InstanceOf()
        {
            var garage = new MyGarageBuilder()
                .Build();

            var bike = garage.Vehicle1.Should().BeOfType<Bike>().Subject;
            var eBike = garage.Vehicle3.Should().BeOfType<EBike>().Subject;

            bike.Speed.Should().Be(42);
            eBike.mhw.Should().Be(3);
        }
    }

    internal class TestClass
    {
        public int PublicInt;

        private int _privateInt;


        public readonly int ReadonlyInt;

        // ReSharper disable once InconsistentNaming
        protected int _protectedInt;

        internal int InternalInt;

        public int PrivateIntValue => this._privateInt;

        public int ProtectedInt => this._protectedInt;
    }

    internal class GenericTestClass<T>
    {
        public T PublicField;
    }
}
