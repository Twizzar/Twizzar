using Twizzar.Design.Core.Command.FixtureItem;
using Twizzar.Design.CoreInterfaces.Command.Services;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Test.UnitTest.Command;

partial class SetProjectNameCommandHandlerTests
{
    private class EmptyStartFixtureItemConfigurationCommandHandlerBuilder : ItemBuilder<StartFixtureItemConfigurationCommandHandler, EmptyStartFixtureItemConfigurationCommandHandlerBuilderPaths>
    {
        public EmptyStartFixtureItemConfigurationCommandHandlerBuilder()
        {
            this.With(p => p.Ctor.eventBus.Stub<IEventBus>());
        }
    }
}