using System.Threading.Tasks;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.CoreInterfaces.Command.Services
{
    /// <summary>
    /// Handles a specific type of command.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to handle.</typeparam>
    public interface ICommandHandler<in TCommand> : IService
        where TCommand : ICommand
    {
        /// <summary>
        /// Handle the command.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        /// <returns>A task.</returns>
        public Task HandleAsync(TCommand command);
    }
}