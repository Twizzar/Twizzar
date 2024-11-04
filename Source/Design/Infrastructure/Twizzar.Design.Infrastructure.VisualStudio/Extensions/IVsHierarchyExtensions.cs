using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ViCommon.Functional.Monads.MaybeMonad;
using static Microsoft.VisualStudio.VSConstants;

namespace Twizzar.Design.Infrastructure.VisualStudio.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IVsHierarchy"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class IVsHierarchyExtensions
    {
        /// <summary>
        /// Converts a <see cref="IVsHierarchy"/> to a <see cref="Project"/>.
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        /// <returns>A <see cref="Project"/>.</returns>
        /// <exception cref="ArgumentException">Invalid hierarchy, the hierarchy is not a project.</exception>
        /// <exception cref="COMException">Must be called on the ui thread.</exception>
        /// <exception cref="ArgumentNullException">When hierarchy is null.</exception>
        public static Maybe<Project> ToDteProject(this IVsHierarchy hierarchy)
        {
            if (hierarchy == null)
            {
                throw new ArgumentNullException(nameof(hierarchy));
            }

            ThreadHelper.ThrowIfNotOnUIThread();
            if (hierarchy.GetProperty(VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out var prjObject) >= 0)
            {
                return Maybe.Some((Project)prjObject);
            }

            return Maybe.None();
        }
    }
}
