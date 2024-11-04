using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators.FloatingPointNumbers;

namespace Twizzar.Runtime.Test.UnitTest.Creator.BaseTypes
{
    [TestClass]
    [TestCategory("FloatingPointTypesGenerators")]
    public class FloatingPointTypesGeneratorImplementationTest
    {
        [TestMethod]
        public void FloatGenerator_Generate1MioElements_NoDuplicates()
        {
            var values = new HashSet<float>();

            for (var i = 0; i < 1_000_000; i++)
            {

                var value = new FloatUniqueCreator().GetNextValue();
                var floatValue = (float)Convert.ChangeType(value, typeof(float));

                if (!values.Add(floatValue))
                {
                    Assert.Fail($"Duplicate value {i} {floatValue}");
                }
            }
        }

        [TestMethod]
        public void DoubleGenerator_Generate1MioElements_NoDuplicates()
        {
            var values = new HashSet<double>();
            for (var i = 0; i < 1_000_000; i++)
            {
                if (i == 1000)
                {
                    Console.WriteLine("get break point");
                }

                var value = new DoubleUniqueCreator().GetNextValue();
                var doubleValue = (double)Convert.ChangeType(value, typeof(double));

                if (!values.Add(doubleValue))
                {
                    Assert.Fail($"Duplicate value {i} {doubleValue}");
                }
            }
        }
    }
}
