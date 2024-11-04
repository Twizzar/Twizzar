using System.Collections.Generic;
using System.Reflection;

using DemoCode.Interfaces.ExampleCode;

namespace DemoCode.ExampleCode
{
    public interface IMethodTest
    {
        void VoidMethod();

        int MethodWithOverloads();
        int MethodWithOverloads(int a);
        string MethodWithOverloads(string a);

        (int, string) SomeComplicatedMethod(Dictionary<string, (int, List<string>, (int, float))> param1, float param2, string param3);

        MyEnum MethodEnum();
        IVehicle GetMyVehicle(string name);

        T GenericTest<T>();
    }

    public interface IGenericClassMethodTest<T>
    {
        T GenericMethod();
    }
}
