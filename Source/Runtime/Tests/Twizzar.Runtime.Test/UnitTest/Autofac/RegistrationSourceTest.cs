using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Runtime.Infrastructure.AutofacServices.Factory;
using Twizzar.Runtime.Infrastructure.AutofacServices.Registration;

namespace Twizzar.Runtime.Test.UnitTest.Autofac
{
    [TestClass]
    [TestCategory("Autofac")]
    public class RegistrationSourceTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegistrationSource_ctor_ArgumentNullException()
        {
            // act
            var sut = new RegistrationSource(null);

            // assert
            Assert.Fail(sut.ToString());
        }

        [TestMethod]
        public void RegistrationSource_ctor_NotNull()
        {
            // arrange
            var doc = new Mock<IComponentRegistrationFactory>().Object;

            // act
            var sut = new RegistrationSource(doc);

            // assert
            Assert.IsNotNull(sut);
            Assert.IsFalse(sut.IsAdapterForIndividualComponents);
        }


        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void RegistrationSource_RegistrationsFor_ArgumentNullException()
        {
            // arrange
            var doc = new Mock<IComponentRegistrationFactory>().Object;

            // act
            var sut = new RegistrationSource(doc);
            sut.RegistrationsFor(null, null);
        }

        [TestMethod]
        public void RegistrationSource_RegistrationsFor_AlreadyRegistered()
        {
            // arrange
            var doc = new Mock<IComponentRegistrationFactory>().Object;
            var autofacService = new Mock<Service>().Object;
            var func = new Mock<Func<Service, IEnumerable<IComponentRegistration>>>(MockBehavior.Strict);
            var alreadyRegistered = new Mock<IComponentRegistration>().Object;
            var expectedResult = new List<IComponentRegistration> {alreadyRegistered};
            func
                .Setup(f => f.Invoke(autofacService))
                .Returns(expectedResult);

            // act
            var sut = new RegistrationSource(doc);
            var registration = sut.RegistrationsFor(autofacService, func.Object);

            // assert
            Assert.AreSame(expectedResult, registration);
        }


        [TestMethod]
        public void RegistrationSource_RegistrationsFor_NotIServiceWithType()
        {
            // arrange
            var doc = new Mock<IComponentRegistrationFactory>().Object;
            var autofacService = new UniqueService();

            var func = new Mock<Func<Service, IEnumerable<IComponentRegistration>>>();
            
            func
                .Setup(f => f.Invoke(autofacService))
                .Returns(Enumerable.Empty<IComponentRegistration>());

            // act
            var sut = new RegistrationSource(doc);
            var registration = sut.RegistrationsFor(autofacService, func.Object);

            // assert
            Assert.IsFalse(registration.Any());
        }

        [TestMethod]
        public void RegistrationSource_RegistrationsFor_AutofacType()
        {
            // arrange
            var doc = new Mock<IComponentRegistrationFactory>().Object;
            var autofacService = new TypedService(typeof(ContainerBuilder));

            var func = new Mock<Func<Service, IEnumerable<IComponentRegistration>>>();
            func.Setup(f => f.Invoke(autofacService))
                .Returns(Enumerable.Empty<IComponentRegistration>());

            // act
            var sut = new RegistrationSource(doc);
            var registration = sut.RegistrationsFor(autofacService, func.Object);

            // assert
            Assert.IsFalse(registration.Any());
        }

        [TestMethod]
        [DataRow(typeof(int), null)]
        [DataRow(typeof(string), null)]
        [DataRow(typeof(bool), null)]
        [DataRow(typeof(int), "randomScope")]
        [DataRow(typeof(string), "randomScope")]
        [DataRow(typeof(bool), "randomScope")]
        public void RegistrationSource_RegistrationsFor_Successfully(Type type, string scope)
        {
            // arrange
            Service autofacService = new TypedService(type);
            if (scope != null)
            {
                autofacService = new KeyedService(scope, type);
            }

            var registrationFactory = new Mock<IComponentRegistrationFactory>(MockBehavior.Strict);

            var expectedResult = new[] { new Mock<IComponentRegistration>().Object };

            var func = new Mock<Func<Service, IEnumerable<IComponentRegistration>>>();
            func.Setup(f => f.Invoke(autofacService))
                .Returns(Enumerable.Empty<IComponentRegistration>());

            registrationFactory
                .Setup(f => f.Create(autofacService, type, scope))
                .Returns(expectedResult);

            // act
            var sut = new RegistrationSource(registrationFactory.Object);
            var registration = sut.RegistrationsFor(autofacService, func.Object);

            // assert
            Assert.IsTrue(registration.Any());
        }
    }
}
