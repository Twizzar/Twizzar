using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.FixtureInformations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Ui.Tests.ViewModels.FixtureItem.Nodes.FixtureInformations;

partial class FixtureItemInformationTests
{
    private class EmptyFixtureItemInformationBuild : ItemBuilder<FixtureItemInformation, EmptyFixtureItemInformationBuilderPaths>
    {
        public EmptyFixtureItemInformationBuild()
        {
            this.With(p => p.Ctor.fixtureDescription.Stub<ITypeDescription>());
            this.With(p => p.Ctor.id.InstanceOf<FixtureItemId>());
            this.With(p => p.Ctor.parentPath.Unique());
            this.With(p => p.Ctor.memberConfiguration.Stub<IMemberConfiguration>());
        }
    }
}