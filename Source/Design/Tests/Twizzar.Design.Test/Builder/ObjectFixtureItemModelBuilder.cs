using System.Collections.Immutable;
using System.Linq;
using Twizzar.Design.CoreInterfaces.Query.Services.ReadModel;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.FixtureConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon;
using Twizzar.TestCommon.TypeDescription.Builders;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Test.Builder;

public class ObjectFixtureItemModelBuilder
{
    private FixtureItemId _id = TestHelper.RandomNamedFixtureItemId();
    private Maybe<FixtureItemConstructorModel> _usedConstructor = Maybe.None();
    private IImmutableDictionary<string, FixtureItemMemberModel> _properties = ImmutableDictionary.Create<string, FixtureItemMemberModel>();
    private IImmutableDictionary<string, FixtureItemMemberModel> _fields = ImmutableDictionary.Create<string, FixtureItemMemberModel>();
    private readonly IImmutableDictionary<string, FixtureItemMemberModel> _methods = ImmutableDictionary.Create<string, FixtureItemMemberModel>();
    private readonly TypeDescriptionBuilder _typeDescriptionBuilder = new TypeDescriptionBuilder();

    public ObjectFixtureItemModelBuilder WithId(FixtureItemId id)
    {
        this._id = id;
        return this;
    }

    public ObjectFixtureItemModelBuilder WithUsedConstructor(FixtureItemConstructorModel constructorModel)
    {
        this._usedConstructor = constructorModel;
        this._typeDescriptionBuilder.WithDeclaredConstructors(constructorModel.MethodDescription);

        return this;
    }

    public ObjectFixtureItemModelBuilder WithProperties(
        IImmutableDictionary<string, FixtureItemMemberModel> properties)
    {
        this._properties = properties;
        this._typeDescriptionBuilder.WithDeclaredProperties(
            properties.Select(pair => pair.Value.Description as IPropertyDescription).ToArray());

        return this;
    }

    public ObjectFixtureItemModelBuilder WithFields(
        IImmutableDictionary<string, FixtureItemMemberModel> fields)
    {
        this._fields = fields;
        return this;
    }

    public ObjectFixtureItemModel Build() =>
        new ObjectFixtureItemModel(
            this._id,
            ImmutableDictionary.Create<string, IFixtureConfiguration>(),
            this._usedConstructor,
            this._properties,
            this._fields,
            this._methods,
            ImmutableArray<IMethodDescription>.Empty,
            this._typeDescriptionBuilder.Build());
}