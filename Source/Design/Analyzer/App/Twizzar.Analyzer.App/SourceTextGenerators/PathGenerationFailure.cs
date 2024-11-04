using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Analyzer2022.App.SourceTextGenerators;

/// <summary>
/// Failure when try to generate a path for a fixture item.
/// </summary>
[ExcludeFromCodeCoverage]
public class PathGenerationFailure : Failure
{
    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="PathGenerationFailure"/> class.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="fixtureItemSymbol"></param>
    /// <param name="diagnostic"></param>
    public PathGenerationFailure(string message, ITypeSymbol fixtureItemSymbol, DiagnosticDescriptor diagnostic)
        : base(message)
    {
        this.FixtureItemSymbol = fixtureItemSymbol;
        this.Diagnostic = diagnostic;
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets the fixture item symbol.
    /// </summary>
    public ITypeSymbol FixtureItemSymbol { get; }

    /// <summary>
    /// Gets the diagnostic descriptior key for the failure.
    /// </summary>
    public DiagnosticDescriptor Diagnostic { get; }

    #endregion
}