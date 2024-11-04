using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Twizzar.SharedKernel.Infrastructure.Helpers;

/// <summary>
/// Instance which decides what do to with monitored events.
/// </summary>
public interface IMonitorInstance
{
    #region properties

    /// <summary>
    /// Gets the current context that will be used to augment telemetry you send.
    /// </summary>
    public TelemetryContext Context { get; }

    #endregion

    #region members

    /// <summary>
    /// Check to determine if the tracking is enabled.
    /// </summary>
    /// <returns></returns>
    bool IsEnabled();

    /// <summary>
    /// Start operation creates an operation object with a respective telemetry item.
    /// </summary>
    /// <param name="operationName">Name of the operation that customer is planning to propagate.</param>
    /// <returns>Operation item object with a new telemetry item having current start time and timestamp.</returns>
    IOperationHolder<RequestTelemetry> StartOperation(string operationName);

    /// <summary>
    /// Send an <see cref="EventTelemetry"/> for display in Diagnostic Search and in the Analytics Portal.
    /// </summary>
    /// <remarks>
    /// <a href="https://go.microsoft.com/fwlink/?linkid=525722#trackevent">Learn more</a>
    /// </remarks>
    /// <param name="eventName">A name for the event.</param>
    /// <param name="properties">Named string values you can use to search and classify events.</param>
    /// <param name="metrics">Measurements associated with this event.</param>
    void TrackEvent(
        string eventName,
        IDictionary<string, string> properties = null,
        IDictionary<string, double> metrics = null);

    /// <summary>
    /// Send an <see cref="EventTelemetry"/> for display in Diagnostic Search and in the Analytics Portal.
    /// Create a separate <see cref="EventTelemetry"/> instance for each call to <see cref="TrackEvent(EventTelemetry)"/>.
    /// </summary>
    /// <remarks>
    /// <a href="https://go.microsoft.com/fwlink/?linkid=525722#trackevent">Learn more</a>
    /// </remarks>
    /// <param name="telemetry">An event log item.</param>
    void TrackEvent(EventTelemetry telemetry);

    /// <summary>
    /// Tracks the specified value.<br />
    /// An aggregate representing tracked values will be automatically sent to the cloud ingestion endpoint at the end of each aggregation period.<br />
    /// This method uses the zero-dimensional <c>MetricSeries</c> associated with this metric.
    /// Use <c>TrackValue(..)</c> to track values into <c>MetricSeries</c> associated with specific dimension-values in multi-dimensional metrics.
    /// </summary>
    /// <param name="metricId">The ID (name) of the metric.</param>
    /// <param name="value">The value to be aggregated.</param>
    void TrackMetric(string metricId, double value);

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
    void TrackDependency(
        string dependencyTypeName,
        string dependencyName,
        string data,
        DateTimeOffset startTime,
        TimeSpan duration,
        bool success);

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
    void TrackDependency(
        string dependencyTypeName,
        string target,
        string dependencyName,
        string data,
        DateTimeOffset startTime,
        TimeSpan duration,
        string resultCode,
        bool success);

    /// <summary>
    /// Send information about external dependency call in the application.
    /// Create a separate <see cref="DependencyTelemetry"/> instance for each call to <see cref="TrackDependency(DependencyTelemetry)"/>.
    /// </summary>
    /// <remarks>
    /// <a href="https://go.microsoft.com/fwlink/?linkid=525722#trackdependency">Learn more</a>
    /// </remarks>
    /// <param name="telemetry"></param>
    void TrackDependency(DependencyTelemetry telemetry);

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
    void TrackAvailability(
        string name,
        DateTimeOffset timeStamp,
        TimeSpan duration,
        string runLocation,
        bool success,
        string message = null,
        IDictionary<string, string> properties = null,
        IDictionary<string, double> metrics = null);

    /// <summary>
    /// Send information about availability of an application.
    /// Create a separate <see cref="AvailabilityTelemetry"/> instance for each call to <see cref="TrackAvailability(AvailabilityTelemetry)"/>.
    /// </summary>
    /// <remarks>
    /// <a href="https://go.microsoft.com/fwlink/?linkid=517889">Learn more</a>
    /// </remarks>
    /// <param name="telemetry"></param>
    void TrackAvailability(AvailabilityTelemetry telemetry);

    /// <summary>
    /// Send information about the page viewed in the application.
    /// </summary>
    /// <param name="name">Name of the page.</param>
    /// <remarks>
    /// <a href="https://go.microsoft.com/fwlink/?linkid=525722#page-views">Learn more</a>
    /// </remarks>
    void TrackPageView(string name);

    /// <summary>
    /// Send information about the page viewed in the application.
    /// Create a separate <see cref="PageViewTelemetry"/> instance for each call to <see cref="TrackPageView(PageViewTelemetry)"/>.
    /// </summary>
    /// <remarks>
    /// <a href="https://go.microsoft.com/fwlink/?linkid=525722#page-views">Learn more</a>
    /// </remarks>
    /// <param name="telemetry"></param>
    void TrackPageView(PageViewTelemetry telemetry);

    /// <summary>
    /// Flushes the in-memory buffer and any metrics being pre-aggregated.
    /// </summary>
    /// <remarks>
    /// <a href="https://go.microsoft.com/fwlink/?linkid=525722#flushing-data">Learn more</a>
    /// </remarks>
    void Flush();

    /// <summary>
    /// Asynchronously Flushes the in-memory buffer and any metrics being pre-aggregated.
    /// </summary>
    /// <returns>
    /// Returns true when telemetry data is transferred out of process (application insights server or local storage) and are emitted before the flush invocation.
    /// Returns false when transfer of telemetry data to server has failed with non-retriable http status.
    /// FlushAsync on InMemoryChannel always returns true, as the channel offers minimal reliability guarantees and doesn't retry sending telemetry after a failure.
    /// </returns>
    /// <param name="cancellationToken"></param>
    Task<bool> FlushAsync(CancellationToken cancellationToken);

    #endregion
}