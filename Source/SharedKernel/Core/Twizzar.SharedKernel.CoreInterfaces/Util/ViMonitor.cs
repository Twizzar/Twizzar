using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

using Twizzar.SharedKernel.Infrastructure.Helpers;

namespace Twizzar.SharedKernel.CoreInterfaces.Util;

#pragma warning disable S3881 // "IDisposable" should be implemented correctly
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

/// <summary>
/// Static class to monitor the application.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ViMonitor
{
    #region static fields and constants

    private static IMonitorInstance _instance;

    #endregion

    #region properties

    /// <summary>
    /// Gets the current context that will be used to augment telemetry you send.
    /// </summary>
    public static TelemetryContext Context => _instance?.Context ?? new TelemetryContext();

    #endregion

    #region members

    /// <summary>
    /// Start tracking an operation.
    /// </summary>
    /// <param name="operationName"></param>
    /// <returns></returns>
    public static IOperationHolder<RequestTelemetry> StartOperation(string operationName) =>
        Do(instance => instance.StartOperation(operationName), new EmptyOperationHolder());

    /// <summary>
    /// Set the monitor instance this should be done at the start of the application.
    /// </summary>
    /// <param name="instance"></param>
    public static void SetInstance(IMonitorInstance instance)
    {
        _instance = instance;
    }

    /// <summary>
    /// Check to determine if the tracking is enabled.
    /// </summary>
    /// <returns></returns>
    public static bool IsEnabled() => _instance is not null && _instance.IsEnabled();

    /// <summary>
    /// Send an <see cref="EventTelemetry"/> for display in Diagnostic Search and in the Analytics Portal.
    /// </summary>
    /// <remarks>
    /// <a href="https://go.microsoft.com/fwlink/?linkid=525722#trackevent">Learn more</a>
    /// </remarks>
    /// <param name="eventName">A name for the event.</param>
    /// <param name="properties">Named string values you can use to search and classify events.</param>
    /// <param name="metrics">Measurements associated with this event.</param>
    public static void TrackEvent(
        string eventName,
        IDictionary<string, string> properties = null,
        IDictionary<string, double> metrics = null) =>
        Do(instance => instance.TrackEvent(eventName, properties, metrics));

    /// <summary>
    /// Send an <see cref="EventTelemetry"/> for display in Diagnostic Search and in the Analytics Portal.
    /// Create a separate <see cref="EventTelemetry"/> instance for each call to <see cref="TrackEvent(EventTelemetry)"/>.
    /// </summary>
    /// <remarks>
    /// <a href="https://go.microsoft.com/fwlink/?linkid=525722#trackevent">Learn more</a>
    /// </remarks>
    /// <param name="telemetry">An event log item.</param>
    public static void TrackEvent(EventTelemetry telemetry) => Do(instance => instance.TrackEvent(telemetry));

    /// <summary>
    /// Tracks the specified value.<br />
    /// An aggregate representing tracked values will be automatically sent to the cloud ingestion endpoint at the end of each aggregation period.<br />
    /// This method uses the zero-dimensional <c>MetricSeries</c> associated with this metric.
    /// Use <c>TrackValue(..)</c> to track values into <c>MetricSeries</c> associated with specific dimension-values in multi-dimensional metrics.
    /// </summary>
    /// <param name="metricId">The ID (name) of the metric.</param>
    /// <param name="value">The value to be aggregated.</param>
    public static void TrackMetric(string metricId, double value) =>
        Do(instance => instance.TrackMetric(metricId, value));

    /// <summary>
    /// Send information about an external dependency (outgoing call) in the application.
    /// </summary>
    /// <param name="dependencyTypeName">External dependency type. Very low cardinality value for logical grouping and interpretation of fields. Examples are SQL, Azure table, and HTTP.</param>
    /// <param name="dependencyName">Name of the command initiated with this dependency call. Low cardinality value. Examples are stored procedure name and URL path template.</param>
    /// <param name="data">Command initiated by this dependency call. Examples are SQL statement and HTTP URL's with all query parameters.</param>
    /// <param name="startTime">The time when the dependency was called.</param>
    /// <param name="duration">The time taken by the external dependency to handle the call.</param>
    /// <param name="success">True if the dependency call was handled successfully.</param>
    /// <remarks>
    /// <a href="https://go.microsoft.com/fwlink/?linkid=525722#trackdependency">Learn more</a>
    /// </remarks>
    public static void TrackDependency(
        string dependencyTypeName,
        string dependencyName,
        string data,
        DateTimeOffset startTime,
        TimeSpan duration,
        bool success) =>
        Do(instance =>
            instance.TrackDependency(dependencyTypeName, dependencyName, data, startTime, duration, success));

    /// <summary>
    /// Send information about an external dependency (outgoing call) in the application.
    /// </summary>
    /// <param name="dependencyTypeName">External dependency type. Very low cardinality value for logical grouping and interpretation of fields. Examples are SQL, Azure table, and HTTP.</param>
    /// <param name="target">External dependency target.</param>
    /// <param name="dependencyName">Name of the command initiated with this dependency call. Low cardinality value. Examples are stored procedure name and URL path template.</param>
    /// <param name="data">Command initiated by this dependency call. Examples are SQL statement and HTTP URL's with all query parameters.</param>
    /// <param name="startTime">The time when the dependency was called.</param>
    /// <param name="duration">The time taken by the external dependency to handle the call.</param>
    /// <param name="resultCode">Result code of dependency call execution.</param>
    /// <param name="success">True if the dependency call was handled successfully.</param>
    /// <remarks>
    /// <a href="https://go.microsoft.com/fwlink/?linkid=525722#trackdependency">Learn more</a>
    /// </remarks>
    public static void TrackDependency(
        string dependencyTypeName,
        string target,
        string dependencyName,
        string data,
        DateTimeOffset startTime,
        TimeSpan duration,
        string resultCode,
        bool success) =>
        Do(instance =>
            instance.TrackDependency(
                dependencyTypeName,
                target,
                dependencyName,
                data,
                startTime,
                duration,
                resultCode,
                success));

    /// <summary>
    /// Send information about external dependency call in the application.
    /// Create a separate <see cref="DependencyTelemetry"/> instance for each call to <see cref="TrackDependency(DependencyTelemetry)"/>.
    /// </summary>
    /// <remarks>
    /// <a href="https://go.microsoft.com/fwlink/?linkid=525722#trackdependency">Learn more</a>
    /// </remarks>
    /// <param name="telemetry"></param>
    public static void TrackDependency(DependencyTelemetry telemetry) =>
        Do(instance => instance.TrackDependency(telemetry));

    /// <summary>
    /// Send information about availability of an application.
    /// </summary>
    /// <param name="name">Availability test name.</param>
    /// <param name="timeStamp">The time when the availability was captured.</param>
    /// <param name="duration">The time taken for the availability test to run.</param>
    /// <param name="runLocation">Name of the location the availability test was run from.</param>
    /// <param name="success">True if the availability test ran successfully.</param>
    /// <param name="message">Error message on availability test run failure.</param>
    /// <param name="properties">Named string values you can use to classify and search for this availability telemetry.</param>
    /// <param name="metrics">Additional values associated with this availability telemetry.</param>
    /// <remarks>
    /// <a href="https://go.microsoft.com/fwlink/?linkid=517889">Learn more</a>
    /// </remarks>
    public static void TrackAvailability(
        string name,
        DateTimeOffset timeStamp,
        TimeSpan duration,
        string runLocation,
        bool success,
        string message = null,
        IDictionary<string, string> properties = null,
        IDictionary<string, double> metrics = null) =>
        Do(instance =>
            instance.TrackAvailability(name, timeStamp, duration, runLocation, success, message, properties, metrics));

    /// <summary>
    /// Send information about availability of an application.
    /// Create a separate <see cref="AvailabilityTelemetry"/> instance for each call to <see cref="TrackAvailability(AvailabilityTelemetry)"/>.
    /// </summary>
    /// <remarks>
    /// <a href="https://go.microsoft.com/fwlink/?linkid=517889">Learn more</a>
    /// </remarks>
    /// <param name="telemetry"></param>
    public static void TrackAvailability(AvailabilityTelemetry telemetry) =>
        Do(instance => instance.TrackAvailability(telemetry));

    /// <summary>
    /// Send information about the page viewed in the application.
    /// </summary>
    /// <param name="name">Name of the page.</param>
    /// <remarks>
    /// <a href="https://go.microsoft.com/fwlink/?linkid=525722#page-views">Learn more</a>
    /// </remarks>
    public static void TrackPageView(string name) => Do(instance => instance.TrackPageView(name));

    /// <summary>
    /// Send information about the page viewed in the application.
    /// Create a separate <see cref="PageViewTelemetry"/> instance for each call to <see cref="TrackPageView(PageViewTelemetry)"/>.
    /// </summary>
    /// <remarks>
    /// <a href="https://go.microsoft.com/fwlink/?linkid=525722#page-views">Learn more</a>
    /// </remarks>
    /// <param name="telemetry"></param>
    public static void TrackPageView(PageViewTelemetry telemetry) => Do(instance => instance.TrackPageView(telemetry));

    /// <summary>
    /// Flushes the in-memory buffer and any metrics being pre-aggregated.
    /// </summary>
    /// <remarks>
    /// <a href="https://go.microsoft.com/fwlink/?linkid=525722#flushing-data">Learn more</a>
    /// </remarks>
    public static void Flush() => Do(instance => instance.Flush());

    /// <summary>
    /// Asynchronously Flushes the in-memory buffer and any metrics being pre-aggregated.
    /// </summary>
    /// <returns>
    /// Returns true when telemetry data is transferred out of process (application insights server or local storage) and are emitted before the flush invocation.
    /// Returns false when transfer of telemetry data to server has failed with non-retriable http status.
    /// FlushAsync on InMemoryChannel always returns true, as the channel offers minimal reliability guarantees and doesn't retry sending telemetry after a failure.
    /// </returns>
    /// <param name="cancellationToken"></param>
    public static async Task<bool> FlushAsync(CancellationToken cancellationToken) =>
        _instance is not null && await _instance.FlushAsync(cancellationToken);

    private static void Do(Action<IMonitorInstance> func)
    {
        try
        {
            if (_instance is not null)
            {
                func?.Invoke(_instance);
            }
        }
        catch (Exception)
        {
            // Do noting
        }
    }

    private static T Do<T>(Func<IMonitorInstance, T> func, T onFailure)
    {
        try
        {
            if (_instance is not null && func is not null)
            {
                return func.Invoke(_instance);
            }

            return onFailure;
        }
        catch (Exception)
        {
            return onFailure;
        }
    }

    #endregion

    #region Nested type: EmptyOperationHolder

    /// <summary>
    /// Empty operation holder used on error.
    /// </summary>
    public class EmptyOperationHolder : IOperationHolder<RequestTelemetry>
    {
        #region properties

        /// <inheritdoc />
        public RequestTelemetry Telemetry => new();

        #endregion

        #region members

        /// <inheritdoc />
        public void Dispose()
        {
            // Do nothing
        }

        #endregion
    }

    #endregion
}