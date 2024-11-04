using System.Diagnostics.CodeAnalysis;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions
{
    /// <summary>
    /// Extension method for easy Measuring.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class PerformanceMeasurementExtensions
    {
        /// <summary>
        /// Start a new measurement.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>A new instance of <see cref="Measurement"/>.</returns>
        public static Measurement StartMeasurement(this object obj) =>
            new(obj.GetType().Name);
    }
}