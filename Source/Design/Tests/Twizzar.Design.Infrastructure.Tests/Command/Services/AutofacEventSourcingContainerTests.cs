using System.Linq;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.Infrastructure.Command.Services;
using Twizzar.TestCommon;

namespace Twizzar.Design.Infrastructure.Tests.Command.Services
{
    [Category("TwizzarInternal")]
    public class AutofacEventSourcingContainerTests
    {
        private ContainerBuilder _containerBuilder;

        [SetUp]
        public void Setup()
        {
            this._containerBuilder = new ContainerBuilder();
        }

        [Test]
        public void Can_resolve_CommandHandlers()
        {
            // arrange
            var handler = Build.New<ICommandHandler<TestCommand>>();

            this._containerBuilder.RegisterInstance(handler)
                .As<ICommandHandler<TestCommand>>()
                .SingleInstance();

            var sut = new AutofacEventSourcingContainer(this._containerBuilder.Build());

            // act
            var result = sut.GetCommandHandler<TestCommand>();

            // assert
            result.IsSome.Should().BeTrue();
            result.GetValueUnsafe().Should().BeAssignableTo<ICommandHandler<TestCommand>>();
        }

        [Test]
        public void Can_resolve_EventListeners()
        {
            // arrange
            var listener = Build.New<IEventListener<TestEvent>>();
            this._containerBuilder.RegisterInstance(listener)
                .As<IEventListener<TestEvent>>()
                .SingleInstance();

            var sut = new AutofacEventSourcingContainer(this._containerBuilder.Build());

            // act
            var listeners = sut.GetEventListeners<TestEvent>().ToList();

            // assert
            listeners.Should().HaveCount(1);
            listeners.Should().AllBeAssignableTo<IEventListener<TestEvent>>();
        }

        [Test]
        public void Can_resolve_EventListeners_when_registered()
        {
            // arrange
            var listener = Build.New<IEventListener<TestEvent>>();

            var sut = new AutofacEventSourcingContainer(this._containerBuilder.Build());
            sut.RegisterListener(listener);

            // act
            var listeners = sut.GetEventListeners<TestEvent>().ToList();

            // assert
            listeners.Should().HaveCount(1);
            listeners.Should().AllBeAssignableTo<IEventListener<TestEvent>>();
        }
    }
}