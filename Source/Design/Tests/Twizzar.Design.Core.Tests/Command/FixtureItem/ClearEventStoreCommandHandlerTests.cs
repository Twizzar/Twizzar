using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.Core.Command.FixtureItem;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Core.Tests.Command.FixtureItem
{
    public partial class ClearEventStoreCommandHandlerTests
    {
        [Test]
        public void Ctor_throws_exception_when_input_is_null()
        {
            Verify.Ctor<EndFixtureItemConfigurationCommandHandler>()
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public void EventStoreCommandHandler_handle_clear_null_command_throws_exception()
        {
            // arrange
            var sut = new ItemBuilder<EndFixtureItemConfigurationCommandHandler>()
                .Build();

            // act
            var act = async () => await sut.HandleAsync(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public async Task EventStoreCommandHandler_handle_clear_event_calls_event_store_clear_all()
        {
            // arrange
            var fixtureItemId = FixtureItemId.CreateNameless(TypeFullName.Create(""))
                .WithRootItemPath("rootTest");

            var command = new EndFixtureItemConfigurationCommand(fixtureItemId);

            var sut = new EndFixtureItemConfigurationCommandHandlerBuilder()
                //.With(p => p.Ctor.eventStore.Stub<IEventStore>())
                .Build(out var context);
            // act
            await sut.HandleAsync(command);

            // assert
            context.Verify(p => p.Ctor.eventStore.ClearAll__String)
                .WhereRootItemIdIs("rootTest")
                //.WhereParam("rootItemId", "rootTest")
                .Called(1);
        }
    }
}
