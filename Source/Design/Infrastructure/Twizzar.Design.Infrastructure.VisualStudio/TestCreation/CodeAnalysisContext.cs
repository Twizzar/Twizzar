using Microsoft.CodeAnalysis;

using Twizzar.Design.CoreInterfaces.TestCreation;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation;

/// <summary>
/// Wrapper for the <see cref="IRoslynContext"/>.
/// </summary>
/// <param name="Document"></param>
public record CodeAnalysisContext(Document Document) : ICodeAnalysisContext;

/// <summary>
/// Extension method for the <see cref="ICodeAnalysisContext"/>.
/// </summary>
public static class CodeAnalysisContextExtension
{
    /// <summary>
    /// Get the roslyn context.
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static Document GetDocument(this ICodeAnalysisContext self) =>
        ((CodeAnalysisContext)self).Document;
}