using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Infrastructure.VisualStudio.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.Functional.Monads.MaybeMonad;
using Task = System.Threading.Tasks.Task;

namespace Twizzar.Design.Infrastructure.VisualStudio.Services
{
    /// <inheritdoc/>
    [ExcludeFromCodeCoverage] // Only works when the addin is running
    public class VsProjectManager : IVsProjectManager
    {
        #region fields

        private const string DependentUpon = "DependentUpon";

        #endregion

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        /// <inheritdoc />
        public Maybe<string> FindProjectName(string projectFileName)
        {
            static string GetProjectName(Project project)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                return project.Name;
            }

            return FindProjectFromFile(projectFileName)
                .Map(GetProjectName);
        }

        /// <inheritdoc />
        public async Task<Maybe<string>> GetProjectPath(string projectName)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            // project FullName not accessible for unloaded projects.
            // unique name is relative path from solution and available for unloaded projects.
            return
                from p in FindProjectFromName(projectName)
                from s in Maybe.ToMaybe(Path.GetDirectoryName(((DTE2)p.DTE).Solution.FullName))
                let absoluteProjectName = Path.Combine(s, p.UniqueName)
                select Path.GetDirectoryName(absoluteProjectName);
        }

        /// <summary>
        /// Sets the given file dependent upon another one.
        /// </summary>
        /// <param name="childName">The file to nest.</param>
        /// <param name="parentName">The file to set dependent.</param>
        /// <returns>An awaitable task.</returns>
        public async Task SetDependentUponAsync(string childName, string parentName)
        {
            InvalidateSetDependentUpon(childName, parentName);

            var project = FindProjectFromFile(childName);

            async Task<Maybe<ProjectItem>> FindProjectItem(Project p, string cn)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                return p.GetProjectItemsFromSelfAndDescendants().FirstOrNone(i =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    return i.Name == cn;
                });
            }

            var item = await project.BindAsync(p => FindProjectItem(p, childName));

            if (await ContainsPropertyAsync(item, DependentUpon, childName))
            {
                return;
            }

            async Task SetDependentUpon(ProjectItem pi, string pn)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                pi.Properties.Item(DependentUpon).Value = pn;
            }

            await item.IfSomeAsync(async i => await SetDependentUpon(i, parentName));
        }

        private static void InvalidateSetDependentUpon(string childName, string parentName)
        {
            if (childName == null)
                throw new ArgumentNullException(nameof(childName));

            if (parentName == null)
                throw new ArgumentNullException(nameof(parentName));
        }

        private static async Task<bool> ContainsPropertyAsync(Maybe<ProjectItem> projectItem, string propertyName, string childName)
        {
            async Task<bool> InternalContainsProperty(ProjectItem p, string pn, string cn)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                return p.Properties
                    .Cast<Property>()
                    .Any(i => i?.Name == pn && i?.Value?.Equals(cn) is true);
            }

            var result = await projectItem
                .MapAsync(async p => await InternalContainsProperty(p, propertyName, childName))
                .AsMaybeValueAsync();

            return result is SomeValue<bool> { Value: true };
        }

        private static Maybe<Project> FindProjectFromFile(string fileName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (string.IsNullOrEmpty(fileName))
            {
                return Maybe.None();
            }

            var dte2 = (DTE2)Package.GetGlobalService(typeof(SDTE));
            var projectItem = dte2?.Solution?.FindProjectItem(fileName);
            return projectItem != null ? Maybe.Some(projectItem.ContainingProject) : Maybe.None();
        }

        private static Maybe<Project> FindProjectFromName(string projectName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (string.IsNullOrEmpty(projectName))
            {
                return Maybe.None();
            }

            var dte2 = (DTE2)Package.GetGlobalService(typeof(SDTE));
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
            var project = dte2?.Solution?.GetAllProjects()?.FirstOrDefault(p => p.Name == projectName);
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
            return Maybe.ToMaybe(project);
        }
    }
}
