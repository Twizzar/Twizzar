using System.IO;
using Microsoft.CodeAnalysis;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <summary>
    /// Extension methods for <see cref="Project"/>.
    /// </summary>
    public static class RoslynProjectExtensions
    {
        /// <summary>
        /// Gets the file name without its extension and the dot.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>The project name.</returns>
        public static string GetFileNameWithoutExtension(this Project project) =>
            Path.GetFileNameWithoutExtension(project.FilePath);
    }
}