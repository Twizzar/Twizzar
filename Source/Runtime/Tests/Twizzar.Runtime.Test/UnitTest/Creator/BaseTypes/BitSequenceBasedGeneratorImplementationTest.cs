using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators.BitSequenceBased;

namespace Twizzar.Runtime.Test.UnitTest.Creator.BaseTypes
{
    [TestClass]
    public class BitSequenceBasedGeneratorImplementationTest
    {
        [TestMethod]
        public void LongGenerator_Get1MioElements_NoDuplicates()
        {
            var values = new HashSet<long>();
            for (var i = 0; i < 1_000_000; i++)
            {
                var sut = new LongUniqueCreator();
                var value = sut.GetNextValue();
                if (!values.Add(value))
                {
                    Assert.Fail($"Duplicate value {i} {value}");
                }
            }
        }

        [TestMethod]
        public void UlongGenerator_Get1MioElements_NoDuplicates()
        {
            var values = new HashSet<ulong>();
            for (var i = 0; i < 1_000_000; i++)
            {
                var sut = new UlongUniqueCreator();
                var value = sut.GetNextValue();
                if (!values.Add(value))
                {
                    Assert.Fail($"Duplicate value {i} {value}");
                }
            }
        }

        [TestMethod]
        public void IntGenerator_Get1MioElements_NoDuplicates()
        {
            var values = new HashSet<int>();
            for (var i = 0; i < 1_000_000; i++)
            {
                if (i == 100)
                {
                    Console.WriteLine("BreakPoint");
                }

                var value = new IntUniqueCreator().GetNextValue();
                if (!values.Add(value))
                {
                    Assert.Fail($"Duplicate value {i} {value}");
                }
            }
        }

        [TestMethod]
        public void UintGenerator_Get1MioElements_NoDuplicates()
        {
            var values = new HashSet<uint>();
            for (var i = 0; i < 1_000_000; i++)
            {
                var value = new UintUniqueCreator().GetNextValue();
                if (!values.Add(value))
                {
                    Assert.Fail($"Duplicate value {i} {value}");
                }
            }
        }

        [TestMethod]
        public void ShortGenerator_GenerateAllValues_NoDuplicates()
        {
            var values = new HashSet<short>();
            for (var i = 0; i < ushort.MaxValue - 2; i++)
            {
                var value = new ShortUniqueCreator().GetNextValue();
                if (!values.Add(value))
                {
                    Assert.Fail($"Duplicate value {i} {value}");
                }
            }
        }

        [TestMethod]
        public void UshortGenerator_GenerateAllValues_NoDuplicates()
        {
            var values = new HashSet<ushort>();
            for (var i = 0; i < ushort.MaxValue - 2; i++)
            {
                var value = new UshortUniqueCreator().GetNextValue();
                if (!values.Add(value))
                {
                    Assert.Fail($"Duplicate value {i} {value}");
                }
            }
        }

        [TestMethod]
        public void ByteGenerator_GenerateAllValues_NoDuplicates()
        {
            var values = new HashSet<byte>();
            for (var i = 0; i < byte.MaxValue; i++)
            {
                var value = new ByteUniqueCreator().GetNextValue();
                if (!values.Add(value))
                {
                    Assert.Fail($"Duplicate value {i} {value}");
                }
            }
        }

        [TestMethod]
        public void SbyteGenerator_GenerateAllValues_NoDuplicates()
        {
            var values = new HashSet<sbyte>();
            for (var i = 0; i < byte.MaxValue; i++)
            {
                var value = new SbyteUniqueCreator().GetNextValue();
                if (!values.Add(value))
                {
                    Assert.Fail($"Duplicate value {i} {value}");
                }
            }
        }

        [TestMethod]
        public void CharGenerator_GenerateAllValues_NoDuplicates()
        {
            var values = new HashSet<char>();
            for (var i = 0; i < ushort.MaxValue - 1; i++)
            {
                var value = new CharUniqueCreator().GetNextValue();
                if (!values.Add(value))
                {
                    Assert.Fail($"Duplicate value {i} {value}");
                }
            }
        }
    }
}
