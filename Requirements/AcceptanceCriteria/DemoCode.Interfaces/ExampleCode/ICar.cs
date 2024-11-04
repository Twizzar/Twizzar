namespace DemoCode.Interfaces.ExampleCode
{
    public interface ICar : IVehicle
    {
        int Seats { get; set; }

        int GetPs(int a);
    }
}
