using System.Diagnostics.CodeAnalysis;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Twizzar.Design.CoreInterfaces.Command.FixtureItem.Config;

namespace Twizzar.Design.Infrastructure.VisualStudio.Extensions
{
    /// <summary>
    /// Extenstion methods for a vs project.
    /// </summary>
    [ExcludeFromCodeCoverage] // This extension only provides an alternative way to set properties.
    public static class ProjectItemExtensions
    {
        /// <summary>
        /// Changes the copy to output directory property.
        /// </summary>
        /// <param name="self">The project.</param>
        /// <param name="copyToOutputDirectory">The copy to output directory enum.</param>
        public static void ChangeCopyToOutputDirectory(
            this ProjectItem self,
            CopyToOutputDirectory copyToOutputDirectory)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            self.Properties.Item("CopyToOutputDirectory").Value =
                (uint)copyToOutputDirectory;
        }
    }
}
