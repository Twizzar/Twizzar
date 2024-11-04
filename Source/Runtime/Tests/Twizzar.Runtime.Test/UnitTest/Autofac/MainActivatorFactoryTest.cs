using System;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.CompositionRoot.Factory;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.Services;
using Twizzar.Runtime.Infrastructure.AutofacServices.Activator;
using Twizzar.TestCommon;

namespace Twizzar.Runtime.Test.UnitTest.Autofac
{
    [TestClass]
    [TestCategory("Autofac")]
    public class MainActivatorFactoryTest
    {
        [TestMethod]
        public void MainActivatorFactory_ctor_NotNull()
        {
            // arrange
            var definitionFactory = new Mock<IFixtureItemDefinitionQuery>().Object;
            var creatorProvider = new Mock<ICreatorProvider>().Object;

            // act
            var sut = new MainActivatorFactory((type, id) => 
                new MainActivator(type, id, definitionFactory, creatorProvider, Build.New<IRegisteredCodeInstanceContainer>()));

            // assert
            Assert.IsNotNull(sut);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MainActivatorFactory_Create_ArgumentNullException()
        {
            // arrange
            var factory = new Mock<IFixtureItemDefinitionQuery>().Object;
            var creatorProvider = new Mock<ICreatorProvider>().Object;
            var builder = new ContainerBuilder();
            builder.RegisterInstance(factory).As<IFixtureItemDefinitionQuery>();
            builder.RegisterInstance(creatorProvider).As<ICreatorProvider>();

            // act
            var sut = new MainActivatorFactory((type, id) => new MainActivator(type, id, factory, creatorProvider, Build.New<IRegisteredCodeInstanceContainer>()));
            sut.Create(null, null);

            // assert
            Assert.IsNotNull(sut);
        }

        [TestMethod]
        [DataRow(typeof(int), null)]
        [DataRow(typeof(string), null)]
        [DataRow(typeof(bool), null)]
        [DataRow(typeof(int), "randomScope")]
        [DataRow(typeof(string), "randomScope")]
        [DataRow(typeof(bool), "randomScope")]
        public void MainActivatorFactory_Create_Successfully(Type type, string scope)
        {
            // arrange
            var definitionFactory = new Mock<IFixtureItemDefinitionQuery>().Object;
            var creatorProvider = new Mock<ICreatorProvider>().Object;
            var builder = new ContainerBuilder();

            builder.RegisterInstance(definitionFactory).As<IFixtureItemDefinitionQuery>();
            builder.RegisterInstance(creatorProvider).As<ICreatorProvider>();

            // act
            var sut = new MainActivatorFactory((t, id) =>
                new MainActivator(t, id, definitionFactory, creatorProvider, Build.New<IRegisteredCodeInstanceContainer>()));
            var result = sut.Create(type, scope);

            // assert
            Assert.IsInstanceOfType(result, typeof(MainActivator));
            Assert.AreEqual(type, result.LimitType);
        }
    }
}
