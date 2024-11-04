using System;
using System.Linq;
using Autofac.Core;
using Autofac.Core.Registration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.CompositionRoot.Factory;
using Twizzar.Runtime.Infrastructure.AutofacServices.Factory;

namespace Twizzar.Runtime.Test.UnitTest.Autofac
{
    [TestClass]
    [TestCategory("Autofac")]
    public class ComponentRegistrationFactoryTest
    {
        private static ComponentRegistrationFactory.FactoryDelegate FactoryDelegate =>
            (id, activator, lifetime, sharing, ownership, services, metadata) =>
                new ComponentRegistration(id, activator, lifetime, sharing, ownership, services, metadata);
            

        [TestMethod]
        public void ComponentRegistrationFactory_ctor_NotNull()
        {
            // act
            var sut = new ComponentRegistrationFactory(null,
                FactoryDelegate,
                new Mock<IMainActivatorFactory>().Object
                );

            // assert
            Assert.IsNotNull(sut);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ComponentRegistrationFactory_ctor_ArgumentNullException()
        {
            // act
            var sut = new ComponentRegistrationFactory(null, null, null);

            // assert
            Assert.Fail(sut.ToString());
        }

        [TestMethod]
        [DataRow(null, null)]
        [DataRow("dummyService", null)]
        [DataRow(null, typeof(int))]
        public void ComponentRegistrationFactory_Create_ArgumentNullException(string serviceConfig, Type type)
        {
            // arrange
            var activatorFactory = new Mock<IMainActivatorFactory>().Object;
            Service autofacService = null;

            if (serviceConfig != null)
            {
                autofacService = new Mock<Service>().Object;
            }

            // act
            var sut = new ComponentRegistrationFactory(null, FactoryDelegate, activatorFactory);
            Action action = () => sut.Create(autofacService, type);
            action.Should().Throw<AggregateException>();
        }

        [TestMethod]
        [DataRow(typeof(int), null)]
        [DataRow(typeof(string), null)]
        [DataRow(typeof(bool), null)]
        [DataRow(typeof(int), "randomScope")]
        [DataRow(typeof(string), "randomScope")]
        [DataRow(typeof(bool), "randomScope")]
        public void ComponentRegistrationFactory_Create_Successfully(Type type, string scope)
        {
            // arrange
            var activatorFactory = new Mock<IMainActivatorFactory>(MockBehavior.Strict);
            var activator = new Mock<IInstanceActivator>();

            activatorFactory
                .Setup(f => f.Create(type, scope))
                .Returns(activator.Object);

            var service = new Mock<Service>().Object;

            // act
            var sut = new ComponentRegistrationFactory(null, FactoryDelegate, activatorFactory.Object);
            var result = sut.Create(service, type, scope);

            // assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length == 1);
            Assert.IsInstanceOfType(result.First(), typeof(ComponentRegistration));
        }
    }
}
