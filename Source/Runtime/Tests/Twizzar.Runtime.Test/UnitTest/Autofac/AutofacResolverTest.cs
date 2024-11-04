using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Runtime.Infrastructure.AutofacServices.Resolver;
using Twizzar.SharedKernel.Infrastructure.Factory;

namespace Twizzar.Runtime.Test.UnitTest.Autofac
{
    [TestClass]
    [TestCategory("Obsolete")]
    public class AutofacResolverTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AutofacResolver_CtorRegistrationSourceIsNull_ThrowArgumentNullException()
        {
            // arrange 
            new AutofacResolverAdapter(null);

            // assert
            Assert.Fail("Should throw ArgumentNullException");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AutofacResolver_ResolveNamedServiceNameIsNull_ThrowArgumentNullException()
        {
            // arrange 
            var resolver = new AutofacResolverAdapter(new Mock<IAutofacContainerFactory>().Object);

            //act
            resolver.ResolveNamed<int>(null);

            // assert
            Assert.Fail("Should throw ArgumentNullException");
        }
    }
}

