using System;
using DemoCode.ExampleCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Fixture;

namespace Net48Tests
{
    [TestClass]
    public partial class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var a = new ItemBuilder<Car>();
        }
    }
}
