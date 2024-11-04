using System;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Core.Command.Services
{
    /// <summary>
    /// Bus for sending commands to the registered command handlers.
    /// </summary>
    public class CommandBus : ICommandBus
    {
        #region fields

        private readonly IEventSourcingContainer _container;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBus"/> class.
        /// </summary>
        /// <param name="container">The container for resolving <see cref="IEventListener{TEvent}"/> and <see cref="IEventStoreToQueryCacheSynchronizer"/>.</param>
        public CommandBus(IEventSourcingContainer container)
        {
            this._container = this.EnsureCtorParameterIsNotNull(container, nameof(container));
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public int RunningCommands { get; private set; } = 0;

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public async Task SendAsync<TCommand>(ICommand<TCommand> command)
            where TCommand : ICommand
        {
            var handler = this._container.GetCommandHandler<TCommand>();

            if (handler.AsMaybeValue() is SomeValue<ICommandHandler<TCommand>> commandHandler)
            {
                this.RunningCommands++;

                await Task.Run(() =>
                {
                    try
                    {
                        return commandHandler.Value.HandleAsync((TCommand)command);
                    }
                    catch (Exception exp)
                    {
                        this.Log(exp, LogLevel.Fatal);
                    }
                    finally
                    {
                        this.RunningCommands--;
                    }
                    return Task.CompletedTask;
                });
            }
            else
            {
                throw this.LogAndReturn(
                    new InternalException(
                        $"No {nameof(ICommandHandler<TCommand>)} is registered for handling the command."),
                    LogLevel.Fatal);
            }
        }

        #endregion
    }
}