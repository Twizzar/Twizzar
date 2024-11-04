using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Fixture;
using Twizzar.Runtime.Infrastructure.DomainService;

namespace Twizzar.Runtime.Test.UnitTest.Autofac
{
    [TestClass]
    [TestCategory("Autofac")]
    public class ConcreteTypeCreatorTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConcreteTypeCreator_CreateInstanceWithInvalidArgument_ArgumentNullException()
        {
            // arrange
            var creator = new ItemBuilder<ConcreteTypeCreator>()
                .Build();

            // act
            creator.CreateInstance(null);
        }
    }
}
