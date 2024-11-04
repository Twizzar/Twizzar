using System;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.CompositionRoot;
using Twizzar.Fixture;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Api.Tests.CompositionRoot
{
    [Category("TwizzarInternal")]
    public class IocOrchestratorTests
    {
        [Test]
        public void Calling_resolve_without_Register_throws_an_exception()
        {
            // arrange
            var sut = new ItemBuilder<IocOrchestrator>().Build();

            // act
            Action action = () => sut.Resolve<int>();

            // assert
            action.Should().Throw<Exception>();
        }

        [Test]
        public void TestDispose()
        {
            // arrange
            var sut = new ItemBuilder<IocOrchestrator>().Build();

            // act
            sut.Register(Maybe.Some(new ItemBuilder<IItemConfig<int>>().Build()));
            sut.Dispose();

            Action action = () => sut.Resolve<int>();

            action.Should().Throw<ObjectDisposedException>();
        }
    }
}