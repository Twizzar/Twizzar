using System;
using System.Threading;
using System.Threading.Tasks;

using Twizzar.Design.Core.Command.Services;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.CoreInterfaces.TestCreation.BaseConfig;
using Twizzar.Design.CoreInterfaces.TestCreation.Config;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Core.Command;

/// <summary>
/// Command handler for handling the command <see cref="UnitTestNavigationCommand"/>.
/// This will navigate to the unit test file and vise versa.
/// </summary>
public class UnitTestNavigationCommandHandler : EventPublisher, ICommandHandler<UnitTestNavigationCommand>
{
    #region fields

    private readonly ILocationService _locationService;
    private readonly IMappingService _mappingService;
    private readonly INavigationService _navigationService;
    private readonly IBaseConfigQuery _configQuery;
    private readonly IUserFeedbackService _userFeedbackService;
    private CancellationTokenSource _cancellationTokenSource = new();

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitTestNavigationCommandHandler"/> class.
    /// </summary>
    /// <param name="locationService"></param>
    /// <param name="mappingService"></param>
    /// <param name="navigationService"></param>
    /// <param name="eventBus"></param>
    /// <param name="configQuery"></param>
    /// <param name="userFeedbackService"></param>
    public UnitTestNavigationCommandHandler(
        ILocationService locationService,
        IMappingService mappingService,
        INavigationService navigationService,
        IEventBus eventBus,
        IBaseConfigQuery configQuery,
        IUserFeedbackService userFeedbackService)
        : base(eventBus)
    {
        this.EnsureMany()
            .Parameter(locationService, nameof(locationService))
            .Parameter(navigationService, nameof(navigationService))
            .Parameter(mappingService, nameof(mappingService))
            .Parameter(configQuery, nameof(configQuery))
            .Parameter(userFeedbackService, nameof(userFeedbackService))
            .ThrowWhenNull();

        this._locationService = locationService;
        this._mappingService = mappingService;
        this._navigationService = navigationService;
        this._configQuery = configQuery;
        this._userFeedbackService = userFeedbackService;
    }

    #endregion

    #region members

    /// <inheritdoc />
    public async Task HandleAsync(UnitTestNavigationCommand command)
    {
        try
        {
            this._cancellationTokenSource.Dispose();
            this._cancellationTokenSource = new CancellationTokenSource();
            var token = this._cancellationTokenSource.Token;

            var sourceContext = await this._locationService.GetCurrentLocation(command.FilePath, command.CharOffset);
            var (config, _) = await this._configQuery.GetOrCreateConfigsAsync(sourceContext?.Info, token);

            var result = await this._navigationService.NavigateBackAsync(sourceContext, token)
                .BindFailureAsync(async f1 =>
                {
                    var destinationInfo = await this._mappingService.MapAsync(sourceContext?.Info, config);
                    return await this._navigationService.NavigateAsync(destinationInfo, token)
                        .MapFailureAsync(f2 => new AggregateFailure(f1, f2));
                });

            if (result.AsResultValue() is FailureValue<AggregateFailure> f)
            {
                await this._userFeedbackService.ShowMessageBoxAsync(@$"Navigation failed, tried first to navigate form test method to method under test but it failed with the error:
{f.Value.Failures[0].Message}

Then tried to navigate form method under test to test method but it failed with the error:
{f.Value.Failures[1].Message}

For navigation form the test:
Make sure the test method is annotated with the TestSource Attribute and the attribute argument contains the method under test name. Use the nameof expression to declare the method name.

For navigation to the test:
Check the mapping configuration.
");
                await this.PublishAsync(new UnitTestNavigationFailedEvent(f.Value.Message));
                return;
            }

            await this.PublishAsync(new UnitTestNavigatedEvent());
        }
        catch (ConfigFilesNotExistsException configFilesNotExistsException)
        {
            await this._userFeedbackService.ShowErrorBoxAsync(configFilesNotExistsException.Message);
        }
        catch (Exception ex)
        {
            this.Log(ex);
            await this.PublishAsync(new UnitTestNavigationFailedEvent(ex.Message));
        }
    }

    #endregion
}