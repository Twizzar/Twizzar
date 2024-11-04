using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;

namespace Twizzar.Design.CoreInterfaces.Command.Commands
{
    /// <summary>
    /// Command to request a creation of a new fixture item.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CreateFixtureItemCommand : ICommand<CreateFixtureItemCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateFixtureItemCommand"/> class.
        /// </summary>
        /// <param name="id">The fixture item id.</param>
        public CreateFixtureItemCommand(FixtureItemId id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets the id of the new fixture item.
        /// </summary>
        public FixtureItemId Id { get; }
    }
}
