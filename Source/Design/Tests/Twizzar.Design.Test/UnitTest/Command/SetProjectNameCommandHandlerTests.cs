using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Design.Core.Command.FixtureItem;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using TwizzarInternal.Fixture;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Test.UnitTest.Command;

[TestClass]
public partial class SetProjectNameCommandHandlerTests
{
    private Mock<IEventBus> _eventBus;
    private StartFixtureItemConfigurationCommandHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        this._handler = new EmptyStartFixtureItemConfigurationCommandHandlerBuilder().Build(out var context);
        this._eventBus = context.GetAsMoq(p => p.Ctor.eventBus);
    }

    [TestMethod]
    public void Ctor_should_throw_ArgumentNullException_when_eventBus_is_null()
    {
        Verify.Ctor<StartFixtureItemConfigurationCommandHandler>()
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public async Task HandleAsync_should_raise_setProjectNameEvent_on_event_bus()
    {
        // arrange
        var fixtureItemId = FixtureItemId.CreateNameless(TypeFullName.Create(""));

        var command = new StartFixtureItemConfigurationCommand(
            fixtureItemId,
            RandomString(),
            RandomString(),
            Mock.Of<IViSpan>());

        // act
        await this._handler.HandleAsync(command);

        // assert
        this._eventBus.Verify(e => e.PublishAsync(It.IsAny<FixtureItemConfigurationStartedEvent>()),Times.Once);
    }
}