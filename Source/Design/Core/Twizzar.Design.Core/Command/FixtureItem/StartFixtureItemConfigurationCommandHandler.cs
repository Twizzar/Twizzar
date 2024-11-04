using System;
using System.Threading;
using System.Threading.Tasks;

using Twizzar.Design.Core.Command.Services;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Core.Command.FixtureItem
{
    /// <summary>
    /// Handles the <see cref="StartFixtureItemConfigurationCommand"/>.
    /// </summary>
    public class StartFixtureItemConfigurationCommandHandler : EventPublisher,
        ICommandHandler<StartFixtureItemConfigurationCommand>
    {
        #region fields

        private readonly IUserConfigurationQuery _userConfigurationQuery;
        private readonly IEventStore _eventStore;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="StartFixtureItemConfigurationCommandHandler"/> class.
        /// </summary>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="userConfigurationQuery"></param>
        /// <param name="eventStore"></param>
        public StartFixtureItemConfigurationCommandHandler(
            IEventBus eventBus,
            IUserConfigurationQuery userConfigurationQuery,
            IEventStore eventStore)
            : base(eventBus)
        {
            this.EnsureMany()
                .Parameter(userConfigurationQuery, nameof(userConfigurationQuery))
                .Parameter(eventStore, nameof(eventStore))
                .ThrowWhenNull();

            this._userConfigurationQuery = userConfigurationQuery;
            this._eventStore = eventStore;
        }

        #endregion

        #region members

        /// <inheritdoc />
        public async Task HandleAsync(StartFixtureItemConfigurationCommand command)
        {
            this.EnsureParameter(command, nameof(command)).ThrowWhenNull();

            var initializeEventStoreEvent = await this._eventStore
                .FindLast<FixtureItemConfigurationStartedEvent>(command.RootFixtureItemId);

            // Get all user configurations for a specific project.
            try
            {
                await this.PublishAsync(
                    new FixtureItemConfigurationStartedEvent(
                        command.RootFixtureItemId,
                        command.ProjectName,
                        command.DocumentFilePath,
                        command.InvocationSpan));

                // When FixtureItemConfigurationStartedEvent already exists the event store is already initialized. And we only update the event store.
                if (initializeEventStoreEvent.IsSome)
                {
                    return;
                }

                var configs = await this._userConfigurationQuery.GetAllAsync(command.RootFixtureItemId.RootItemPath, CancellationToken.None);

                // For all user configs
                foreach (var item in configs)
                {
                    // Publish a create event.
                    await this.PublishAsync(new FixtureItemCreatedEvent(item.Id));

                    // For all member configurations publish a member changed event.
                    foreach (var (_, memberConfiguration) in item.MemberConfigurations)
                    {
                        await this.PublishAsync(new FixtureItemMemberChangedEvent(item.Id, memberConfiguration, true));
                    }
                }
            }
            catch (Exception e)
            {
                this.Log(e.Message, LogLevel.Fatal);
            }
        }

        #endregion
    }
}