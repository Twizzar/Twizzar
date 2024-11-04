using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.TestCommon;

namespace Twizzar.Runtime.Test.IntegrationTest
{
    [TestClass]
    [TestCategory("IntegrationTest")]
    public class GenericTest
    {
        [TestMethod]
        public void Nullable_baseType_behave_like_a_baseType()
        {
            // act
            var instance1 = Build.New<int?>();
            var instance2 = Build.New<int?>();

            // assert
            instance1.HasValue.Should().BeTrue();
            instance2.HasValue.Should().BeTrue();
            instance1.Should().NotBe(instance2);
        }

        [TestMethod]
        [DataRow(typeof(IList<int>))]
        [DataRow(typeof(Tuple<int, string>))]
        [DataRow(typeof(IEnumerable<Tuple<int, string>>))]
        [DataRow(typeof(int?))]
        [DataRow(typeof(Nullable<int>))]
        public void Generic_types_get_instantiated(Type type)
        {
            // act
            var fixtureType = typeof(Build);
            var methodInfo = fixtureType.GetMethod(nameof(Build.New), new Type[]{});

            var genericMethodInfo = methodInfo.MakeGenericMethod(type);

            var instance = genericMethodInfo.Invoke(null, new object[] { });

            // assert
            instance.Should().NotBeNull();
        }
    }
}