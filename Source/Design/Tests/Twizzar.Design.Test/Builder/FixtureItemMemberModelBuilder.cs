using Moq;
using Twizzar.Design.CoreInterfaces.Query.Services.ReadModel;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.Test.Builder;

public class FixtureItemMemberModelBuilder
{
    private IMemberConfiguration _memberConfiguration = new Mock<IMemberConfiguration>().Object;
    private IMemberDescription _memberDescription = new Mock<IMemberDescription>().Object;

    public FixtureItemMemberModelBuilder WithMemberConfiguration(IMemberConfiguration configuration)
    {
        this._memberConfiguration = configuration;
        return this;
    }

    public FixtureItemMemberModelBuilder WithMemberDescription(IMemberDescription memberDescription)
    {
        this._memberDescription = memberDescription;
        return this;
    }

    public FixtureItemMemberModel Build() =>
        new(
            this._memberConfiguration,
            this._memberDescription);
}