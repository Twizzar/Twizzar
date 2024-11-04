using System.Collections.Immutable;
using System.Linq;
using Moq;
using Twizzar.Design.CoreInterfaces.Query.Services.ReadModel;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.TestCommon;
using Twizzar.TestCommon.TypeDescription.Builders;

namespace Twizzar.Design.Test.Builder;

public class FixtureItemConstructorModelBuilder
{
    private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();

    private ImmutableArray<FixtureItemParameterModel> _parameters = ImmutableArray<FixtureItemParameterModel>.Empty;
    private readonly MethodDescriptionBuilder _methodDescriptionBuilder = new();
    private CtorMemberConfiguration _ctorMemberConfiguration = new(
        ImmutableArray<IMemberConfiguration>.Empty
            .Add(new UniqueValueMemberConfiguration(TestHelper.RandomString(), Source)),
        ImmutableArray<ITypeFullName>.Empty,
        Source);

    public FixtureItemConstructorModelBuilder WithParameters(ImmutableArray<FixtureItemParameterModel> parameters)
    {
        this._parameters = parameters;
        this._methodDescriptionBuilder.WithDeclaredParameter(parameters.Select(model => model.Description));
            
        foreach (var fixtureItemParameterModel in parameters)
        {
            this._ctorMemberConfiguration = this._ctorMemberConfiguration.WithParameter(
                fixtureItemParameterModel.Description.Name, fixtureItemParameterModel.Configuration);
        }

        return this;
    }

    public FixtureItemConstructorModel Build() =>
        new(
            this._parameters,
            this._methodDescriptionBuilder.Build(),
            this._ctorMemberConfiguration
        );
}