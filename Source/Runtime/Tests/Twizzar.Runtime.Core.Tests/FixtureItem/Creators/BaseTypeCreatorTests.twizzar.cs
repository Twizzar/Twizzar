using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using TwizzarInternal.Fixture;

namespace Twizzar.Runtime.Core.Tests.FixtureItem.Creators
{
    partial class BaseTypeCreatorTests
    {
        private class RawValueBaseTypeNodeBuilder : ItemBuilder<Twizzar.Runtime.Core.FixtureItem.Definition.BaseTypeNode, BaseTypeNode0857BuilderPaths>
        {
            public RawValueBaseTypeNodeBuilder()
            {
                this.With(p => p.Ctor.isNullable.Value(false));
                this.With(p => p.Ctor.valueDefinition.Stub<IRawValueDefinition>());
            }
        }
    }
}