using System.Threading.Tasks;
using Twizzar.Design.Core.Command.Services;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;

namespace Twizzar.Design.Core.Command;

/// <summary>
/// Handler for handling <see cref="EnableOrDisableAnalyticsCommand"/>.
/// </summary>
internal class AnalyticsCommandHandler : EventPublisher, ICommandHandler<EnableOrDisableAnalyticsCommand>
{
    #region fields

    private readonly ISettingsWriter _settingsWriter;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyticsCommandHandler"/> class.
    /// </summary>
    /// <param name="eventBus"></param>
    /// <param name="settingsWriter"></param>
    public AnalyticsCommandHandler(IEventBus eventBus, ISettingsWriter settingsWriter)
        : base(eventBus)
    {
        this._settingsWriter = settingsWriter;
    }

    #endregion

    #region members

    /// <inheritdoc />
    public async Task HandleAsync(EnableOrDisableAnalyticsCommand command)
    {
        this._settingsWriter.SetAnalyticsEnabled(command.Enabled);
        await this.PublishAsync(new AnalyticsEnabledOrDisabledEvent(command.Enabled));
    }

    #endregion
}