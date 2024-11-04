using DemoCode.Interfaces.ExampleCode;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoCode.ExampleCode
{
    public interface IGenericMethods
    {
        int SimpleGenericMethod();
        T SimpleGenericMethod<T>();
        T SimpleGenericMethod<T>(T paramA);

        IList<T> CreateNewList<T>(params T[] items);

        T StructGeneric<T>(T paramA) where T : struct;

        T ConstrainGeneric<T>(T paramA) where T : IVehicle;

        IList<IList<T>> ConstrainNestedGeneric<T>(IList<T> paramA) where T : IVehicle;

        IList<Tuple<T, K>> ComplicatedGeneric<T, K>(Task<IList<K>> task) where K : struct;

        T MoreThanOneParam<T, K>((T,K) a, T b, int c);
    }
}
