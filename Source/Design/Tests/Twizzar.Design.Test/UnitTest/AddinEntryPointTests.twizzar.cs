using TwizzarInternal.Fixture;

namespace Twizzar.Design.Test.UnitTest;

partial class AddinEntryPointTests
{
    private class AddinEntryPointBuilder : ItemBuilder<Twizzar.Design.Infrastructure.VisualStudio.AddinEntryPoint, AddinEntryPoint04c7BuilderPaths>
    {
    }

    private class ISolutionEventsPublishefBuilder : ItemBuilder<Twizzar.Design.CoreInterfaces.Common.VisualStudio.ISolutionEventsPublisher, ISolutionEventsPublisher9c0fBuilderPaths>
    {
        public ISolutionEventsPublishefBuilder()
        {
        }
    }
}