using System;
using System.Data;
using Autofac;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.CompositionRoot.Factory;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;

namespace Twizzar.Runtime.Test.UnitTest.Creator
{
    [TestClass]
    [TestCategory("Obsolete")]
    public class UniqueCreatorProviderTests
    {
        [TestMethod]
        public void GetUniqueCreator_throws_InvalidConstraintException_when_type_is_not_baseType()
        {
            // arrange
            var baseTypeService = new Mock<IBaseTypeService>();
            baseTypeService
                .Setup(service => service.IsBaseType(It.IsAny<TypeFullName>()))
                .Returns(false);

            var lifetimeScope = new Mock<ILifetimeScope>();

            var sut = new UniqueCreatorProvider(baseTypeService.Object, lifetimeScope.Object);
            
            //act
            Action action = () => sut.GetUniqueCreator<UniqueCreatorProviderTests>();

            action.Should().Throw<InvalidConstraintException>();
        }
    }
}