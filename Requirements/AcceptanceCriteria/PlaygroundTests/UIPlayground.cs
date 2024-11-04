using System;
using System.Collections.Generic;
using DemoCode.ExampleCode;
using DemoCode.Interfaces.ExampleCode;

using NUnit.Framework;
using Twizzar.Fixture;

namespace PlaygroundTests
{
    [TestFixture]
    public partial class UIPlayground
    {
        [Test]
        public void Test1()
        {
            var aa = new ClassWithInternalDependeciesdacaBuilder()
                .Build();

            new ItemBuilder<ClassWitPrivateCtor>();

            new ItemBuilder<Garage>()
                .Build();

            new ItemBuilder<Garage>().Build();

            new ItemBuilder<UIPlayground>();

            var classA = new ItemBuilder<ClassA>().Build();

            Assert.IsInstanceOf<Garage>(classA.Garage10);

            new ItemBuilder<BigClass2>();

            var bla = new ItemBuilder<GenericsTest>().Build();

            new ItemBuilder<ClassA>();
            var structTest = new ItemBuilder<TestStruct>();

            var myInt = new ItemBuilder<int?>();

            var defaultEmailMessageBuffer = new ItemBuilder<DemoCode.EmailMessageBuffer>();

            var superLong = new ItemBuilder<System.Int64>();

            var asdfawsdgfasdf = (new ItemBuilder<TestStruct>(), new ItemBuilder<ClassA>());

            var classA2 = new ItemBuilder<ClassA>();

            var garage = new ItemBuilder<Garage>();
            
            var memberTest = new ItemBuilder<MemberTest>();

            var genericsTest = new ItemBuilder<GenericsTest>();

            var genericTestTuple = new ItemBuilder<GenericTest<Tuple<int, string>>>();

            var listTest = new ItemBuilder<ListTest>();

            var garages = new ItemBuilder<Garage>().BuildMany(5);

            var empty = new ItemBuilder<NotTheContainer>();

            var statusTest = new ItemBuilder<StatusTest>();

            var marker = new ItemBuilder<IMarkerInterface>();

            var tobi = new ItemBuilder<List<int>>();
            var t = new ItemBuilder<IEnumerable<double>>();

            var list = new ItemBuilder<ListTest>();

            new ItemBuilder<IVehicle>();

            var blub = new ItemBuilder<MyEnum>();

            var bikeStruct = new ItemBuilder<BikeStruct>();


            // Arrange
            Assert.IsTrue(true);
        }

        [Test]
        public void RealLiteralsTest()
        {
            var sut = new Car1d6fBuilder().Build();

            Assert.That(sut.DecimalProp, Is.EqualTo(2.01m));
            Assert.That(sut.DoubleProp, Is.EqualTo(2.0));
            Assert.That(sut.FloatProp, Is.EqualTo(3.0f));
        }
    }
}