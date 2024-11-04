using DemoCode.ExampleCode;
using Twizzar.Fixture;

namespace DemoCode.Tests
{
    public partial class ApiTests
    {
        private class MyGarageBuilder : ItemBuilder<Garage, MyGaragePathsABC>
        {
            public MyGarageBuilder()
            {
                this.With(p => p.Vehicle1.InstanceOf<Bike>());
                this.With(p => p.Vehicle1.Speed.Value(42));
                this.With(p => p.Vehicle3.InstanceOf<EBike>());
                this.With(p => p.Vehicle3.mhw.Value(3));
            }
        }

        private class LongPathClassDBuilder : ItemBuilder<ClassD, LongPathClassDPaths>
        {
            public LongPathClassDBuilder()
            {
                this.With(p => p.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.A1.Value_.Value(42));
                this.With(p => p.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.D1.A1.Value_.Value(42));

                this.With(p => p.A1.Garage.Vehicle1.Speed.Value(3));
            }
        }
    }
}
