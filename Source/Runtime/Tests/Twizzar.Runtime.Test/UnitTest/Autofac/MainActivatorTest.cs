using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Fixture;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.Services;
using Twizzar.Runtime.Infrastructure.AutofacServices.Activator;
using Twizzar.TestCommon;
using ViCommon.Functional.Monads.MaybeMonad;


namespace Twizzar.Runtime.Test.UnitTest.Autofac
{
    [TestClass]
    [TestCategory("Autofac")]
    public class MainActivatorTest
    {
        [TestMethod]
        public void MainActivator_CtorWithoutDefinition_ArgumentNullException()
        {
            Verify.Ctor<MainActivator>()
                .IgnoreParameter("type", typeof(MainActivator))
                .SetupParameter("definitionId", Maybe.None<string>())
                .ShouldThrowArgumentNullException();
        }

        [TestMethod]
        [DataRow(typeof(int), null)]
        [DataRow(typeof(string), null)]
        [DataRow(typeof(MainActivator), null)]
        [DataRow(typeof(int), "someDefinition")]
        [DataRow(typeof(string), "someDefinition")]
        [DataRow(typeof(MainActivator), "someDefinition")]
        public void MainActivator_CtorWithoutDefinition_NotNull(Type type, string definitionId)
        {
            // arrange
            var fixtureDefinitionService = new Mock<IFixtureItemDefinitionQuery>().Object;
            var fixtureFinder = new Mock<ICreatorProvider>().Object;

            // act
            var sut = new MainActivator(
                type, 
                Maybe.ToMaybe(definitionId), 
                fixtureDefinitionService, 
                fixtureFinder,
                Build.New<IRegisteredCodeInstanceContainer>());

            // assert
            Assert.IsNotNull(sut);
        }
    }
}
