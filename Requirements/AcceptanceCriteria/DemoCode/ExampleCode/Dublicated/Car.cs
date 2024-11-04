using DemoCode.Interfaces.ExampleCode;

namespace DemoCode.ExampleCode.Dublicated
{
    public class Car : ICar
    {
        public string ThisISADublicate { get; set; }
        public int Seats { get; set; }
        public float Speed { get; set; }

        public int GetPs(int a)
        {
            throw new System.NotImplementedException();
        }
    }
}
