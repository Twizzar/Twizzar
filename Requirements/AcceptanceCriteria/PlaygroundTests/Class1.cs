using DemoCode.ExampleCode;
using DemoCode.Interfaces.ExampleCode;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Twizzar.Fixture;

namespace PlaygroundTests
{
    public interface MyList<T> where T : struct
    {

    }

    public interface ITest
    {
        T GenericMethod<T>(T a);

        T GenericStructMethod<T>(T a) where T : struct;
        MyList<T> GenericNestedStructMethod<T>(MyList<T> a) where T : struct;

        MyList<ValueTuple<T, K>> GenericComplexNestedMethod<T, K>(IEnumerable<T> a) where T : IVehicle;
        //MyList<T> GenericNestedStructMethod<T>() where T : struct;
    }

    public partial class Class1
    {
        [Test]
        public void TT()
        {
            var sut = new ItemBuilder<ITest>()
                .Build();
        }

        [Test]
        public void TT2()
        {
            var sut = new ItemBuilder<ITest>()
                .With(p => p.GenericMethodT__T.Stub<ICar>())
                .Build();

            var r = sut.GenericMethod<ICar>(new Car());
            Assert.That(r, Is.AssignableTo<ICar>());
        }

        [Test]
        public void Test3()
        {
            var myList = new ItemBuilder<MyList<int>>().Build(); 

            var sut = new ItemBuilder<ITest>()
                .With(p => p.GenericNestedStructMethodT.Value(myList))
                .Build();

            var r = sut.GenericNestedStructMethod<int>(myList);
            Assert.That(r, Is.AssignableTo<MyList<int>>());
        }


        [Test]
        public void Test4()
        {
            var myList = new ItemBuilder<MyList<ValueTuple<Car, float>>>().Build();

            var sut = new ItemBuilder<ITest>()
                .With(p => p.GenericComplexNestedMethodTK.Value(myList))
                .Build();

            var r = sut.GenericComplexNestedMethod<Car, float>(null);
            Assert.That(r, Is.EqualTo(myList));

            var r2 = sut.GenericComplexNestedMethod<IVehicle, float>(null);
            Assert.That(r, Is.EqualTo(myList));
        }
    }
}
