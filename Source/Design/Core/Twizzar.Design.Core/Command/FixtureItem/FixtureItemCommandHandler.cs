using System;
using System.Threading;
using System.Threading.Tasks;

using Twizzar.Design.Core.Command.Services;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.FixtureItem.Definition;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Core.Command.FixtureItem
{
    /// <summary>
    /// Command handler for handling fixture item specific commands.
    /// </summary>
    public class FixtureItemCommandHandler :
        EventPublisher,
        ICommandHandler<ChangeMemberConfigurationCommand>,
        ICommandHandler<CreateFixtureItemCommand>,
        ICommandHandler<CreateCustomBuilderCommand>
    {
        #region fields

        private readonly IFixtureItemDefinitionRepository _fixtureItemDefinitionRepository;
        private readonly SemaphoreSlim _lock = new(1, 1);

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemCommandHandler"/> class.
        /// </summary>
        /// <param name="eventBus">The event bus service.</param>
        /// <param name="fixtureItemDefinitionRepository">The node repo.</param>
        public FixtureItemCommandHandler(
            IEventBus eventBus,
            IFixtureItemDefinitionRepository fixtureItemDefinitionRepository)
            : base(eventBus)
        {
            this.EnsureParameter(fixtureItemDefinitionRepository, nameof(fixtureItemDefinitionRepository))
                .ThrowWhenNull();

            this._fixtureItemDefinitionRepository = fixtureItemDefinitionRepository;
        }

        #endregion

        #region members

        /// <inheritdoc />
        public async Task HandleAsync(ChangeMemberConfigurationCommand command)
        {
            this.EnsureParameter(command, nameof(command)).ThrowWhenNull();

            try
            {
                await this._lock.WaitAsync(TimeSpan.FromSeconds(1));

                var node = await this._fixtureItemDefinitionRepository
                    .RestoreDefinitionNode(command.FixtureItemId);

                await node.DoAsync(
                    definitionNode => definitionNode.ChangeMemberConfiguration(command.MemberConfiguration),
                    failure => this.PublishAsync(
                        new FixtureItemMemberChangedFailedEvent(
                            command.FixtureItemId,
                            command.MemberConfiguration,
                            failure.Message)));
            }
            catch (Exception e)
            {
                this.Log(e);
            }
            finally
            {
                this._lock.Release();
            }
        }

        /// <inheritdoc />
        public async Task HandleAsync(CreateFixtureItemCommand command)
        {
            this.EnsureParameter(command, nameof(command)).ThrowWhenNull();

            try
            {
                await this._lock.WaitAsync(TimeSpan.FromSeconds(1));
                var id = command.Id;

                var result = await this._fixtureItemDefinitionRepository.CreateFixtureItem(id);

                result.IfFailure(async failure =>
                    await this.PublishAsync(
                        new FixtureItemCreatedFailedEvent(
                            command.Id,
                            failure.Message)));
            }
            catch (Exception e)
            {
                this.Log(e);
            }
            finally
            {
                this._lock.Release();
            }
        }

        /// <inheritdoc />
        public async Task HandleAsync(CreateCustomBuilderCommand command)
        {
            try
            {
                await this._lock.WaitAsync(TimeSpan.FromSeconds(1));

                await command.DocumentWriter.PrepareCodeBehindAsync(command.AdornmentInformation, command.BuilderName);
                await command.DocumentWriter.PrepareClassAsync(command.AdornmentInformation, command.BuilderName);
            }
            catch (Exception e)
            {
                this.Log(e);
            }
            finally
            {
                this._lock.Release();
            }
        }

        #endregion
    }
}