using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Design.Core.Command.FixtureItem.Definition;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.TestCommon;
using Twizzar.TestCommon.Builder;
using Twizzar.TestCommon.Configuration.Builders;
using Twizzar.TestCommon.TypeDescription.Builders;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;
using static Twizzar.TestCommon.TestHelper;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;
using static ViCommon.Functional.Monads.ResultMonad.Result;

namespace Twizzar.Design.Test.UnitTest;

[TestClass]
public class FixtureItemDefinitionNodeTest
{
    private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();

    private Mock<IEventStore> _emptyEventStoreMock;
    private Mock<IEventBus> _eventBusMock;
    private IDocumentFileNameQuery _documentFileNameQuery;

    [TestInitialize]
    public void Initialize()
    {
        this._eventBusMock = new Mock<IEventBus>();
        this._emptyEventStoreMock = new Mock<IEventStore>();
        this._emptyEventStoreMock.Setup(store => store.FindLast<FixtureItemCreatedEvent>(It.IsAny<FixtureItemId>()))
            .Returns(Task.FromResult(None<FixtureItemCreatedEvent>()));
        this._documentFileNameQuery = Mock.Of<IDocumentFileNameQuery>(query =>
            query.GetDocumentFileName(It.IsAny<Maybe<string>>()) == SuccessAsync<string, Failure>(""));
    }

    [TestMethod]
    public void Replay_FixtureItemMemberChangedEvent_results_in_new_ConfigurationMember()
    {
        // arrange
        var propName = RandomString();
        var propType = RandomTypeFullName();

        var propValue = RandomString();

        var id = RandomNamedFixtureItemId();
        var typeDescription = new TypeDescriptionBuilder()
            .WithTypeFullName(id.TypeFullName)
            .WithDeclaredProperties(new []
            {
                new PropertyDescriptionBuilder()
                    .WithName(propName)
                    .WithType(propType)
                    .Build(),
            })
            .AsInterface()
            .Build();
        var systemDefaultService = new SystemDefaultServiceBuilder().Build();
        var configFactory = new ConfigurationItemFactoryBuilder().Build();
        var eventStore = new Mock<IEventStore>();

        var e = new FixtureItemMemberChangedEvent(id, new ValueMemberConfiguration(propName, propValue, Source));

        var sut = new FixtureItemDefinitionNode(
            id,
            typeDescription,
            systemDefaultService,
            configFactory,
            this._eventBusMock.Object,
            eventStore.Object,
            this._documentFileNameQuery);

        // act
        sut.Replay(e);

        // assert
        sut.ConfigurationItem.MemberConfigurations.Keys.Should().Contain(propName);
        sut.ConfigurationItem.MemberConfigurations[propName].Should().BeEquivalentTo(e.MemberConfiguration);
    }

    [TestMethod]
    public async Task ChangeMemberConfiguration_raises_FixtureItemMemberChangedEvent_on_success()
    {
        // arrange
        var propName = RandomString();
        var propType = RandomTypeFullName();

        var propValue = RandomString();

        var id = RandomNamedFixtureItemId();
        var typeDescription = new TypeDescriptionBuilder()
            .WithTypeFullName(id.TypeFullName)
            .AsInterface()
            .WithDeclaredProperties(new[]
            {
                new PropertyDescriptionBuilder()
                    .WithName(propName)
                    .WithType(propType)
                    .Build(),
            })
            .Build();
        var systemDefaultService = new SystemDefaultServiceBuilder().Build();
        var configFactory = new ConfigurationItemFactoryBuilder().Build();
        var eventStore = new Mock<IEventStore>();

        var sut = new FixtureItemDefinitionNode(
            id,
            typeDescription,
            systemDefaultService,
            configFactory,
            this._eventBusMock.Object,
            eventStore.Object,
            this._documentFileNameQuery);

        // act
        await sut.ChangeMemberConfiguration(new ValueMemberConfiguration(propName, propValue, Source));

        // assert
        this._eventBusMock.Verify(bus => bus.PublishAsync(It.IsAny<FixtureItemMemberChangedEvent>()), Times.Once);

        this._eventBusMock.Verify(
            bus => bus.PublishAsync(It.Is<FixtureItemMemberChangedEvent>(
                e => e.MemberConfiguration.Name == propName)),
            Times.Once);

        this._eventBusMock.Verify(
            bus => bus.PublishAsync(It.Is<FixtureItemMemberChangedEvent>(
                e => ((ValueMemberConfiguration)e.MemberConfiguration).Value == propValue)),
            Times.Once);

        this._eventBusMock.Verify(bus => bus.PublishAsync(It.Is<FixtureItemMemberChangedEvent>(
                e => e.FixtureItemId == id)),
            Times.Once);
    }

