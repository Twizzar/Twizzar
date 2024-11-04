using DemoCode.Interfaces.ExampleCode;
using System.CodeDom;

namespace DemoCode.ExampleCode
{
    public class Garage
    {
        public IVehicle Vehicle1 { get; set; }

        public IVehicle Vehicle2 { get; set; }
        public IVehicle Vehicle3 { get; set; }
    }

    public interface IGarage
    {
        IVehicle Vehicle1 { get; set; }
        IVehicle Vehicle2 { get; set; }
        IVehicle Vehicle3 { get; set; }
        IVehicle GetVehicle(int number);
    }

    public class GargeWrapper
    {
        public readonly IGarage Garage;

        public GargeWrapper(IGarage g, ICar car)
        {
            Garage = g;
        }
    }

    public class Garage2 : Garage
    {

    }
}
