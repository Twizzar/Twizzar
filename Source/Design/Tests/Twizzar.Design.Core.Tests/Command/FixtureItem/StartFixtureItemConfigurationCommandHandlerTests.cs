using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Twizzar.Design.Core.Command.FixtureItem;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.TestCommon;
using Twizzar.TestCommon.Configuration.Builders;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Core.Tests.Command.FixtureItem
{
    [TestFixture]
    public class StartFixtureItemConfigurationCommandHandlerTests
    {
        private Mock<IEventBus> _eventBusMock;
        private Mock<IUserConfigurationQuery> _userConfigurationQueryMock;
        private Mock<IEventStore> _eventStoreMock;
        private StartFixtureItemConfigurationCommandHandler _sut;
        private string _projectName;

        [SetUp]
        public void SetUp()
        {
            this._projectName = RandomString();
            this._eventBusMock = new Mock<IEventBus>();
            this._userConfigurationQueryMock = new Mock<IUserConfigurationQuery>();
            this._eventStoreMock = new Mock<IEventStore>();

            this._sut = new StartFixtureItemConfigurationCommandHandler(
                this._eventBusMock.Object,
                this._userConfigurationQueryMock.Object,
                this._eventStoreMock.Object);
        }

        [Test]
        public void Ctor_parameters_throw_ArgumentNullException_when_null()
        {
            // assert
            Verify.Ctor<StartFixtureItemConfigurationCommandHandler>()
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public async Task FixtureItemConfigurationStartedEvent_is_raised()
        {
            // arrange
            var id = RandomNamedFixtureItemId();

            // act
            await this._sut.HandleAsync(
                new StartFixtureItemConfigurationCommand(
                    id,
                    this._projectName,
                    RandomString(),
                    Mock.Of<IViSpan>()));

            // assert
            this._eventBusMock.Verify(bus => bus.PublishAsync(It.IsAny<FixtureItemCreatedEvent>()), Times.Never);
            this._eventBusMock.Verify(bus => bus.PublishAsync(It.IsAny<FixtureItemConfigurationStartedEvent>()), Times.Once);
        }

        [Test]
        public async Task FixtureItemCreatedEvent_is_raised()
        {
            // arrange
            var id = RandomNamedFixtureItemId();

            var configItem = new ConfigurationItemBuilder()
                .WithId(id)
                .Build();

            this._userConfigurationQueryMock.Setup(query => query.GetAllAsync(id.RootItemPath, CancellationToken.None))
                .Returns(Task.FromResult<IEnumerable<IConfigurationItem>>(new [] { configItem }));

            // act
            await this._sut.HandleAsync(
                new StartFixtureItemConfigurationCommand(
                    id,
                    this._projectName,
                    RandomString(),
                    Mock.Of<IViSpan>()));

            // assert
            this._eventBusMock.Verify(bus => bus.PublishAsync(new FixtureItemCreatedEvent(id)), Times.Once);
            this._eventBusMock.Verify(bus => bus.PublishAsync(It.IsAny<FixtureItemConfigurationStartedEvent>()), Times.Once);
        }

        [Test]
        public async Task FixtureItemMemberChangedEvent_is_raised()
        {
            // arrange
            var id = RandomNamedFixtureItemId();
            var memberConfiguration = new UniqueValueMemberConfiguration(RandomString(), Mock.Of<IConfigurationSource>());

            var configItem = new ConfigurationItemBuilder()
                .WithId(id)
                .WithMemberConfiguration(memberConfiguration)
                .Build();

            this._userConfigurationQueryMock.Setup(query => query.GetAllAsync(id.RootItemPath, CancellationToken.None))
                .Returns(Task.FromResult<IEnumerable<IConfigurationItem>>(new[] { configItem }));

            // act
            await this._sut.HandleAsync(
                new StartFixtureItemConfigurationCommand(
                    id,
                    this._projectName,
                    RandomString(),
                    Mock.Of<IViSpan>()));

            // assert
            this._eventBusMock.Verify(bus => bus.PublishAsync(new FixtureItemCreatedEvent(id)), Times.Once);
            this._eventBusMock.Verify(bus => bus.PublishAsync(new FixtureItemMemberChangedEvent(id, memberConfiguration, true)), Times.Once);
            this._eventBusMock.Verify(bus => bus.PublishAsync(It.IsAny<FixtureItemConfigurationStartedEvent>()), Times.Once);
        }

        [Test]
        public async Task When_FixtureItemConfigurationEvent_already_started_do_not_publish_configuration_events()
        {
            // arrange
            var id = RandomNamedFixtureItemId();

            var memberConfiguration = new UniqueValueMemberConfiguration(RandomString(), Mock.Of<IConfigurationSource>());
            var configItem = new ConfigurationItemBuilder()
                .WithId(id)
                .WithMemberConfiguration(memberConfiguration)
                .Build();
            this._userConfigurationQueryMock.Setup(query => query.GetAllAsync(id.RootItemPath, CancellationToken.None))
                .Returns(Task.FromResult<IEnumerable<IConfigurationItem>>(new[] { configItem }));

            this._eventStoreMock.Setup(store => store.FindLast<FixtureItemConfigurationStartedEvent>(It.IsAny<FixtureItemId>()))
                .Returns(
                    () => Task.FromResult(
                        Maybe.Some(
                            Build.New<FixtureItemConfigurationStartedEvent>())));

            var command = new StartFixtureItemConfigurationCommand(
                id,
                this._projectName,
                RandomString(),
                Mock.Of<IViSpan>());

            // act
            await this._sut.HandleAsync(command);

            // assert
            this._eventBusMock.Verify(bus => bus.PublishAsync(It.IsAny<FixtureItemCreatedEvent>()), Times.Never);
            this._eventBusMock.Verify(bus => bus.PublishAsync(It.IsAny<FixtureItemMemberChangedEvent>()), Times.Never);
            this._eventBusMock.Verify(bus => bus.PublishAsync(It.IsAny<FixtureItemConfigurationStartedEvent>()), Times.Once);
        }
    }
}