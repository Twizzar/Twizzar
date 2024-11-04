using System.Linq;

using DemoCode.Car;
using DemoCode.ExampleCode;

using FluentAssertions;
using NUnit.Framework;
using Twizzar.Fixture;

namespace DemoCode.Tests
{
    [TestFixture]
    public partial class BaseFeaturesTests
    {
        [Test]
        public void Build_New()
        {
            // Entry point to fixture container
            // Helpful for accessing sut and input values for method under test.
            var car = new ItemBuilder<Car.Car>().Build();

            car.Should().BeAssignableTo<Car.Car>();

            // asking for resolving a configuration
            var car2 = new ItemBuilder<Car.Car>().Build();
            car.Should().NotBeSameAs(car2);

            // same valid for build with configuration:
            var redCar = new RedCarBuilder().Build();
            var redCar2 = new RedCarBuilder().Build();
            redCar.Should().NotBeSameAs(redCar2);
        }

        [Test]
        public void Build_New_Class()
        {
            // default: largest public ctor will be chosen.
            // if no public ctor available, largest non public ctor will be chosen.
            // all dependencies in ctor will be resolved with default configuration.
            var car = new ItemBuilder<Car.Car>().Build();

            // others will not be touched.
            car.InterfacePropNotSetByCtor.Should().Be(default(IEngine));
            car.InterfacePropNotSetByCtor.Should().Be(null);

            // if defined by configuration, value will be set once:
            var redCar = new RedCarBuilder().Build();
            redCar.IntPropSetByCtor.Should().Be(630);
            redCar.IntPropSetByCtor = 42;
            redCar.IntPropSetByCtor.Should().Be(42);
        }

        [Test]
        public void Build_New_BaseTypes()
        {
            // supported base types:
            // numeric types like int, long, double, float, single, etc.
            // string, char
            // bool

            // default: unique
            var intValue = new ItemBuilder<int>().Build();
            var intValue2 = new ItemBuilder<int>().Build();
            intValue2.Should().NotBe(intValue);

            // string: GUID
            // bool: true
            // numeric value: unique value

            // dependency in ctor will be resolved unique
            var car = new ItemBuilder<Car.Car>().Build();
            var car2 = new ItemBuilder<Car.Car>().Build();
            car2.SerialNumber.Should().NotBe(car.SerialNumber);
            car2.WheelCount.Should().NotBe(car.WheelCount);

            // others will not be touched.
            car.IntPropSetByCtor.Should().Be(default(int));

            // values can be set:
            var specialCar = new CarWithoutRadioAnd3WheelsBuilder().Build();
            specialCar.WheelCount.Should().Be(3);
            specialCar.HasRadio.Should().BeFalse();

            // other keywords like:
            // range, startsWith, positive, random,
            // etc. will extend concept later one.
        }

        [Test]
        public void Build_New_Interfaces()
        {
            // default: mock instance which is not further configured.
            var engine = new ItemBuilder<IEngine>().Build();
            engine.CylinderCount.Should().Be(default);

            // dependency in ctor will be resolved as mock
            var car = new ItemBuilder<Car.Car>().Build();
            car.Engine.Should().BeAssignableTo<IEngine>();

            // others will not be touched.
            car.InterfacePropNotSetByCtor.Should().Be(default(IEngine));
            car.InterfacePropNotSetByCtor.Should().Be(null);

        }

        [Test]
        public void Enum_is_BaseType_as_well()
        {
            // default: unique
            var enum1 = new ItemBuilder<MyEnum>().Build();
            var enum2 = new ItemBuilder<MyEnum>().Build();
            var enum3 = new ItemBuilder<MyEnum>().Build();
            var enum1Again = new ItemBuilder<MyEnum>().Build();

            enum1.Should().NotBe(enum2).And.NotBe(enum3).And.Be(enum1Again);

            // limited UI support so far
            // can only be set with index numbers
        }

        [Test]
        public void Structs_are_supported_as_well()
        {
            // default: same behavior as classes
            // structs always have a public empty ctor, which will be chosen if
            // no other public ctor is available.
            var testStruct = new ItemBuilder<TestStruct>().Build();

            testStruct.Should().BeAssignableTo<TestStruct>();
            testStruct.X.Should().NotBe(testStruct.Y);
        }

        [Test]
        public void Build_Many()
        {
            // resolves the asked configuration n times:
            var cars = new ItemBuilder<Car.Car>().BuildMany(5).ToList();
            cars.Count.Should().Be(5);
            cars.First().Should().NotBe(cars.Last());

            // same can be done with specific configuration:
            var redCars = new RedCarBuilder().BuildMany(50).ToList();
            redCars.Count.Should().Be(50);
            cars.First().Should().NotBe(cars.Last());
        }

        [Test]
        public void FixtureItem_VerifyCtor()
        {

            //var engine = new Mock<IEngine>();
            //engine.Setup(e => e.AnotherEngine)
            //    .Returns(new Mock<IEngine>().Object);


            // checks if ctor throws ArgumentNullExceptions for all nullable types.
            //FixtureItem.VerifyCtor<Car.Car>()
                //.IgnoreThisParameter("serialNumber", "123")
                //.AddInstance("engine", engine.Object)
            //.CheckCtorParameterThrowsArgumentNullException();

            // checks all ctors
            // if ctor wil be extended, parameter will be checked automatically.
        }
    }
}