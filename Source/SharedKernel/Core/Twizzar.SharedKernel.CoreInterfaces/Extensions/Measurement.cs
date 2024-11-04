using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Twizzar.SharedKernel.NLog.Logging;

namespace Twizzar.SharedKernel.CoreInterfaces.Extensions;

/// <summary>
/// Helper class for measuring performance.
/// </summary>
[ExcludeFromCodeCoverage]
public class Measurement
{
    private readonly Stopwatch _stopwatch;
    private readonly string _className;

    /// <summary>
    /// Initializes a new instance of the <see cref="Measurement"/> class.
    /// </summary>
    /// <param name="className"></param>
    public Measurement(string className)
    {
        this._className = className;
        this._stopwatch = Stopwatch.StartNew();
    }

    /// <summary>
    /// End a measurement and start a new one.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="callerMemberName"></param>
    public void Measure(string name = "", [CallerMemberName] string callerMemberName = null)
    {
        this.Log($"{name} at {this._className}.{callerMemberName}: {this._stopwatch.ElapsedMilliseconds}ms");
        this._stopwatch.Restart();
    }
}