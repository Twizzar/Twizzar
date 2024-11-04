using System.Linq;
using Microsoft.CodeAnalysis;
using Twizzar.Design.Shared.Infrastructure;

namespace Twizzar.Design.Infrastructure.VisualStudio.Extensions;

/// <summary>
/// Extension methods for <see cref="Project"/>.
/// </summary>
public static class RoslynProjectExtensions
{
    /// <summary>
    /// Check if the project has the twizzar analyzer referenced.
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public static bool HasTwizzarAnalyzer(this Project project) =>
        project?.AnalyzerReferences
            .Any(
                reference =>
                    reference.FullPath?.Contains(ApiNames.AnalyzerProjectNamePrefix) ??
                    false) ??
        false;
}