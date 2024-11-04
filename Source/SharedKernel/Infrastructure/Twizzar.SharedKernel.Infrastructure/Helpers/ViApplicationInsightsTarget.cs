using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace Twizzar.SharedKernel.Infrastructure.Helpers;

/// <summary>
/// NLog Target that routes all logging output to the Application Insights logging framework.
/// The messages will be uploaded to the Application Insights cloud service.
/// </summary>
[Target("ApplicationInsightsTarget")]
[ExcludeFromCodeCoverage]
public sealed class ViApplicationInsightsTarget : TargetWithLayout
{
    #region fields

    private DateTime _lastLogEventTime;
    private Layout _instrumentationKeyLayout = string.Empty;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="ViApplicationInsightsTarget"/> class.
    /// </summary>
    /// <param name="telemetryClient"></param>
    public ViApplicationInsightsTarget(TelemetryClient telemetryClient)
    {
        this.TelemetryClient = telemetryClient;
        this.Layout = @"${message}";
        this.OptimizeBufferReuse = true;
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets the Application Insights instrumentationKey for your application.
    /// </summary>
    public string InstrumentationKey
    {
        get => (this._instrumentationKeyLayout as SimpleLayout)?.Text ?? null;
        set => this._instrumentationKeyLayout = value ?? string.Empty;
    }

    /// <summary>
    /// Gets the array of custom attributes to be passed into the logevent context.
    /// </summary>
    [ArrayParameter(typeof(TargetPropertyWithContext), "contextproperty")]
    public IList<TargetPropertyWithContext> ContextProperties { get; } = new List<TargetPropertyWithContext>();

    /// <summary>
    /// Gets the logging controller we will be using.
    /// </summary>
    internal TelemetryClient TelemetryClient { get; }

    #endregion

    #region members

    /// <summary>
    /// Initializes the Target and perform instrumentationKey validation.
    /// </summary>
    /// <exception cref="NLogConfigurationException">Will throw when <see cref="InstrumentationKey"/> is not set.</exception>
    protected override void InitializeTarget()
    {
        base.InitializeTarget();

        var instrumentationKey = this._instrumentationKeyLayout.Render(LogEventInfo.CreateNullEvent());

        if (!string.IsNullOrWhiteSpace(instrumentationKey))
        {
            this.TelemetryClient.Context.InstrumentationKey = instrumentationKey;
        }
    }

    /// <summary>
    /// Send the log message to Application Insights.
    /// </summary>
    /// <param name="logEvent"></param>
    /// <exception cref="ArgumentNullException">If <paramref name="logEvent"/> is null.</exception>
    protected override void Write(LogEventInfo logEvent)
    {
        if (logEvent == null)
        {
            throw new ArgumentNullException(nameof(logEvent));
        }

        this._lastLogEventTime = DateTime.UtcNow;

        if (logEvent.Exception != null)
        {
            this.SendException(logEvent);
        }
        else
        {
            this.SendTrace(logEvent);
        }
    }

    /// <summary>
    /// Flush any pending log messages.
    /// </summary>
    /// <param name="asyncContinuation">The asynchronous continuation.</param>
    protected override void FlushAsync(AsyncContinuation asyncContinuation)
    {
        if (asyncContinuation == null)
        {
            throw new ArgumentNullException(nameof(asyncContinuation));
        }

        try
        {
            this.TelemetryClient.Flush();

            if (DateTime.UtcNow.AddSeconds(-30) > this._lastLogEventTime)
            {
                // Nothing has been written, so nothing to wait for
                asyncContinuation(null);
            }
            else
            {
                // Documentation says it is important to wait after flush, else nothing will happen
                // https://docs.microsoft.com/azure/application-insights/app-insights-api-custom-events-metrics#flushing-data
                Task.Delay(TimeSpan.FromMilliseconds(500)).ContinueWith((task) => asyncContinuation(null));
            }
        }
        catch (Exception ex)
        {
            asyncContinuation(ex);
        }
    }

    private void BuildPropertyBag(LogEventInfo logEvent, ITelemetry trace)
    {
        trace.Timestamp = logEvent.TimeStamp;
        trace.Sequence = logEvent.SequenceID.ToString(CultureInfo.InvariantCulture);

        IDictionary<string, string> propertyBag;

        if (trace is ExceptionTelemetry telemetry)
        {
            propertyBag = telemetry.Properties;
        }
        else
        {
            propertyBag = ((TraceTelemetry)trace).Properties;
        }

        if (!string.IsNullOrEmpty(logEvent.LoggerName))
        {
            propertyBag.Add("LoggerName", logEvent.LoggerName);
        }

        if (logEvent.UserStackFrame != null)
        {
            propertyBag.Add("UserStackFrame", logEvent.UserStackFrame.ToString());

            propertyBag.Add(
                "UserStackFrameNumber",
                logEvent.UserStackFrameNumber.ToString(CultureInfo.InvariantCulture));
        }

        for (var i = 0; i < this.ContextProperties.Count; ++i)
        {
            var contextProperty = this.ContextProperties[i];

            if (!string.IsNullOrEmpty(contextProperty.Name) && contextProperty.Layout != null)
            {
                var propertyValue = this.RenderLogEvent(contextProperty.Layout, logEvent);
                PopulatePropertyBag(propertyBag, contextProperty.Name, propertyValue);
            }
        }

        if (logEvent.HasProperties)
        {
            LoadLogEventProperties(logEvent, propertyBag);
        }
    }

    private static void LoadLogEventProperties(LogEventInfo logEvent, IDictionary<string, string> propertyBag)
    {
        if (logEvent.Properties?.Count > 0)
        {
            foreach (var keyValuePair in logEvent.Properties)
            {
                var key = keyValuePair.Key.ToString();
                var valueObj = keyValuePair.Value;
                PopulatePropertyBag(propertyBag, key, valueObj);
            }
        }
    }

    private static void PopulatePropertyBag(IDictionary<string, string> propertyBag, string key, object valueObj)
    {
        if (valueObj == null)
        {
            return;
        }

        var value = Convert.ToString(valueObj, CultureInfo.InvariantCulture);

        if (propertyBag.ContainsKey(key))
        {
            if (string.Equals(value, propertyBag[key], StringComparison.Ordinal))
            {
                return;
            }

            key += "_1";
        }

        propertyBag.Add(key, value);
    }

    private static SeverityLevel? GetSeverityLevel(LogLevel logEventLevel)
    {
        if (logEventLevel == null)
        {
            return null;
        }

        if (logEventLevel.Ordinal == LogLevel.Trace.Ordinal ||
            logEventLevel.Ordinal == LogLevel.Debug.Ordinal)
        {
            return SeverityLevel.Verbose;
        }

        if (logEventLevel.Ordinal == LogLevel.Info.Ordinal)
        {
            return SeverityLevel.Information;
        }

        if (logEventLevel.Ordinal == LogLevel.Warn.Ordinal)
        {
            return SeverityLevel.Warning;
        }

        if (logEventLevel.Ordinal == LogLevel.Error.Ordinal)
        {
            return SeverityLevel.Error;
        }

        if (logEventLevel.Ordinal == LogLevel.Fatal.Ordinal)
        {
            return SeverityLevel.Critical;
        }

        // The only possible value left if OFF but we should never get here in this case
        return null;
    }

    private void SendException(LogEventInfo logEvent)
    {
        var exceptionTelemetry = new ExceptionTelemetry(logEvent.Exception)
        {
            SeverityLevel = GetSeverityLevel(logEvent.Level),
            Message = $"{logEvent.Exception?.GetType()}: {logEvent.Exception?.Message}",
        };

        exceptionTelemetry.Properties.Add("Method", logEvent.CallerMemberName);
        exceptionTelemetry.Properties.Add("ClassName", logEvent.CallerClassName);
        exceptionTelemetry.Properties.Add("FileName", Path.GetFileName(logEvent.CallerFilePath));
        exceptionTelemetry.Properties.Add("LineNumber", logEvent.CallerLineNumber.ToString());

        var logMessage = this.RenderLogEvent(this.Layout, logEvent);

        if (!string.IsNullOrEmpty(logMessage))
        {
            exceptionTelemetry.Properties.Add("Message", logMessage);
        }

        this.BuildPropertyBag(logEvent, exceptionTelemetry);
        this.TelemetryClient.Track(exceptionTelemetry);
    }

    private void SendTrace(LogEventInfo logEvent)
    {
        var logMessage = this.RenderLogEvent(this.Layout, logEvent);

        var trace = new TraceTelemetry(logMessage)
        {
            SeverityLevel = GetSeverityLevel(logEvent.Level),
        };

        this.BuildPropertyBag(logEvent, trace);
        this.TelemetryClient.Track(trace);
    }

    #endregion
}