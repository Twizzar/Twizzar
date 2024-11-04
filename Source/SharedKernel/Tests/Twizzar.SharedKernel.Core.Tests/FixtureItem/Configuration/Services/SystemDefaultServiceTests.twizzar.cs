using System.Collections.Immutable;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using TwizzarInternal.Fixture;

namespace Twizzar.SharedKernel.Core.Tests.FixtureItem.Configuration.Services
{
    partial class SystemDefaultServiceTests
    {
        private class BaseTypeDescriptionBuilder : ItemBuilder<Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.ITypeDescription, ITypeDescription5cddBuilderPaths>
        {
            public BaseTypeDescriptionBuilder()
            {
                this.With(p => p.IsBaseType.Value(true));
                this.With(p => p.DefaultFixtureKind.Value(FixtureKind.BaseType));
            }
        }

        private class CtorMethodDescriptionBuilder : ItemBuilder<Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.IMethodDescription, IMethodDescription6352BuilderPaths>
        {
            public CtorMethodDescriptionBuilder()
            {
                this.With(p => p.IsConstructor.Value(true));
                this.With(p => p.Name.Value(".ctor"));
            }
        }

        private class ClassITypeDescriptionBuilder : ItemBuilder<Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.ITypeDescription, ITypeDescription5846BuilderPaths>
        {
            public ClassITypeDescriptionBuilder()
            {
                this.With(p => p.IsBaseType.Value(false));
                this.With(p => p.IsClass.Value(true));
                this.With(p => p.DefaultFixtureKind.Value(FixtureKind.Class));
                this.With(p => p.IsInterface.Value(false));
                this.With(p => p.TypeFullName.Stub<ITypeFullName>());
                this.With(p => p.GetDeclaredConstructors.Value(ImmutableArray<IMethodDescription>.Empty));
                this.With(p => p.GetDeclaredFields.Value(ImmutableArray<IFieldDescription>.Empty));
                this.With(p => p.GetDeclaredMethods.Value(ImmutableArray<IMethodDescription>.Empty));
                this.With(p => p.GetDeclaredProperties.Value(ImmutableArray<IPropertyDescription>.Empty));
            }
        }
    }
}