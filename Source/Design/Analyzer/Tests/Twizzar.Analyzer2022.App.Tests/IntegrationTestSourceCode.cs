using System;
using System.Collections;
using Twizzar.Fixture;
using Twizzar.Fixture.Member;
using System.Collections.Generic;

namespace Twizzar.SourceGenerator.Playground
{
    public class A
    {
        public int IntProp { get; set; }
        public A AProp { get; set; }
        public B BProp { get; }
    }

    public class B
    {
        public B(Class2<string> class2) { }

        public int IntProp { get; }
    }


    public class MyList<T> where T : struct { }

    public interface IA
    {
        public void AVoidMethod();
        public int AIntMethod();

        public int MethodTest(int a, float b);

        public T GenericMethodTest<T, K>(MyList<T>[] a, float b) where T : struct, IA, IList where K : class, IList;
        //public T GenericMethodTest2<T>(T a, float b);
    }

    public class Class2<T>
    {
        public A AProp { get; set; }

        public Class2(IA ia)
        {
        }
    }

    public class MyBuilder : ItemBuilder<Class2<int>, MyAPaths>
    {
        public MyBuilder()
        {
            With(p => p.Ctor.ia.Value(null));
            With(p => p.AProp.IntProp.Value(5));
            With(p => p.AProp.AProp.IntProp.Value(6));
            With(p => p.AProp.AProp.AProp.AProp.AProp.IntProp.Value(6));
        }
    }

    public class Test
    {
        public void Bla()
        {
            new ItemBuilder<IA>();

            new MyBuilder()
                .Build(out var context);

            context.Verify(p => p.Ctor.ia.MethodTest)
                .Called();

            //new ItemBuilder<B>();

            //new ItemBuilder<Dictionary<Int32, String>>();

            //new ItemBuilder<A>()
            //    .With(p => p.BProp.IntProp.Value(3));

            //new ItemBuilder<Class2<int>>()
            //    .With(p => p.Ctor.ia.InstanceOf<A>());
        }
    }
}