using Microsoft.VisualStudio.Text;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.FixtureItem.Config;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.SharedKernel.NLog.Interfaces;
using TwizzarInternal.Fixture;


namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Configs;

public class EmptyConfigs
{
    public class EmptyRoslynConfigEventWriterBuilder : ItemBuilder<RoslynConfigEventWriter, RoslynConfigEventWriterCustomPaths>
    {
        public EmptyRoslynConfigEventWriterBuilder()
        {
            this.With(p => p.Ctor.roslynConfigWriter.Stub<IRoslynConfigWriter>());
            this.With(p => p.Logger.Stub<ILogger>());
        }
    }

    public class EmptyFixtureItemMemberChangedEventBuilder : ItemBuilder<FixtureItemMemberChangedEvent, FixtureItemMemberChangedEventCustomPaths>
    {
        public EmptyFixtureItemMemberChangedEventBuilder()
        {
            this.With(p => p.Ctor.IsFromInitialization.Value(false));
        }
    }

    public class DefaultSnapShotConfigBuilder : ItemBuilder<SnapshotSpan, DefaultSnapShotConfigBuilderPaths>
    {
        public DefaultSnapShotConfigBuilder()
        {
            this.With(p => p.Ctor.length.Value(10));
            this.With(p => p.Ctor.start.Value(0));

            this.With(p => p.Ctor.snapshot.Length.Value(20));
            this.With(p => p.Ctor.snapshot.Version.VersionNumber.Value(1));
            this.With(p => p.Ctor.snapshot.CreateTrackingPoint.Stub<ITrackingPoint>());
        }
    }
}