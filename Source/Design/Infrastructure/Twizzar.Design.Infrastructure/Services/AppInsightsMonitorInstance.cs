using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.Infrastructure.Helpers;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.Services;

/// <summary>
/// Application Insights implementation of <see cref="IMonitorInstance"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public class AppInsightsMonitorInstance : IMonitorInstance, IEventListener<AnalyticsEnabledOrDisabledEvent>
{
    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="AppInsightsMonitorInstance"/> class.
    /// </summary>
    /// <param name="appName"></param>
    /// <param name="isEnabled"></param>
    public AppInsightsMonitorInstance(string appName, bool isEnabled)
    {
        var configuration = TelemetryConfiguration.CreateDefault();

        configuration.ConnectionString =
            "InstrumentationKey=66df3071-78db-4f79-a69a-879a265ae8a9;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/";

        configuration.TelemetryInitializers.Clear();
        configuration.DisableTelemetry = !isEnabled;

        this.Client = new TelemetryClient(configuration)
        {
            Context =
            {
                User =
                {
                    Id = Environment.UserName.GetHashCode().ToString(),
                    AccountId = string.Empty,
                    UserAgent = string.Empty,
                    AuthenticatedUserId = string.Empty,
                },
                Device =
                {
                    Id = Environment.MachineName.GetHashCode().ToString(),
                    Model = string.Empty,
                    OemName = string.Empty,
                    OperatingSystem = string.Empty,
                    Type = string.Empty,
                },
                Session =
                {
                    Id = Guid.NewGuid().ToString(),
                },
                Cloud =
                {
                    RoleInstance = "None",
                    RoleName = string.Empty,
                },
                Component = { Version = Assembly.GetExecutingAssembly().GetName().Version.ToString() },
                GlobalProperties = { { "appName", appName } },
                Location = { Ip = string.Empty },
            },
        };
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets the telemetry client.
    /// </summary>
    public TelemetryClient Client { get; }

    /// <inheritdoc />
    public TelemetryContext Context => this.Client.Context;

    /// <inheritdoc />
    public bool IsListening => true;

    /// <inheritdoc />
    public Maybe<SynchronizationContext> SynchronizationContext { get; }

    #endregion

    #region members

    /// <inheritdoc/>
    public IOperationHolder<RequestTelemetry> StartOperation(string operationName) =>
        this.Client.StartOperation<RequestTelemetry>(operationName);

    /// <inheritdoc />
    public bool IsEnabled() => this.Client.IsEnabled();

    /// <inheritdoc />
    public void TrackEvent(
        string eventName,
        IDictionary<string, string> properties = null,
        IDictionary<string, double> metrics = null) =>
        this.Client.TrackEvent(eventName, properties, metrics);

    /// <inheritdoc />
    public void TrackEvent(EventTelemetry telemetry) => this.Client.TrackEvent(telemetry);

    /// <inheritdoc />
    public void TrackMetric(string metricId, double value)
    {
        var metric = this.Client.GetMetric(metricId);
        metric.TrackValue(value);
    }

    /// <inheritdoc />
    public void TrackDependency(
        string dependencyTypeName,
        string dependencyName,
        string data,
        DateTimeOffset startTime,
        TimeSpan duration,
        bool success) =>
        this.Client.TrackDependency(dependencyTypeName, dependencyName, data, startTime, duration, success);

    /// <inheritdoc />
    public void TrackDependency(
        string dependencyTypeName,
        string target,
        string dependencyName,
        string data,
        DateTimeOffset startTime,
        TimeSpan duration,
        string resultCode,
        bool success) =>
        this.Client.TrackDependency(
            dependencyTypeName,
            target,
            dependencyName,
            data,
            startTime,
            duration,
            resultCode,
            success);

    /// <inheritdoc />
    public void TrackDependency(DependencyTelemetry telemetry) => this.Client.TrackDependency(telemetry);

    /// <inheritdoc />
    public void TrackAvailability(
        string name,
        DateTimeOffset timeStamp,
        TimeSpan duration,
        string runLocation,
        bool success,
        string message = null,
        IDictionary<string, string> properties = null,
        IDictionary<string, double> metrics = null) =>
        this.Client.TrackAvailability(name, timeStamp, duration, runLocation, success, message, properties, metrics);

    /// <inheritdoc />
    public void TrackAvailability(AvailabilityTelemetry telemetry) => this.Client.TrackAvailability(telemetry);

    /// <inheritdoc />
    public void TrackPageView(string name) => this.Client.TrackPageView(name);

    /// <inheritdoc />
    public void TrackPageView(PageViewTelemetry telemetry) => this.Client.TrackPageView(telemetry);

    /// <inheritdoc />
    public void Flush() => this.Client.Flush();

    /// <inheritdoc />
    public Task<bool> FlushAsync(CancellationToken cancellationToken) => this.Client.FlushAsync(cancellationToken);

    /// <inheritdoc />
    public void Handle(AnalyticsEnabledOrDisabledEvent e)
    {
        this.Client.TelemetryConfiguration.DisableTelemetry = !e.Enabled;
    }

    #endregion
}