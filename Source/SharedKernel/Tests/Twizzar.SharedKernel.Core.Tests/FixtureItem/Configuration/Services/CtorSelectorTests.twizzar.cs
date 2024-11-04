using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using TwizzarInternal.Fixture;

namespace Twizzar.SharedKernel.Core.Tests.FixtureItem.Configuration.Services
{
    partial class CtorSelectorTests
    {
        private class PrivateIMethodDescriptionBuilder : ItemBuilder<Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.IMethodDescription, IMethodDescriptionca30BuilderPaths>
        {
            public PrivateIMethodDescriptionBuilder()
            {
                this.With(p => p.AccessModifier.InstanceOf<AccessModifier>());
                this.With(p => p.AccessModifier.Ctor.isPrivate.Value(true));
                this.With(p => p.AccessModifier.Ctor.isPublic.Value(false));
                this.With(p => p.AccessModifier.Ctor.isProtected.Value(false));
                this.With(p => p.AccessModifier.Ctor.isInternal.Value(true));
            }
        }

        private class ClassITypeDescriptionBuilder : ItemBuilder<Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.ITypeDescription, ITypeDescription65fcBuilderPaths>
        {
            public ClassITypeDescriptionBuilder()
            {
                this.With(p => p.IsClass.Value(true));
            }
        }

        private class IMethodDescriptiond2eeBuilder : ItemBuilder<Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.IMethodDescription, IMethodDescriptiond2eeBuilderPaths>
        {
            public IMethodDescriptiond2eeBuilder()
            {
                this.With(p => p.AccessModifier.InstanceOf<AccessModifier>());
                this.With(p => p.AccessModifier.Ctor.isPrivate.Value(false));
                this.With(p => p.AccessModifier.Ctor.isPublic.Value(true));
                this.With(p => p.AccessModifier.Ctor.isProtected.Value(true));
                this.With(p => p.AccessModifier.Ctor.isInternal.Value(true));
            }
        }
    }
}