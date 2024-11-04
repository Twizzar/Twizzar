using Twizzar.Fixture;
using DemoCode.ExampleCode;

namespace Net48Tests
{
    partial class UnitTest1
    {
        private class Car9524Builder : ItemBuilder<DemoCode.ExampleCode.Car, Car9524BuilderPaths>
        {
            public Car9524Builder()
            {
                this.With(p => p.Seats.Value(4));
            }
        }
    }
}