    [TestMethod]
    public async Task ChangeMemberConfiguration_raises_FixtureItemMemberChangedFailEvent_on_missing_member()
    {
        // arrange
        var propName = RandomString();

        var propValue = RandomString();

        var id = RandomNamedFixtureItemId();
        var typeDescription = new TypeDescriptionBuilder()
            .WithTypeFullName(id.TypeFullName)
            .AsInterface()
            .Build();
        var systemDefaultService = new SystemDefaultServiceBuilder().Build();
        var configFactory = new ConfigurationItemFactoryBuilder().Build();
        var eventStore = new Mock<IEventStore>();

        var sut = new FixtureItemDefinitionNode(
            id,
            typeDescription,
            systemDefaultService,
            configFactory,
            this._eventBusMock.Object,
            eventStore.Object,
            this._documentFileNameQuery);

        // act
        await sut.ChangeMemberConfiguration(new ValueMemberConfiguration(propName, propValue, Source));

        // assert
        this._eventBusMock.Verify(bus => bus.PublishAsync(It.IsAny<FixtureItemMemberChangedFailedEvent>()), Times.Once);

        this._eventBusMock.Verify(
            bus => bus.PublishAsync(It.Is<FixtureItemMemberChangedFailedEvent>(
                e => e.MemberConfiguration.Name == propName)),
            Times.Once);

        this._eventBusMock.Verify(
            bus => bus.PublishAsync(It.Is<FixtureItemMemberChangedFailedEvent>(
                e => ((ValueMemberConfiguration)e.MemberConfiguration).Value == propValue)),
            Times.Once);

        this._eventBusMock.Verify(bus => bus.PublishAsync(It.Is<FixtureItemMemberChangedFailedEvent>(
                e => e.FixtureItemId == id)),
            Times.Once);
    }

    [TestMethod]
    public async Task ChangeMemberConfiguration_creates_fixtureItem_when_it_does_not_exists()
    {
        // arrange
        var id = RandomNamedFixtureItemId();
        var valueId = RandomNamedFixtureItemId();

        var typeDescription = new TypeDescriptionBuilder()
            .WithTypeFullName(id.TypeFullName)
            .AsInterface()
            .WithDeclaredProperties(new []
            {
                new PropertyDescriptionBuilder()
                    .WithName(valueId.Name.GetValueUnsafe())
                    .WithType(valueId.TypeFullName)
                    .Build(),
            })
            .Build();

        var systemDefaultService = new SystemDefaultServiceBuilder().Build();
        var configFactory = new ConfigurationItemFactoryBuilder().Build();

        var typeDescriptionQuery = new Mock<CoreInterfaces.Common.FixtureItem.Description.Services.ITypeDescriptionQuery>();
        typeDescriptionQuery.Setup(query => query.GetTypeDescriptionAsync(valueId.TypeFullName, valueId.RootItemPath))
            .Returns(SuccessAsync<ITypeDescription, Failure>(typeDescription));

        var sut = new FixtureItemDefinitionNode(
            id,
            typeDescription,
            systemDefaultService,
            configFactory,
            this._eventBusMock.Object,
            this._emptyEventStoreMock.Object,
            this._documentFileNameQuery);

        // act
        await sut.ChangeMemberConfiguration(
            new LinkMemberConfiguration(valueId.Name.GetValueUnsafe(), valueId, Source));

        // assert
        this._emptyEventStoreMock.Verify(store => store.FindLast<FixtureItemCreatedEvent>(valueId), Times.Once);
        this._eventBusMock.Verify(bus => 
                bus.PublishAsync(
                    It.Is<FixtureItemCreatedEvent>(e => e.FixtureItemId == valueId)), 
            Times.Once);
        this._eventBusMock.Verify(bus =>
                bus.PublishAsync(
                    It.Is<FixtureItemMemberChangedEvent>(e => e.FixtureItemId == id)),
            Times.Once);
    }

    [TestMethod]
    public async Task ChangeMemberConfiguration_on_CtorMemberConfig_creates_fixtureItem_for_parameters_when_it_does_not_exists()
    {
        // arrange
        var id = RandomNamedFixtureItemId();
        var valueId = RandomNamedFixtureItemId();

        var parameterName = RandomString("param");

        var ctorDescription = new MethodDescriptionBuilder()
            .AsConstructor()
            .WithDeclaredParameter(
                new ParameterDescriptionBuilder()
                    .WithName(parameterName)
                    .Build())
            .Build();

        var typeDescription = new TypeDescriptionBuilder()
            .WithTypeFullName(id.TypeFullName)
            .AsInterface()
            .WithDeclaredConstructors(ctorDescription)
            .Build();

        var systemDefaultService = new SystemDefaultServiceBuilder()
            .WithCtorSelector(
                Mock.Of<ICtorSelector>(
                    selector =>
                        selector.GetCtorDescription(It.IsAny<ITypeDescription>(), CtorSelectionBehavior.Max) ==
                        Success(ctorDescription)))
            .Build();

        var configFactory = new ConfigurationItemFactoryBuilder().Build();

        var typeDescriptionQuery = new Mock<CoreInterfaces.Common.FixtureItem.Description.Services.ITypeDescriptionQuery>();

        typeDescriptionQuery.Setup(query => query.GetTypeDescriptionAsync(valueId.TypeFullName, valueId.RootItemPath))
            .Returns(SuccessAsync<ITypeDescription, Failure>(typeDescription));

        var sut = new FixtureItemDefinitionNode(
            id,
            typeDescription,
            systemDefaultService,
            configFactory,
            this._eventBusMock.Object,
            this._emptyEventStoreMock.Object,
            this._documentFileNameQuery);

        // act
        await sut.ChangeMemberConfiguration(
            new CtorMemberConfiguration(
                ImmutableArray<IMemberConfiguration>.Empty
                    .Add(new LinkMemberConfiguration(parameterName, valueId, Source)),
                ImmutableArray<ITypeFullName>.Empty,
                Source)
        );

        // assert
        this._eventBusMock.Verify(bus =>
                bus.PublishAsync(
                    It.Is<FixtureItemCreatedEvent>(e => e.FixtureItemId == valueId)),
            Times.Once);
        this._eventBusMock.Verify(bus =>
                bus.PublishAsync(
                    It.Is<FixtureItemMemberChangedEvent>(e => e.FixtureItemId == id)),
            Times.Once);
    }

