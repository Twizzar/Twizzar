using DemoCode.ExampleCode;

using NUnit.Framework;
using PlaygroundTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twizzar.Fixture;

namespace PlaygroundTests
{
    public interface IMyMethodTest
    {
        int FirstMethod(int a);
        int SecondMethod(int b, float c);

        IMethodTest GetMethodTest();

        int Value2 { get; set;}
    }

    public partial class TestMethod
    {
        [Test]
        public void T1()
        {
            var sut = new IMyMethodTest0fbcBuilder()
                .Build(out var context);

            context.Verify(p => p.FirstMethod__Int32)
                .WhereAIs(3)
                .CalledAtLeastOnce();

            context.Verify(p => p.SecondMethod)
                .WhereBIs(3)
                .WhereCIs(2f)
                .CalledAtLeastOnce();

            context.Verify(p => p.GetMethodTest.SomeComplicatedMethod__Dictionary2_Single_String)
                .WhereParam2Is(3)
                .CalledAtLeastOnce();
        }
    }
}
