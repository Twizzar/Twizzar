using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Runtime.CoreInterfaces.Exceptions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using Twizzar.Runtime.Infrastructure.DomainService;

namespace Twizzar.Runtime.Test.UnitTest.Creator
{
    [TestClass]
    [TestCategory("Creator")]
    public class MoqCreatorTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MoqCreator_CreateInstance_ArgumentNullException()
        {
            // act 
            var sut = new MoqCreator(
                    Mock.Of<IInstanceCacheRegistrant>(),
                Mock.Of<IBaseTypeCreator>())
                .CreateInstance(null);

            // assert
            Assert.Fail(sut.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ResolveTypeException))]
        public void MoqCreator_CreateInstance_InvalidCreatorInfoException()
        {
            // act 
            var sut = new MoqCreator(
                    Mock.Of<IInstanceCacheRegistrant>(),
                    Mock.Of<IBaseTypeCreator>())
                .CreateInstance(
                    new Mock<IFixtureItemDefinitionNode>().Object);

            // assert
            Assert.Fail(sut.ToString());
        }
    }
}
