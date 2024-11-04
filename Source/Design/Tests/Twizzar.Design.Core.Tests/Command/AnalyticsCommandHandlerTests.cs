using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;

namespace Twizzar.Design.Core.Tests.Command;

[TestFixture]
public partial class AnalyticsCommandHandlerTests
{
    #region members

    [TestCase(true)]
    [TestCase(false)]
    public async Task Ctor_parameters_throw_ArgumentNullException_when_null(bool enabled)
    {
        // assert
        var sut = new AnalyticsCommandHandlerBuilder()
            //.With(p => p.Ctor.settingsWriter.Stub<ISettingsWriter>())
            .Build(out var scope);

        var command = new EnableOrDisableAnalyticsCommand(enabled);

        await sut.HandleAsync(command);

        scope.Verify(p => p.Ctor.settingsWriter.SetAnalyticsEnabled__Boolean)
            .WhereEnabledIs(enabled)
            .Called(1);
    }

    [Test]
    public async Task Command_gets_published()
    {
        // assert
        var eventBusMock = new Mock<IEventBus>();

        var sut = new AnalyticsCommandHandlerBuilder()
            .With(p => p.Ctor.eventBus.Value(eventBusMock.Object))
            .Build(out var scope);

        var command = new EnableOrDisableAnalyticsCommand(false);

        await sut.HandleAsync(command);

        eventBusMock.Verify(bus => bus.PublishAsync(
            It.Is<AnalyticsEnabledOrDisabledEvent>(
                e => e.Enabled == command.Enabled)));
    }

    #endregion
}