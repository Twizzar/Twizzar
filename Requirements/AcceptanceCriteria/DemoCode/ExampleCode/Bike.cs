using DemoCode.Interfaces.ExampleCode;

namespace DemoCode.ExampleCode
{
    public class Bike : IVehicle
    {
        /// <inheritdoc />
        public float Speed { get; set; }
    }
}
