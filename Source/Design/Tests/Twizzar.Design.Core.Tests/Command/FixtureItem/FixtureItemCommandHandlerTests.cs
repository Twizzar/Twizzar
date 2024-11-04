using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.FixtureItem.Definition;

using TwizzarInternal.Fixture;

using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Core.Command.FixtureItem
{
    [TestFixture]
    public partial class FixtureItemCommandHandlerTests
    {
        [Test]
        public async Task HandleFixtureItemCommandHandlerTest()
        {
            // arrange
            var sut = new ItemBuilder<FixtureItemCommandHandler>()
                .Build();

            var command = new CreateCustomBuilderCommandBuilder()
                .Build(out var commandContext);

            // act
            await sut.HandleAsync(command);

            // assert
            commandContext.Verify(p => p.DocumentWriter.PrepareClassAsync)
                .Called(1);
        }

        [Test]
        public async Task Handle_CreateFixtureItemCommand_CreateFixtureItem_is_called()
        {
            // arrange
            var definition = new ItemBuilder<IFixtureItemDefinitionNode>()
                .Build();

            var sut = new FixtureItemCommandHandlerBuilder()
                .With(p => p._fixtureItemDefinitionRepository.CreateFixtureItem__FixtureItemId.Value(
                    Result.SuccessAsync<IFixtureItemDefinitionNode, Failure>(definition)))
                .Build(out var context);

            var command = new ItemBuilder<CreateFixtureItemCommand>().Build();

            // act
            await sut.HandleAsync(command);

            // assert
            context.Verify(p => p._fixtureItemDefinitionRepository.CreateFixtureItem)
                .Called(1);
        }

        [Test]
        public async Task Handle_CreateFixtureItemCommand_On_failure_FixtureItemCreatedFailedEvent_is_published()
        {
            // arrange
            var sut = new FixtureItemCommandHandlerBuilder()
                .With(p => p._fixtureItemDefinitionRepository.CreateFixtureItem__FixtureItemId.Value(
                    Result.FailureAsync<IFixtureItemDefinitionNode, Failure>(new Failure(""))))
                .Build(out var context);

            var command = new ItemBuilder<CreateFixtureItemCommand>().Build();

            // act
            await sut.HandleAsync(command);

            // assert
            Mock.Get(context.Get(p => p.Ctor.eventBus))
                .Verify(bus => bus.PublishAsync(It.IsAny<FixtureItemCreatedFailedEvent>()));
        }

        [Test]
        public async Task Handle_ChangeMemberConfigurationCommand_ChangeMemberConfiguration_is_called_on_definition()
        {
            // arrange
            var definition = new IFixtureItemDefinitionNodeBuilder()
                .Build(out var contextDefinition);

            var successDefinition = Result.SuccessAsync<IFixtureItemDefinitionNode, Failure>(definition);

            var sut = new FixtureItemCommandHandlerBuilder2()
                .With(p => p.Ctor.fixtureItemDefinitionRepository.RestoreDefinitionNode__FixtureItemId.Value(
                        successDefinition))
                .Build();

            var command = new ItemBuilder<ChangeMemberConfigurationCommand>().Build();

            // act
            await sut.HandleAsync(command);

            // assert
            contextDefinition.Verify(p => p.ChangeMemberConfiguration)
                .Called(1);
        }

        [Test]
        public async Task Handle_ChangeMemberConfigurationCommand_FixtureItemMemberChangedFailedEvent_is_published_on_failure()
        {
            // arrange
            var failureTask = Result.FailureAsync<IFixtureItemDefinitionNode, Failure>(new Failure(""));

            var sut = new FixtureItemCommandHandlerBuilder2()
                .With(p => p.Ctor.fixtureItemDefinitionRepository.RestoreDefinitionNode__FixtureItemId.Value(
                    failureTask))
                .Build(out var context);

            var command = new ItemBuilder<ChangeMemberConfigurationCommand>().Build();

            // act
            await sut.HandleAsync(command);

            // assert
            Mock.Get(context.Get(p => p.Ctor.eventBus))
                .Verify(bus => bus.PublishAsync(It.IsAny<FixtureItemMemberChangedFailedEvent>()));
        }
    }
}