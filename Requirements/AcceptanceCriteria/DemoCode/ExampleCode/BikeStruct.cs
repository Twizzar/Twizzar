using DemoCode.Interfaces.ExampleCode;

namespace DemoCode.ExampleCode
{
    public struct BikeStruct
    {
        public float Speed { get; set; }

        public IVehicle Vehicle { get; set; }

        public Garage Garage { get; set; }

        public TestStruct MyProperty { get; set; }

        public BikeStruct(float speed, IVehicle vehicle, Garage garage)
        {
            Speed = speed;
            Vehicle = vehicle;
            Garage = garage;
            MyProperty = new TestStruct();
        }
    }
}
