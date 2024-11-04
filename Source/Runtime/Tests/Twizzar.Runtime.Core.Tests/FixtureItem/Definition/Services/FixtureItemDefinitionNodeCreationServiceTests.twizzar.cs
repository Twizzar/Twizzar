using System;
using System.Collections.Immutable;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using TwizzarInternal.Fixture;

namespace Twizzar.Runtime.Core.Tests.FixtureItem.Definition.Services
{
    partial class FixtureItemDefinitionNodeCreationServiceTests
    {
        private class NullableBaseTypeIRuntimeTypeDescriptionBuilder : ItemBuilder<Twizzar.Runtime.CoreInterfaces.FixtureItem.Description.IRuntimeTypeDescription, IRuntimeTypeDescription3dabBuilderPaths>
        {
            public NullableBaseTypeIRuntimeTypeDescriptionBuilder()
            {
                this.With(p => p.IsBaseType.Value(true));
                this.With(p => p.DefaultFixtureKind.Value(FixtureKind.BaseType));
                this.With(p => p.IsClass.Value(false));
                this.With(p => p.IsInterface.Value(false));
                this.With(p => p.IsNullableBaseType.Value(true));
            }
        }

        private class BaseTypeIConfigurationItemBuilder : ItemBuilder<Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.IConfigurationItem, IConfigurationItemb8d0BuilderPaths>
        {
            public BaseTypeIConfigurationItemBuilder()
            {
                this.With(p => p.OnlyVariableMemberConfiguration.Stub<IImmutableDictionary<String, IMemberConfiguration>>());
                this.With(p => p.MemberConfigurations.Stub<IImmutableDictionary<String, IMemberConfiguration>>());
            }
        }

        private class ClassIRuntimeTypeDescriptionBuilder : ItemBuilder<Twizzar.Runtime.CoreInterfaces.FixtureItem.Description.IRuntimeTypeDescription, IRuntimeTypeDescriptionbcf8BuilderPaths>
        {
            public ClassIRuntimeTypeDescriptionBuilder()
            {
                this.With(p => p.IsClass.Value(true));
                this.With(p => p.IsNullableBaseType.Value(false));
                this.With(p => p.IsBaseType.Value(false));
            }
        }
    }
}