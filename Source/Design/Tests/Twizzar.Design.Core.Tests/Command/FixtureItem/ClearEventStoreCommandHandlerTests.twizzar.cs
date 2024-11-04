using Twizzar.Design.CoreInterfaces.Command.Services;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Core.Tests.Command.FixtureItem
{
    partial class ClearEventStoreCommandHandlerTests
    {
        private class EndFixtureItemConfigurationCommandHandlerBuilder : ItemBuilder<Twizzar.Design.Core.Command.FixtureItem.EndFixtureItemConfigurationCommandHandler, EndFixtureItemConfigurationCommandHandler03b8BuilderPaths>
        {
            public EndFixtureItemConfigurationCommandHandlerBuilder()
            {
                this.With(p => p.Ctor.eventBus.Stub<IEventBus>());
            }
        }
    }
}