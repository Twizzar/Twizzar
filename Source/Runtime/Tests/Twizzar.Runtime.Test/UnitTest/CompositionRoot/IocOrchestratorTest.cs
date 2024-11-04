using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.CompositionRoot;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.Test.UnitTest.CompositionRoot
{
    [TestClass]
    [TestCategory("Obsolete")]
    public class IocOrchestratorTest
    {
        [TestMethod()]
        public void Calling_resolve_without_Register_throws_an_exception()
        {
            // arrange
            var sut = new IocOrchestrator();

            // act
            Action action = () => sut.Resolve<int>();

            // assert
            action.Should().Throw<Exception>();
        }

        [TestMethod()]
        public void TestDispose()
        {
            // arrange
            var sut = new IocOrchestrator();
            
            // act
            sut.Register(Maybe.Some(Mock.Of<IItemConfig<int>>()));
            sut.Dispose();
            
            Action action = () => sut.Resolve<int>();

            action.Should().Throw<ObjectDisposedException>();
        }
    }
}