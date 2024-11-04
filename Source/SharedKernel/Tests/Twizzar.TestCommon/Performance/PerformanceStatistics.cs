using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

using Twizzar.SharedKernel.CoreInterfaces.Extensions;

namespace Twizzar.TestCommon.Performance;

/// <summary>
/// Class for measuring performance.
/// </summary>
[ExcludeFromCodeCoverage]
public class PerformanceStatistics
{
    #region fields

    private readonly Action _action;
    private readonly bool _preventGarbageCollection;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceStatistics"/> class.
    /// </summary>
    /// <param name="actionToMeasure">The action to measure.</param>
    /// <param name="preventGarbageCollection">Garbage collection is prevented in the actionToMeasure when this is set to true.</param>
    public PerformanceStatistics(Action actionToMeasure, bool preventGarbageCollection = true)
    {
        this._action = actionToMeasure;
        this._preventGarbageCollection = preventGarbageCollection;
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets the Max size to prevent garbage collecting, only apply when preventGarbageCollection is true.
    /// </summary>
    public long MaxGcSizeToPrevent { get; set; } = (long)1E8;

    #endregion

    #region members

    /// <summary>
    /// Do the action till the given microseconds is reached. For warming up the system.
    /// </summary>
    /// <param name="milliseconds">How long to do the warmup.</param>
    public void Warmup(int milliseconds)
    {
        var timer = Stopwatch.StartNew();

        while (timer.ElapsedMilliseconds < milliseconds)
        {
            this._action();
        }
    }

    /// <summary>
    /// Measure the performance.
    /// </summary>
    /// <param name="numberOfActions">How many action one sample should contain.</param>
    /// <param name="samples">Number of samples to collect.</param>
    /// <returns>A <see cref="MeasurementsResults"/>.</returns>
    public MeasurementsResults Measure(int numberOfActions, int samples) =>
        new(
            this.MeasureTime(this._action, numberOfActions, samples).Select(span => span.TotalMilliseconds),
            numberOfActions);

    private static double StandardDeviation(IEnumerable<double> values, double mean) =>
        Math.Sqrt(Variance(values, mean));

    private static double Variance(IEnumerable<double> values, double mean)
    {
        var vs = values.ToList();
        var n = vs.Count;

        return 1f / n * vs.Select(x => Math.Pow(x - mean, 2)).Sum();
    }

    private TimeSpan MeasureTime(Action toTime)
    {
        if (this._preventGarbageCollection && !GC.TryStartNoGCRegion(this.MaxGcSizeToPrevent))
        {
            throw new Exception("Cannot disable GC collection");
        }

        var timer = Stopwatch.StartNew();
        toTime();
        timer.Stop();

        if (this._preventGarbageCollection)
        {
            GC.EndNoGCRegion();
        }

        return timer.Elapsed;
    }

    private TimeSpan MeasureTime(Action toTime, int n)
    {
        return this.MeasureTime(() =>
        {
            for (var i = 0; i < n; i++)
            {
                toTime();
            }
        });
    }

    private IEnumerable<TimeSpan> MeasureTime(Action toTime, int n, int samples)
    {
        for (var i = 0; i < samples; i++)
        {
            yield return this.MeasureTime(toTime, n);
        }
    }

    #endregion

    #region Nested type: MeasurementsResults

    /// <summary>
    /// Contains the result samples of the performance test.
    /// </summary>
    public readonly struct MeasurementsResults
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasurementsResults"/> struct.
        /// </summary>
        /// <param name="measurements">The time measured in milliseconds.</param>
        /// <param name="numberOfExecutions">The number of executions in one sample.</param>
        internal MeasurementsResults(IEnumerable<double> measurements, int numberOfExecutions)
        {
            this.Measurements = measurements.ToList();
            var min = double.PositiveInfinity;
            var max = double.NegativeInfinity;
            var sum = 0.0;
            this.NumberOfExecutions = numberOfExecutions;
            this.Samples = this.Measurements.Count;

            foreach (var x in this.Measurements)
            {
                if (x < min)
                {
                    min = x;
                }

                if (x > max)
                {
                    max = x;
                }

                sum += x;
            }

            this.Avg = sum / this.Samples;
            this.Min = min;
            this.Max = max;
            this.Std = StandardDeviation(this.Measurements, this.Avg);
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the Number of samples.
        /// </summary>
        public int Samples { get; }

        /// <summary>
        /// Gets the number of executions made per samples.
        /// </summary>
        public int NumberOfExecutions { get; }

        /// <summary>
        /// Gets the average runtime.
        /// </summary>
        public double Avg { get; }

        /// <summary>
        /// Gets the minimum runtime.
        /// </summary>
        public double Min { get; }

        /// <summary>
        /// Gets the maximum runtime.
        /// </summary>
        public double Max { get; }

        /// <summary>
        /// Gets the standard deviation of the runtime.
        /// </summary>
        public double Std { get; }

        /// <summary>
        /// Gets the time in milliseconds of all the measurements.
        /// </summary>
        public List<double> Measurements { get; }

        #endregion

        #region members

        /// <summary>
        /// Get how many times this measurement average is slower.
        /// </summary>
        /// <param name="otherAvg">The avg to compare.</param>
        /// <returns>The average factor.
        /// When this has a higher average the value will be less than one else greater equals to one.
        /// Where one means the average is the same.</returns>
        public double GetAvgFactor(double otherAvg) => this.Avg / otherAvg;

        /// <summary>
        /// Get the results as an string.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(
                "Tested with {0} samples for each was executed {1} times. \n",
                this.Samples,
                this.NumberOfExecutions);

            sb.AppendLine(Format("Average runtime:", this.Avg));
            sb.AppendLine(Format("Average runtime for 1 action:", this.Avg / this.NumberOfExecutions));
            sb.AppendLine(Format("Maximum:", this.Max));
            sb.AppendLine(Format("Minimum:", this.Min));
            sb.AppendLine(Format("Standard Deviation:", this.Std));

            return sb.ToString();
        }

