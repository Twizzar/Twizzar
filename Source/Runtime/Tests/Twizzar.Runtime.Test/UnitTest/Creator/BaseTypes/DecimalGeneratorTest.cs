using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators;

namespace Twizzar.Runtime.Test.UnitTest.Creator.BaseTypes
{
    [TestClass]
    public class DecimalGeneratorTest
    {

        [TestMethod]
        public void DecimalGenerator_TestSome_NoDuplicates()
        {
            var values = new HashSet<decimal>();
            for (var i = 0; i < 1_000_000; i++)
            {
                var value = new DecimalUniqueCreator().GetNextValue();
                var decimalValue = (decimal)Convert.ChangeType(value, typeof(decimal));

                if (!values.Add(decimalValue))
                {
                    Assert.Fail($"Duplicate value {i} {decimalValue }");
                }
            }

        }
    }
}