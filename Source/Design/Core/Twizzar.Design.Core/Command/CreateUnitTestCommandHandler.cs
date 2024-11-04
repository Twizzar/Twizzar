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
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Core.Command;

/// <summary>
/// Command handler for handling the command <see cref="CreateUnitTestCommand"/>.
/// This will create a unit test for the user.
/// </summary>
public class CreateUnitTestCommandHandler : EventPublisher, ICommandHandler<CreateUnitTestCommand>
{
    #region fields

    private readonly ILocationService _locationService;
    private readonly IMappingService _mappingService;
    private readonly IProjectQuery _projectQuery;
    private readonly IDocumentQuery _documentQuery;
    private readonly ITemplateService _templateService;
    private readonly IDocumentContentCreationService _documentContentCreationService;
    private readonly ITestCreationProgressFactory _progressFactory;
    private readonly INavigationService _navigationService;
    private readonly IUserFeedbackService _feedbackService;
    private readonly IBaseConfigQuery _configQuery;
    private CancellationTokenSource _cancellationTokenSource = new();

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateUnitTestCommandHandler"/> class.
    /// </summary>
    /// <param name="locationService"></param>
    /// <param name="mappingService"></param>
    /// <param name="documentQuery"></param>
    /// <param name="templateService"></param>
    /// <param name="documentContentCreationService"></param>
    /// <param name="navigationService"></param>
    /// <param name="eventBus"></param>
    /// <param name="projectQuery"></param>
    /// <param name="progressFactory"></param>
    /// <param name="feedbackService"></param>
    /// <param name="configQuery"></param>
    public CreateUnitTestCommandHandler(
        ILocationService locationService,
        IMappingService mappingService,
        IDocumentQuery documentQuery,
        ITemplateService templateService,
        IDocumentContentCreationService documentContentCreationService,
        IEventBus eventBus,
        IProjectQuery projectQuery,
        ITestCreationProgressFactory progressFactory,
        INavigationService navigationService,
        IUserFeedbackService feedbackService,
        IBaseConfigQuery configQuery)
        : base(eventBus)
    {
        this.EnsureMany()
            .Parameter(locationService, nameof(locationService))
            .Parameter(mappingService, nameof(mappingService))
            .Parameter(documentQuery, nameof(documentQuery))
            .Parameter(templateService, nameof(templateService))
            .Parameter(documentContentCreationService, nameof(documentContentCreationService))
            .Parameter(projectQuery, nameof(projectQuery))
            .Parameter(progressFactory, nameof(progressFactory))
            .Parameter(navigationService, nameof(navigationService))
            .Parameter(feedbackService, nameof(feedbackService))
            .Parameter(configQuery, nameof(configQuery))
            .ThrowWhenNull();

        this._locationService = locationService;
        this._mappingService = mappingService;
        this._documentQuery = documentQuery;
        this._templateService = templateService;
        this._documentContentCreationService = documentContentCreationService;
        this._projectQuery = projectQuery;
        this._progressFactory = progressFactory;
        this._configQuery = configQuery;
        this._navigationService = navigationService;
        this._feedbackService = feedbackService;
    }

    #endregion

    #region members

    /// <inheritdoc />
    public async Task HandleAsync(CreateUnitTestCommand command)
    {
        try
        {
            this._cancellationTokenSource.Dispose();
            this._cancellationTokenSource = new CancellationTokenSource();
            var token = this._cancellationTokenSource.Token;

            using var progress = this.InitUserFeedback();

            var sourceContext = await this._locationService.GetCurrentLocation(command.FilePath, command.CharOffset);
            var (config, templateFile) = await this._configQuery.GetOrCreateConfigsAsync(sourceContext?.Info, token);
            var destinationInfo = await this._mappingService.MapAsync(sourceContext?.Info, config);
            await this._projectQuery.GetOrCreateProject(destinationInfo, sourceContext, config);

            var isSuccessful = await this.TryUntilSuccessful(
                () => this.TryCreateOrUpdateDocument(destinationInfo, sourceContext, templateFile),
                10,
                TimeSpan.FromMilliseconds(500));

            if (!isSuccessful)
            {
                throw new InternalException(
                    $"Cannot create the document {destinationInfo.File}, after several retries.");
            }

            await this.PublishAsync(new UnitTestCreatedEvent());
            await this._navigationService.NavigateAsync(destinationInfo, token);
        }
        catch (ConfigFilesNotExistsException configFilesNotExistsException)
        {
            await this._feedbackService.ShowMessageBoxAsync(
                $"{configFilesNotExistsException.Message}. " +
                $"The files will be generated using the default settings. " +
                $"Please review the files, and subsequently, initiate the test creation once more.");
        }
        catch (UserCanceledException)
        {
            // ignore
        }
        catch (Exception ex)
        {
            await this._feedbackService.ShowErrorBoxAsync(ex.Message);
            this.Log(ex);
            await this.PublishAsync(new UnitTestCreateFailedEvent(ex.Message));
        }
    }

    private ITestCreationProgress InitUserFeedback()
    {
        var maxSteps = this._projectQuery.NumberOfProgressSteps +
                       this._documentContentCreationService.NumberOfProgressSteps +
                       this._documentQuery.NumberOfProgressSteps;

        var progress = this._progressFactory.Create(maxSteps);

        this._projectQuery.AddProgress(progress);
        this._documentContentCreationService.AddProgress(progress);
        this._documentQuery.AddProgress(progress);
        return progress;
    }

    private Task<bool> TryCreateOrUpdateDocument(
        CreationInfo destinationInfo,
        CreationContext sourceContext,
        ITemplateFile templateFile) =>
        this._documentQuery.GetOrCreateDocumentAsync(destinationInfo, sourceContext)
            .MapAsync(context => this._templateService.AddTemplate(sourceContext, context, templateFile))
            .MapAsync(this._documentContentCreationService.CreateContentAsync)
            .SomeOrProvidedAsync(() => false);

    private async Task<bool> TryUntilSuccessful(Func<Task<bool>> task, int maxRetries, TimeSpan waitBetweenRetries)
    {
        this.EnsureParameter(maxRetries, nameof(maxRetries))
            .IsGreaterThan(0)
            .ThrowOnFailure();
        this.EnsureParameter(task, nameof(task))
            .ThrowWhenNull();

        for (var retryCount = maxRetries; retryCount > 0; retryCount--)
        {
            if (await task())
            {
                return true;
            }
            await Task.Delay(waitBetweenRetries);
        }

        return false;
    }

    #endregion
}