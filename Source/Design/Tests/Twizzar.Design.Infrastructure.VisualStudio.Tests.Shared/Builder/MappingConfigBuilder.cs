using System.Collections.Immutable;
using Twizzar.Design.CoreInterfaces.TestCreation.Mapping;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;

public class MappingConfigBuilder : ItemBuilder<MappingConfig, MappingConfigBuilderPaths>
{
    public MappingConfigBuilder(ImmutableArray<MappingEntry> entries)
    {
        this.With(p => p.Ctor.FileMapping.Value(entries));
        this.With(p => p.Ctor.MemberMapping.Value(entries));
        this.With(p => p.Ctor.NamespaceMapping.Value(entries));
        this.With(p => p.Ctor.ProjectMapping.Value(entries));
        this.With(p => p.Ctor.TypeMapping.Value(entries));
        this.With(p => p.Ctor.ProjectPathMapping.Value(entries));
        this.With(p => p.Ctor.FilePathMapping.Value(entries));
    }
}