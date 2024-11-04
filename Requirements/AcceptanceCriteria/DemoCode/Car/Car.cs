using System;
using System.Collections.Generic;

namespace DemoCode.Car
{
    public class Car
    {
        public IReadOnlyList<IPassenger> Passengers { get; private set; }

        public bool HasRadio { get; }
        public string SerialNumber { get; }

        public char Model { get; }

        public byte WheelCount { get; }

        public IEngine Engine { get; set; }

        public int IntPropSetByCtor { get; set; }

        public float FloatProp { get; set; }

        public double DoubleProp { get; set; }

        public decimal DecimalProp { get; set; }

        public IEngine InterfacePropNotSetByCtor { get; set; }

        #region ctor

        public Car()
        {

        }

        public Car(string serialNumber)
        {
            /*if (serialNumber != "123")
            {
                throw new ArgumentNullException(nameof(serialNumber));
            }*/

            SerialNumber = serialNumber ?? throw new ArgumentNullException(nameof(serialNumber));
        }

        public Car(bool hasRadio)
        {
            HasRadio = hasRadio;
        }

        public Car(char model)
        {
            this.Model = model;
        }

        public Car(byte wheelCount)
        {
            this.WheelCount = wheelCount;
        }

        public Car(IEngine engine)
        {
            Engine = engine ?? throw new ArgumentNullException(nameof(engine));
        }

        public Car(byte wheelCount, char model, bool hasRadio, string serialNumber, IEngine engine)
        {
            this.WheelCount = wheelCount;
            this.Model = model;
            this.HasRadio = hasRadio;
            this.SerialNumber = serialNumber ?? throw new ArgumentNullException(nameof(serialNumber));
            this.Engine = engine ?? throw new ArgumentNullException(nameof(engine));

            // engine.Start();
            // engine.AnotherEngine.Start();
        }

        #endregion

        #region public Methods

        public void BoardPassengers(IEnumerable<IPassenger> passengers)
        {
            this.Passengers = new List<IPassenger>(passengers);
        }

        #endregion

    }
}