        /// <summary>
        /// Get the results as an dictionary.
        /// </summary>
        /// <returns>Dictionary with a meaningful name as keys and the measurements value as values.</returns>
        public Dictionary<string, double> ToDict() =>
            new Dictionary<string, double>()
            {
                { "samples", this.Samples },
                { "number of executions", this.NumberOfExecutions },
                { "average runtime", this.Avg },
                { "Average runtime for one action", this.Avg / this.NumberOfExecutions },
                { "Maximum", this.Max },
                { "Minimum", this.Min },
                { "Standard Deviation", this.Std },
            };

        /// <summary>
        /// Get the results in a csv format.
        /// </summary>
        /// <param name="withHeader">Also include a header.</param>
        /// <param name="horizontal">Print it horizontal 1 row the headers 1 row the values,
        /// or vertical 1 column the header 1 column the values.</param>
        /// <returns>String in a csv format.</returns>
        public string AsCsvFormat(bool withHeader = true, bool horizontal = false)
        {
            var sb = new StringBuilder();

            if (horizontal)
            {
                if (withHeader)
                {
                    sb.AppendLine(
                        this.ToDict()
                            .Keys
                            .Aggregate(string.Empty, (s, s1) => s + "," + s1));
                }

                sb.AppendLine(
                    this.ToDict()
                        .Values
                        .Aggregate(string.Empty, (s, s1) => s + "," + s1));
            }
            else
            {
                foreach (var (key, value) in this.ToDict())
                {
                    if (withHeader)
                    {
                        sb.Append($"{key}, ");
                    }

                    sb.AppendLine(value.ToString(CultureInfo.InvariantCulture));
                }
            }

            return sb.ToString();
        }

        private static string Format(string title, double value) => $"{title,-40}{value,10:#0.0000 ms}";

        #endregion
    }

    #endregion
}