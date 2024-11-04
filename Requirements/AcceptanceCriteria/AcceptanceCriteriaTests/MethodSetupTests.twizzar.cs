using DemoCode.ExampleCode;

using Twizzar.Fixture;

namespace AcceptanceCriteriaTests
{
    public partial class MethodSetupTests
    {
        private class SimpleStringMethodBuilder : ItemBuilder<IMethodTest, SimpleStringMethodPaths>
        {
            public SimpleStringMethodBuilder()
            {
                this.With(p => p.MethodWithOverloads__String.Value("Einen Test"));
            }
        }

        private class FastAfCarBuilder : ItemBuilder<Car, FastAfCarPaths>
        {
            public FastAfCarBuilder()
            {
                this.With(p => p.Seats.Value(2));
                this.With(p => p.Speed.Value(350));
            }
        }
    }
}