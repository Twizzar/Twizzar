using DemoCode.Interfaces.ExampleCode;

namespace DemoCode.ExampleCode
{
    public class Car : ICar
    {
        #region Implementation of IVehicle

        /// <inheritdoc />
        public float Speed { get; set; }

        public int Seats { get; set; }

        #endregion

        public override bool Equals(object obj) => 
            obj is Car c && c.Speed == this.Speed && c.Seats == this.Seats;

        public int GetPs(int a)
        {
            throw new System.NotImplementedException();
        }
    }
}
