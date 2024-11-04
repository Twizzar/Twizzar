using System.Threading.Tasks;

using Twizzar.Design.Core.Command.Services;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;

using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Core.Command.FixtureItem
{
    /// <summary>
    /// Command handler for command <see cref="EndFixtureItemConfigurationCommand"/>.
    /// </summary>
    public class EndFixtureItemConfigurationCommandHandler : EventPublisher, ICommandHandler<EndFixtureItemConfigurationCommand>
    {
        private readonly IEventStore _eventStore;
        private readonly ICommandBus _commandBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndFixtureItemConfigurationCommandHandler"/> class.
        /// </summary>
        /// <param name="eventBus">The event bus service.</param>
        /// <param name="eventStore">The event store.</param>
        /// <param name="commandBus"></param>
        public EndFixtureItemConfigurationCommandHandler(
            IEventBus eventBus,
            IEventStore eventStore,
            ICommandBus commandBus)
            : base(eventBus)
        {
            this.EnsureMany()
                .Parameter(eventStore, nameof(eventStore))
                .Parameter(commandBus, nameof(commandBus))
                .ThrowWhenNull();

            this._eventStore = eventStore;
            this._commandBus = commandBus;
        }

        /// <inheritdoc />
        public async Task HandleAsync(EndFixtureItemConfigurationCommand command)
        {
            this.EnsureParameter(command, nameof(command)).ThrowWhenNull();

            await command.RootFixtureItemId.RootItemPath
                .IfSomeAsync(async path =>
                {
                    // busy waiting for all commands to finish. Because this is also called by a command wait till only one is running.
                    for (int i = 0; this._commandBus.RunningCommands > 1 && i < 10; i++)
                    {
                        await Task.Delay(100);
                    }

                    await this._eventStore.ClearAll(path);
                    await this.PublishAsync(new FixtureItemConfigurationEndedEvent(path));
                });
        }
    }
}
