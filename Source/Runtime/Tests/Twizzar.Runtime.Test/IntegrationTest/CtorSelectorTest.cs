using System;
using System.Linq;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Twizzar.Runtime.Infrastructure.ApplicationService;
using Twizzar.Runtime.Test.Builder;
using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.NLog.Logging;

namespace Twizzar.Runtime.Test.IntegrationTest
{
    [TestClass]
    [TestCategory("IntegrationTest")]
    public class CtorSelectorTest
    {
        private readonly SharedKernel.Core.FixtureItem.Configuration.Services.CtorSelector _sut = new SharedKernel.Core.FixtureItem.Configuration.Services.CtorSelector();

        [TestInitialize]
        public void Initialize()
        {
            LoggerFactory.SetConfig(new LoggerConfigurationBuilder());
        }

        [TestMethod]
        public void CtorSelector_GetCtorDescription_Successful()
        {
            // arrange
            var reflectionDescriptionProvider = new ReflectionTypeDescriptionProvider(
                new ReflectionDescriptionFactoryBuilder().Build());
            var typeDescription = reflectionDescriptionProvider.GetTypeDescription(typeof(TestClass));
            var numberParams = typeof(TestClass).GetConstructors().Max(ctor => ctor.GetParameters().Length);

            // act
            var res = this._sut.GetCtorDescription(typeDescription, CtorSelectionBehavior.Max);
            var res2 = this._sut.GetCtorDescription(typeDescription, CtorSelectionBehavior.Max);

            // assert
            Assert.IsNotNull(res);
            var methodDescription = res.GetSuccessUnsafe();
            var methodDescription2 = res2.GetSuccessUnsafe();
            Assert.AreEqual(numberParams, methodDescription.DeclaredParameters.Count());
            Assert.AreEqual(methodDescription.DeclaredParameters, methodDescription2.DeclaredParameters);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CtorSelector_GetCtorDescription_ArgumentNullException()
        {
            // arrange
            var sut = this._sut;

            // act
            sut.GetCtorDescription(null, CtorSelectionBehavior.Max);
        }

        [TestMethod]
        public void CtorSelector_GetCtorDescription_InvalidTypeDescriptionException()
        {
            // arrange
            LoggerFactory.SetConfig(new LoggerConfigurationBuilder());

            var reflectionDescriptionProvider = new ReflectionTypeDescriptionProvider(
                new ReflectionDescriptionFactoryBuilder().Build());
            var typeDescription = reflectionDescriptionProvider.GetTypeDescription(typeof(ITestInterface));

            // act
            var result = this._sut.GetCtorDescription(typeDescription, CtorSelectionBehavior.Max);

            result.IsFailure.Should().BeTrue();
            result.GetFailureUnsafe().Should().BeOfType<InvalidTypeDescriptionFailure>();
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        [DataRow(CtorSelectionBehavior.Custom)]
        public void CtorSelector_GetCtorDescription_NotImplementedException(CtorSelectionBehavior behavior)
        {
            // arrange
            LoggerFactory.SetConfig(new LoggerConfigurationBuilder());

            var reflectionDescriptionProvider = new ReflectionTypeDescriptionProvider(
                new ReflectionDescriptionFactoryBuilder().Build());
            var typeDescription = reflectionDescriptionProvider.GetTypeDescription(typeof(TestClass));


            // act
            this._sut.GetCtorDescription(typeDescription, behavior);
        }
    }

    public interface ITestInterface
    {
    }
}
