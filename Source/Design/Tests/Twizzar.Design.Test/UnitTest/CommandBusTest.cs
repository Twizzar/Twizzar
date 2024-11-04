using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Design.Core.Command.Services;
using Twizzar.Design.CoreInterfaces.Command.Services;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Test.UnitTest;

[TestClass]
[TestCategory("Obsolete")]
public class CommandBusTest
{
    [TestMethod]
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
}