using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Fixture;

namespace Twizzar.Runtime.Test.IntegrationTest
{
    [TestClass]
    [TestCategory("IntegrationTest")]
    public partial class SettingFieldsTest
    {
        [TestMethod]
        public void Public_field_is_set_to_value_in_config()
        {
            var instance = new TestFieldsBuilder().Build();

            instance.PublicInt.Should().Be(5);
        }

        [TestMethod]
        public void Private_field_is_set_to_value_in_config()
        {
            var instance = new TestFieldsBuilder().Build();

            instance.PrivateIntValue.Should().Be(6);
        }

        [TestMethod]
        public void Protected_field_is_set_to_value_in_config()
        {
            var instance = new TestFieldsBuilder().Build();

            instance.ProtectedInt.Should().Be(7);
        }

        [TestMethod]
        public void Internal_field_is_set_to_value_in_config()
        {
            var instance = new TestFieldsBuilder().Build();

            instance.InternalInt.Should().Be(8);
        }

        [TestMethod]
        public void ReadOnly_field_is_set_to_value_in_config()
        {
            var instance = new TestFieldsBuilder().Build();

            instance.ReadonlyInt.Should().Be(9);
        }

        [TestMethod]
        public void Not_set_does_not_touch_the_field()
        {
            static void TestGeneric<T>()
            {
                var value = new ItemBuilder<GenericTestClass<T>>()
                    .Build();
                value.PublicField.Should().Be(default(T));
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
    }

#pragma warning disable S3459 // Unassigned members should be removed
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable 649
// ReSharper disable ConvertToAutoProperty


    public class TestClass
    {
        public int PublicInt;

        private int _privateInt;


        public readonly int ReadonlyInt;

        // ReSharper disable once InconsistentNaming
        protected int _protectedInt;

        public int InternalInt;

        public int PrivateIntValue =>  this._privateInt;

        public int ProtectedInt =>  this._protectedInt;
    }

    public class GenericTestClass<T>
    {
        public T PublicField;
    }

#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore S3459 // Unassigned members should be removed
#pragma warning restore 649 
// ReSharper restore ConvertToAutoProperty
}