    [TestMethod]
    public async Task ChangeMemberConfiguration_does_not_creates_fixtureItem_when_it_does_exists()
    {
        // arrange
        var id = RandomNamedFixtureItemId();
        var valueId = RandomNamedFixtureItemId();

        var typeDescription = new TypeDescriptionBuilder()
            .WithTypeFullName(id.TypeFullName)
            .AsInterface()
            .WithDeclaredProperties(new[]
            {
                new PropertyDescriptionBuilder()
                    .WithName(valueId.Name.GetValueUnsafe())
                    .WithType(valueId.TypeFullName)
                    .Build(),
            })
            .Build();

        var systemDefaultService = new SystemDefaultServiceBuilder().Build();
        var configFactory = new ConfigurationItemFactoryBuilder().Build();

        var eventStore = new Mock<IEventStore>();
        eventStore.Setup(store => store.FindLast<FixtureItemCreatedEvent>(valueId))
            .Returns(
                Task.FromResult(
                    Some(
                        new FixtureItemCreatedEvent(valueId))));

        var typeDescriptionQuery = new Mock<CoreInterfaces.Common.FixtureItem.Description.Services.ITypeDescriptionQuery>();
        typeDescriptionQuery.Setup(query => query.GetTypeDescriptionAsync(valueId.TypeFullName, valueId.RootItemPath))
            .Returns(SuccessAsync<ITypeDescription, Failure>(typeDescription));

        var sut = new FixtureItemDefinitionNode(
            id,
            typeDescription,
            systemDefaultService,
            configFactory,
            this._eventBusMock.Object,
            eventStore.Object,
            this._documentFileNameQuery);

        // act
        await sut.ChangeMemberConfiguration(
            new LinkMemberConfiguration(valueId.Name.GetValueUnsafe(), valueId, Source));

        // assert
        this._eventBusMock.Verify(bus =>
                bus.PublishAsync(
                    It.Is<FixtureItemCreatedEvent>(e => e.FixtureItemId == valueId)),
            Times.Never);

        this._eventBusMock.Verify(bus =>
                bus.PublishAsync(
                    It.Is<FixtureItemMemberChangedEvent>(e => e.FixtureItemId == id)),
            Times.Once);

    }

    [TestMethod]
    public async Task CreateNamedFixtureItem_send_FixtureItemCreatedFailedEvent_when_a_fixtureItem_with_the_same_name_already_exists()
    {
        // arrange
        var id = RandomNamedFixtureItemId();
        var idWithSameName = FixtureItemId.CreateNamed(id.Name.GetValueUnsafe(), RandomTypeFullName());
        idWithSameName.WithRootItemPath(id.RootItemPath);

        var typeDescription = new TypeDescriptionBuilder().Build();
        var systemDefaultService = new SystemDefaultServiceBuilder().Build();
        var eventStore = new Mock<IEventStore>();

        var rootItemPath = id.RootItemPath.AsMaybeValue()
            .Should()
            .BeAssignableTo<SomeValue<string>>()
            .Subject.Value;

        eventStore.Setup(store => store.FindAll<FixtureItemCreatedEvent>(rootItemPath))
            .Returns(
                Task.FromResult(Enumerable.Empty<FixtureItemCreatedEvent>()
                    .Append(new FixtureItemCreatedEvent(idWithSameName))));

        var sut = new FixtureItemDefinitionNode(
            id,
            typeDescription,
            systemDefaultService,
            Build.New<IConfigurationItemFactory>(),
            this._eventBusMock.Object,
            eventStore.Object,
            this._documentFileNameQuery);

        // act
        await sut.CreateNamedFixtureItem();

        // assert
        this._eventBusMock.Verify(bus => 
            bus.PublishAsync(
                It.Is<FixtureItemCreatedFailedEvent>(e => e.FixtureItemId == id)), Times.Once);
    }
}