using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes;
using Twizzar.SharedKernel.NLog.Interfaces;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Ui.Tests.ViewModels.FixtureItem.Nodes;

partial class MethodNodeSenderTests
{
    private class EmptyIFixtureItemInformationBuilder : ItemBuilder<IFixtureItemInformation, EmptyIFixtureItemInformationBuilderPaths>
    {
    }

    private class EmptyMethodNodeSenderBuilder : ItemBuilder<MethodNodeSender, EmptyMethodNodeSenderBuilderPaths>
    {
        public EmptyMethodNodeSenderBuilder()
        {
            this.With(p => p.Ctor.commandBus.Stub<ICommandBus>());
        }
    }

    private class MethodNodeSenderWithStubLoggerBuilder : ItemBuilder<MethodNodeSender, MethodNodeSenderWithStubLoggerBuilderPaths>
    {
        public MethodNodeSenderWithStubLoggerBuilder()
        {
            this.With(p => p.Logger.Stub<ILogger>());
        }
    }
}