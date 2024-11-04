using System.Diagnostics.CodeAnalysis;

namespace Twizzar.Design.CoreInterfaces.TestCreation;

/// <summary>
/// Data for the <see cref="ITestCreationProgress"/>.
/// </summary>
/// <param name="Message"></param>
[ExcludeFromCodeCoverage]
public record TestCreationProgressData(string Message);