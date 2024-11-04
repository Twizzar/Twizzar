using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Runtime.Infrastructure.ApplicationService;
using Twizzar.Runtime.Test.Builder;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Runtime.Test.UnitTest.FixtureItem.Description
{
    [TestClass]
    public class GenericTests
    {
        [TestMethod]
        [DataRow(typeof(int?))]
        [DataRow(typeof(IEnumerable<string>))]
        [DataRow(typeof(IResult<string, Failure>))]
        [DataRow(typeof(Tuple<string, string, Tuple<int, int>>))]
        public void Test(Type type)
        {
            var factory = new ReflectionDescriptionFactoryBuilder().Build();
            var typeDescriptionQuery = new ReflectionTypeDescriptionProvider(factory);

            var description = typeDescriptionQuery.GetTypeDescription(type);

            //TestCop				
            Console.WriteLine(type.FullName + " => " + description.TypeFullName);

            description.TypeFullName.FullName.Should().Be(type.FullName);
        }
    }
}

