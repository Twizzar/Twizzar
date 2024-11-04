using Twizzar.Fixture;
using DemoCode.ExampleCode;
using DemoCode.Car;
using DemoCode.Interfaces;

namespace PlaygroundTests
{
    partial class UIPlayground
    {
        private class ClassWithInternalDependeciese472Builder : ItemBuilder<DemoCode.ExampleCode.ClassWithInternalDependecies, ClassWithInternalDependeciese472BuilderPaths>
        {
            public ClassWithInternalDependeciese472Builder()
            {
                this.With(p => p.MyInternalInt.Value(5));
            }
        }

        private class ClassWithInternalDependeciesda52Builder : ItemBuilder<DemoCode.ExampleCode.ClassWithInternalDependecies, ClassWithInternalDependeciesda52BuilderPaths>
        {
            public ClassWithInternalDependeciesda52Builder()
            {
                this.With(p => p.MyInternalInt.Value(5));
            }
        }

        private class ClassWithInternalDependeciesbe29Builder : ItemBuilder<DemoCode.ExampleCode.ClassWithInternalDependecies, ClassWithInternalDependeciesbe29BuilderPaths>
        {
            public ClassWithInternalDependeciesbe29Builder()
            {
                this.With(p => p.MyInternalInt.Value(5));
            }
        }

        private class ClassWithInternalDependeciesa6b0Builder : ItemBuilder<DemoCode.ExampleCode.ClassWithInternalDependecies, ClassWithInternalDependeciesa6b0BuilderPaths>
        {
            public ClassWithInternalDependeciesa6b0Builder()
            {
                this.With(p => p.MyInternalInt.Value(5));
            }
        }

        private class ClassWithInternalDependeciesdacaBuilder : ItemBuilder<DemoCode.ExampleCode.ClassWithInternalDependecies, ClassWithInternalDependeciesdacaBuilderPaths>
        {
            public ClassWithInternalDependeciesdacaBuilder()
            {
                this.With(p => p.MyInternalInt.Value(53));
            }
        }

        private class Carb71eBuilder : ItemBuilder<DemoCode.Car.Car, Carb71eBuilderPaths>
        {
            public Carb71eBuilder()
            {
                this.With(p => p.Ctor.wheelCount.Value(5));
            }
        }

        private class Car1d6fBuilder : ItemBuilder<DemoCode.Car.Car, Car1d6fBuilderPaths>
        {
            public Car1d6fBuilder()
            {
                this.With(p => p.FloatProp.Value(3f));
                this.With(p => p.DoubleProp.Value(2.0000d));
                this.With(p => p.DecimalProp.Value(2.01m));
                this.With(p => p.Ctor.engine.Stub<IEngine>());
                this.With(p => p.IntPropSetByCtor.Value(5));
            }
        }
    }
}