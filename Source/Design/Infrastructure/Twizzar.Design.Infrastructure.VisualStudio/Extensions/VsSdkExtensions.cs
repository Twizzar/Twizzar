using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using EnvDTE;
using EnvDTE80;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace Twizzar.Design.Infrastructure.VisualStudio.Extensions
{
    /// <summary>
    /// Helper class for Visual Studio SDK calls.
    /// </summary>
    [ExcludeFromCodeCoverage] // Has dependencies to Vs SDK.
    public static class VsSdkExtensions
    {
        /// <summary>
        /// Gets all projects for a solution.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <returns>A sequence of all project.</returns>
        public static IEnumerable<Project> GetAllProjects(this Solution solution)
        {
            var projects = solution?.Projects?.SafeCast<Project>();

            if (projects == null)
            {
                return Enumerable.Empty<Project>();
            }

            var projectsList = projects.ToList();

            return GetProjects(projectsList);
        }

        /// <summary>
        /// Gets a flattened collection of <see cref="Project"/>, which only contains actual prjects and not directories or hirachial level of the solution.
        /// </summary>
        /// <param name="self">The collection of projects.</param>
        /// <returns>An collation of <see cref="Project"/>.</returns>
        public static IEnumerable<Project> GetProjects(this IEnumerable<Project> self)
        {
            var resultProjects = new List<Project>();
            foreach (var project in self)
            {
                if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    resultProjects.AddRange(GetProjects(project.GetSubProjects()));
                }
                else
                {
                    resultProjects.Add(project);
                }
            }

            return resultProjects;
        }

        /// <summary>
        /// Gets all sub-project items in the project.
        /// </summary>
        /// <param name="self">The project.</param>
        /// <returns>The SubProject as <see cref="Project"/> of each ProjectItem.</returns>
        public static IEnumerable<Project> GetSubProjects(this Project self)
        {
            List<Project> containedProjects = new();
            for (var i = 1; i <= self.ProjectItems.Count; i++)
            {
                var subProject = self.ProjectItems.Item(i).SubProject;
                if (subProject != null)
                {
                    containedProjects.Add(subProject);
                }
            }

            return containedProjects;
        }

        /// <summary>
        /// Gets the project items.
        /// </summary>
        /// <param name="self">The project.</param>
        /// <returns>A sequence of project items.</returns>
        public static IEnumerable<ProjectItem> GetProjectItems(this Project self) =>
            self?.ProjectItems != null
                ? self.ProjectItems.SafeCast<ProjectItem>()
                : Enumerable.Empty<ProjectItem>();

        /// <summary>
        /// Gets the project items.
        /// </summary>
        /// <param name="self">The project.</param>
        /// <returns>A sequence of project items.</returns>
        public static IEnumerable<ProjectItem> GetProjectItems(this ProjectItem self) =>
            self?.ProjectItems != null
                ? self.ProjectItems.SafeCast<ProjectItem>()
                : Enumerable.Empty<ProjectItem>();

        /// <summary>
        /// Gets the code models.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>A sequence of code elements.</returns>
        public static IEnumerable<CodeElement> GetCodeModels(this FileCodeModel self) =>
            self?.CodeElements != null
                ? self.CodeElements.SafeCast<CodeElement>()
                : Enumerable.Empty<CodeElement>();

        /// <summary>
        /// Gets the project items from self and all descendants recursively.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>All found project items.</returns>
        public static IEnumerable<ProjectItem> GetProjectItemsFromSelfAndDescendants(this Project self) =>
            self.GetProjectItems()
                .SelectMany(item => item.GetProjectItemsFromSelfAndDescendants());

        /// <summary>
        /// Gets the project items from self and descendants recursively.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>All found project items.</returns>
        public static IEnumerable<ProjectItem> GetProjectItemsFromSelfAndDescendants(this ProjectItem self) =>
            self.GetProjectItems()
                .Where(item => item != null)
                .SelectMany(item => item.GetProjectItemsFromSelfAndDescendants())
                .Prepend(self);

        /// <summary>
        /// Gets the code elements from self and descendants recursively.
        /// </summary>
        /// <param name="projectItems">The project items.</param>
        /// <returns>All found code elements.</returns>
        public static IEnumerable<CodeElement> GetCodeElementsFromSelfAndDescendants(this IEnumerable<ProjectItem> projectItems) =>
            projectItems
                .Where(projectItem => projectItem?.FileCodeModel != null)
                .SelectMany(projectItem => projectItem.FileCodeModel.GetCodeModels()
                    .SelectMany(element => element.GetCodeElementFromSelfAndDescendants()));

        /// <summary>
        /// Gets the code element from self and descendants recursively.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>All found code elements.</returns>
        public static IEnumerable<CodeElement> GetCodeElementFromSelfAndDescendants(this CodeElement self)
        {
            yield return self;
            if (self.Children != null)
            {
                foreach (var descendant in self.Children
                    .SafeCast<CodeElement>()
                    .SelectMany(element => element.GetCodeElementFromSelfAndDescendants()))
                {
                    yield return descendant;
                }
            }
        }

        /// <summary>
        /// Cast all elements to the type T if possible else ignore the element.
        /// </summary>
        /// <typeparam name="T">Type to cast.</typeparam>
        /// <param name="self">The non typed sequence.</param>
        /// <returns>All elements castable to T.</returns>
        public static IEnumerable<T> SafeCast<T>(this IEnumerable self)
        {
            foreach (var o in self)
            {
                if (o is T t)
                {
                    yield return t;
                }
            }
        }
    }
}
