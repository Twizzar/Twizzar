using System.Threading.Tasks;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.CoreInterfaces.Command.Services
{
    /// <summary>
    /// The command bus service delegates command to the registered handlers <see cref="ICommandHandler{TCommand}"/>.
    /// </summary>
    public interface ICommandBus : IService
    {
        /// <summary>
        /// Gets the count of still running commands.
        /// </summary>
        public int RunningCommands { get; }

        /// <summary>
        /// Send a command to the handlers.
        /// </summary>
        /// <typeparam name="TCommand">Command type.</typeparam>
        /// <param name="command">The command to send.</param>
        /// <returns>A task.</returns>
        public Task SendAsync<TCommand>(ICommand<TCommand> command)
            where TCommand : ICommand;
    }
}
