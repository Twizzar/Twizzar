using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Command.Commands;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Infrastructure.VisualStudio2019.Interfaces.Roslyn;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment;

/// <summary>
/// Represents an adornment session. The session is opened when the peek view is created and closed when the peek view will be closed.
/// </summary>
public sealed class AdornmentSession : IAdornmentSession, IHasEnsureHelper, IHasLogger
{
    #region fields

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly IViAdornment _viAdornment;
    private readonly ICommandBus _commandBus;
    private readonly ICompilationTypeQueryFactory _compilationTypeQueryFactory;
    private readonly IAssignableTypesQuery _assignableTypesQuery;
    private readonly IUiEventHub _uiEventHub;
    private readonly ICompilationQuery _compilationQuery;
    private IPeekSession _peekSession;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="AdornmentSession"/> class.
    /// </summary>
    /// <param name="viAdornment">The adornment which is currently open.</param>
    /// <param name="commandBus">The event bus for application public communication.</param>
    /// <param name="compilationTypeQueryFactory"></param>
    /// <param name="assignableTypesQuery"></param>
    /// <param name="uiEventHub"></param>
    /// <param name="compilationQuery"></param>
    public AdornmentSession(
        IViAdornment viAdornment,
        ICommandBus commandBus,
        ICompilationTypeQueryFactory compilationTypeQueryFactory,
        IAssignableTypesQuery assignableTypesQuery,
        IUiEventHub uiEventHub,
        ICompilationQuery compilationQuery)
    {
        this.EnsureMany()
            .Parameter(viAdornment, nameof(viAdornment))
            .Parameter(commandBus, nameof(commandBus))
            .Parameter(compilationTypeQueryFactory, nameof(compilationTypeQueryFactory))
            .Parameter(assignableTypesQuery, nameof(assignableTypesQuery))
            .Parameter(uiEventHub, nameof(uiEventHub))
            .Parameter(compilationQuery, nameof(compilationQuery))
            .ThrowWhenNull();

        this._viAdornment = viAdornment;
        this._commandBus = commandBus;
        this._compilationTypeQueryFactory = compilationTypeQueryFactory;
        this._assignableTypesQuery = assignableTypesQuery;
        this._uiEventHub = uiEventHub;
        this._compilationQuery = compilationQuery;
    }

    #endregion

    #region properties

    /// <inheritdoc />
    public IEnsureHelper EnsureHelper { get; set; }

    /// <inheritdoc />
    public ILogger Logger { get; set; }

    #endregion

    #region members

    /// <summary>
    /// Close the session.
    /// </summary>
    /// <returns>A task.</returns>
    public async Task CloseAsync()
    {
        try
        {
            this._cancellationTokenSource?.Cancel();
            this._viAdornment.UpdateIsExpanded(false);

            await this.RaiseAdornmentSessionClosedAsync();
            await this.ClosePeekSessionAsync();
        }
        finally
        {
            this.Dispose();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (this._peekSession is { IsDismissed: false })
        {
            this._peekSession.Dismiss();
        }

        if (!this._cancellationTokenSource.IsCancellationRequested)
        {
            this._cancellationTokenSource.Cancel();
        }

        this._cancellationTokenSource.Dispose();
    }

    /// <inheritdoc/>
    public async Task StartAsync(
        ITextView textView,
        SnapshotSpan snapshotSpan,
        IFixtureItemPeekResultContent fixtureItemPeekResultContent,
        IDocumentWriter documentWriter,
        IPeekBroker peekBroker)
    {
        // Get the cancellation source for this adornment. If a close is requested this token will be canceled.
        var cancellationToken = this._cancellationTokenSource.Token;

        var maybeCompilation = await this._compilationQuery.GetFromBufferAsync(textView.TextBuffer, cancellationToken);

        if (maybeCompilation.IsNone)
        {
            this.Log($"Cannot find the compilation of the document {textView.TextBuffer.GetFileName()}.", LogLevel.Error);
            return;
        }

        var compilation = maybeCompilation.GetValueUnsafe();

        var compilationTypeQuery = this._compilationTypeQueryFactory.Create(compilation);

        await this.RaiseSetProjectNameCommandAsync();

        await fixtureItemPeekResultContent.InitializeAsync(
            this._viAdornment.Id,
            this._viAdornment.AdornmentInformation,
            documentWriter,
            this._viAdornment.StatusPanelViewModel,
            compilationTypeQuery,
            cancellationToken);

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        this.TriggerPeekSession(
            textView,
            peekBroker,
            fixtureItemPeekResultContent,
            this._viAdornment,
            snapshotSpan);

        Task.Run(() => this.InitTypeSystemAsync(compilationTypeQuery), cancellationToken)
            .Forget();
    }

    private async Task InitTypeSystemAsync(ICompilationTypeQuery compilationTypeQuery)
    {
        try
        {
            await this._assignableTypesQuery.InitializeAsync(compilationTypeQuery);
            this._uiEventHub.Publish(new AdornmentTypesInitializedEvent());
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            this.Log(ex);
        }
    }

    private void TriggerPeekSession(
        ITextView textView,
        IPeekBroker peekBroker,
        IFixtureItemPeekResultContent fixtureItemPeekResultContent,
        IViAdornment viAdornment,
        SnapshotSpan snapshotSpan)
    {
        var options = new PeekSessionCreationOptions(
            textView,
            viAdornment.Id.ToString(),
            snapshotSpan.Snapshot.CreateTrackingPoint(
                snapshotSpan.End,
                PointTrackingMode.Negative),
            allowUserResize: false,
            shouldFocusOnLoad: false,
            defaultHeight: fixtureItemPeekResultContent.ControlHeight);

        this._peekSession = peekBroker.TriggerPeekSession(options);
    }

    private async Task ClosePeekSessionAsync()
    {
        this._peekSession?.Dismiss();
        await Task.Delay(50); // wait till the peek view is closed
    }

    private Task RaiseAdornmentSessionClosedAsync()
    {
        var id = this._viAdornment.AdornmentInformation.FixtureItemId;
        var command = new EndFixtureItemConfigurationCommand(id);
        return this._commandBus.SendAsync(command);
    }

    private Task RaiseSetProjectNameCommandAsync()
    {
        var id = this._viAdornment.AdornmentInformation.FixtureItemId;
        var projectName = this._viAdornment.AdornmentInformation.ProjectName;
        var documentPath = this._viAdornment.AdornmentInformation.DocumentFilePath;
        var invocationSpan = this._viAdornment.AdornmentInformation.ObjectCreationSpan;

        var command = new StartFixtureItemConfigurationCommand(
            id,
            projectName,
            documentPath,
            invocationSpan);
        return this._commandBus.SendAsync(command);
    }

    #endregion
}