using System;
using System.Threading.Tasks;

using Twizzar.Design.CoreInterfaces.Common.Messaging.Events;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.CoreInterfaces.Resources;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Services;

/// <inheritdoc cref="IVersionChecker"/>
public class VersionChecker : IVersionChecker
{
    #region fields

    private readonly IAddinVersionQuery _addinVersionQuery;
    private readonly IViNotificationService _notificationService;
    private Maybe<Version> _addinVersion;
    private DateTime _lastEvent = DateTime.MinValue;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionChecker"/> class.
    /// </summary>
    /// <param name="eventHub"></param>
    /// <param name="addinVersionQuery"></param>
    /// <param name="notificationService"></param>
    public VersionChecker(
        IUiEventHub eventHub,
        IAddinVersionQuery addinVersionQuery,
        IViNotificationService notificationService)
    {
        this.EnsureMany()
            .Parameter(eventHub, nameof(eventHub))
            .Parameter(addinVersionQuery, nameof(addinVersionQuery))
            .Parameter(notificationService, nameof(notificationService))
            .ThrowWhenNull();

        this._addinVersionQuery = addinVersionQuery;
        this._notificationService = notificationService;
        eventHub.Subscribe<TwizzarAnalyzerAddedEvent>(this, this.OnAnalyzerAddedAsync);
    }

    #endregion

    #region properties

    /// <inheritdoc />
    public ILogger Logger { get; set; }

    /// <inheritdoc />
    public IEnsureHelper EnsureHelper { get; set; }

    #endregion

    #region members

    /// <inheritdoc />
    public void Initialize()
    {
        this._addinVersion = ParseVersion(this._addinVersionQuery.GetVsAddinVersion());
    }

    private async Task OnAnalyzerAddedAsync(TwizzarAnalyzerAddedEvent e)
    {
        try
        {
            await this._addinVersion.IfSomeAsync(async version =>
            {
                if ((version.Major != e.Version.Major || version.Minor != e.Version.Minor)
                    && DateTime.Now.Subtract(this._lastEvent).TotalSeconds > 30)
                {
                    this._lastEvent = DateTime.Now;

                    var message = MessagesDesign.TwizzarVersionChecker_Handler_The_Twizzar_API_version_of_one_or_more_projects_does_not_match_the_Twizzar_Addin_version;

                    await this._notificationService.SendToInfoBarAsync(message);
                    await this._notificationService.SendToOutputAsync(message);
                }
            });
        }
        catch (Exception ex)
        {
            this.Log(ex);
        }
    }

    private static Maybe<Version> ParseVersion(string version)
    {
        if (Version.TryParse(version, out var v))
        {
            return v;
        }

        return Maybe.None();
    }

    #endregion
}