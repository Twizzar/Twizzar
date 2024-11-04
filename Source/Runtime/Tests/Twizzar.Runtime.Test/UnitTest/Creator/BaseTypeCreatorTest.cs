using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Runtime.Core.FixtureItem.Creators;
using Twizzar.Runtime.CoreInterfaces.Exceptions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;

namespace Twizzar.Runtime.Test.UnitTest.Creator
{
    [TestClass]
    [TestCategory("Creator")]
    public class BaseTypeCreatorTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BaseTypeCreator_CreateInstance_ArgumentNullException()
        {
            // act 
            var doc = new Mock<IBaseTypeUniqueCreator>();
            var sut = new BaseTypeCreator(doc.Object, Mock.Of<IInstanceCacheRegistrant>())
                .CreateInstance(null);

            // assert
            Assert.Fail(sut.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ResolveTypeException))]
        public void BaseTypeCreator_CreateInstance_InvalidCreatorInfoException()
        {
            // act 
            var doc = new Mock<IBaseTypeUniqueCreator>();
            var sut = new BaseTypeCreator(doc.Object, Mock.Of<IInstanceCacheRegistrant>())
                .CreateInstance(new Mock<IFixtureItemDefinitionNode>().Object);

            // assert
            Assert.Fail(sut.ToString());
        }
    }
}
