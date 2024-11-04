using Twizzar.Fixture;
using DemoCode.Car;

namespace DemoCode.Tests
{
    partial class BaseFeaturesTests
    {
        private class Card637Builder : ItemBuilder<DemoCode.Car.Car, Card637BuilderPaths>
        {
            public Card637Builder()
            {
                this.With(p => p.IntPropSetByCtor.Value(78));
                this.With(p => p.FloatProp.Value(12f));
                this.With(p => p.DoubleProp.Value(5D));
                this.With(p => p.DecimalProp.Value(7.2M));
            }
        }

        private class RedCarBuilder : ItemBuilder<Car.Car, RedCarPaths>
        {
            public RedCarBuilder()
            {
                this.With(p => p.SerialNumber.Value("RedCar"));
                this.With(p => p.IntPropSetByCtor.Value(630));
            }
        }

        private class CarWithoutRadioAnd3WheelsBuilder : ItemBuilder<Car.Car, CarWithoutRadioAnd3WheelsPaths>
        {
            public CarWithoutRadioAnd3WheelsBuilder()
            {
                this.With(p => p.Ctor.hasRadio.Value(false));
                this.With(p => p.Ctor.wheelCount.Value(3));
            }
        }
    }
}