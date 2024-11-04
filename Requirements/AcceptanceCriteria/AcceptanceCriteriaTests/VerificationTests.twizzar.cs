using Twizzar.Fixture;
using DemoCode.ExampleCode;
using DemoCode.Car;

namespace AcceptanceCriteriaTests
{
    partial class VerificationTests
    {
        private class IMethodTest85adBuilder : ItemBuilder<DemoCode.ExampleCode.IMethodTest, IMethodTest85adBuilderPaths>
        {
            public IMethodTest85adBuilder()
            {
                this.With(p => p.MethodWithOverloads__String.Value("test"));
            }
        }

        private class IMethodTestBuilder : ItemBuilder<DemoCode.ExampleCode.IMethodTest, IMethodTestcc03BuilderPaths>
        {
        }

        private class IPassengerBuilder : ItemBuilder<DemoCode.Car.IPassenger, IPassenger7e0eBuilderPaths>
        {
        }
    }
}