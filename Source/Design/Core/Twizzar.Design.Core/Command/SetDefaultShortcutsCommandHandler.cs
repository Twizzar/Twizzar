using System.Threading.Tasks;
using Twizzar.Design.Core.Command.Services;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;

namespace Twizzar.Design.Core.Command;

/// <summary>
/// Handler for handling <see cref="SetDefaultShortcutsCommand"/>.
/// </summary>
internal class SetDefaultShortcutsCommandHandler : EventPublisher, ICommandHandler<SetDefaultShortcutsCommand>
{
    private readonly IShortcutService _shortcutService;

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="SetDefaultShortcutsCommandHandler"/> class.
    /// </summary>
    /// <param name="eventBus">Internal cqrs event bus.</param>
    /// <param name="shortcutService">Service for managing shortcuts.</param>
    public SetDefaultShortcutsCommandHandler(IEventBus eventBus, IShortcutService shortcutService)
        : base(eventBus)
    {
        this._shortcutService = shortcutService;
    }

    #endregion

    #region members

    /// <inheritdoc />
    public async Task HandleAsync(SetDefaultShortcutsCommand command)
    {
        await this._shortcutService.SetDefaultKeyBindingsAsync();
    }

    #endregion
}