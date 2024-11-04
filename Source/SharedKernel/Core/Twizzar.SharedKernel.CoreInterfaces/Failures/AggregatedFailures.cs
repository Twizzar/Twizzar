using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.Failures;

/// <summary>
/// Represents an aggregation of failures.
/// </summary>
[ExcludeFromCodeCoverage]
public class AggregatedFailures : Failure
{
    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregatedFailures"/> class.
    /// </summary>
    /// <param name="failures"></param>
    public AggregatedFailures(params Failure[] failures)
        : base(ConstructMessage(failures))
    {
        this.Failures = failures;
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets the failures.
    /// </summary>
    public Failure[] Failures { get; }

    #endregion

    #region members

    private static string ConstructMessage(IEnumerable<Failure> failures) =>
        failures.Select(failure => failure.Message)
            .ToDisplayString(Environment.NewLine);

    #endregion
}