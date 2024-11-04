using Microsoft.CodeAnalysis;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <summary>
    /// Extenstion methods for <see cref="Solution"/>.
    /// </summary>
    public static class RoslynSolutionExtensions
    {
        /// <summary>
        /// Gets the <see cref="Project"/> by its project name.
        /// <remarks>When the project has many target frameworks roslyn adds the frameworks to the project name.</remarks>
        /// </summary>
        /// <param name="solution">The solution where the project should be searched for.</param>
        /// <param name="projectName">The project name to search.</param>
        /// <returns>Some <see cref="Project"/> when found else None.</returns>
        public static Maybe<Project> GetProjectByName(this Solution solution, string projectName) =>
           solution.Projects.FirstOrNone(p => p.Name == projectName)

                // Workaround to find project when different Frameworks are used.
                .BindNone(() =>
                    solution.Projects.FirstOrNone(p => p.Name.StartsWith(projectName)));
    }
}