using System;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Twizzar.Design.Core.Command.Services;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;

using TwizzarInternal.Fixture;

using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Core.Tests.Command.Services
{
    [TestFixture]
    public class CommandBusTests
    {
        [SetUp]
        public void SetUp()
        {
            LoggerFactory.SetConfig(new LoggerConfigurationBuilder());
        }

        [Test]
        public void Ctor_parameters_throw_ArgumentNullException_when_null()
        {
            // assert
            Verify.Ctor<CommandBus>()
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public async Task CommandBus_calls_registered_CommandHandlers()
        {
            // arrange
            var testCommand = new TestCommand();

            var container = new Mock<IEventSourcingContainer>();
            var commandHandler = new Mock<ICommandHandler<ITestCommand>>();

            container.Setup(sourcingContainer =>
                    sourcingContainer.GetCommandHandler<ITestCommand>())
                .Returns(Maybe.Some(commandHandler.Object));

            var sut = new CommandBus(container.Object);

            // act
            await sut.SendAsync(testCommand);

            // assert
            container.Verify(
                sourcingContainer =>
                    sourcingContainer.GetCommandHandler<ITestCommand>(),
                Times.Once);

            commandHandler.Verify(
                handler => handler.HandleAsync(testCommand),
                Times.Once);
        }

        [Test]
        public async Task When_CommandHandler_throws_Exception_catch_and_log_as_fatal()
        {
            // arrange
            var commandHandler = new Mock<ICommandHandler<ITestCommand>>();
            var container = new Mock<IEventSourcingContainer>();
            var logger = new Mock<ILogger>();

            container.Setup(sourcingContainer =>
                    sourcingContainer.GetCommandHandler<ITestCommand>())
                .Returns(Maybe.Some(commandHandler.Object));

            commandHandler.Setup(handler => handler.HandleAsync(It.IsAny<ITestCommand>()))
                .Throws<Exception>();

            var sut = new ItemBuilder<CommandBus>()
                .With(p => p.Ctor.container.Value(container.Object))
                .Build();

            sut.Logger = logger.Object;

            // act
            await sut.SendAsync(new TestCommand());

            // assert
            logger.Verify(l => l.Log(LogLevel.Fatal, It.IsAny<Exception>()));
        }

        [Test]
        public void When_no_CommandHandler_is_registered_throw_InternalException()
        {
            // arrange
            var sut = new ItemBuilder<CommandBus>().Build();

            // act
            var f = () => sut.SendAsync(new TestCommand());

            // arrange
            f.Should().Throw<InternalException>();
        }
    }
}